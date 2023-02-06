using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovableBlock : MonoBehaviour
{
    protected bool isMovable = true;
    protected bool isAnimating = false;
    [FormerlySerializedAs("sphereSprite")] [SerializeReference] public Transform spriteImage;
    [SerializeReference] public LayerMask whatAllowsMovement;
    [SerializeReference] public LayerMask whatStopsMovement;
    [SerializeReference] public float moveSpeed = 5f;
    [SerializeReference] public Transform player;
    protected Vector3 collisionDirection;
    protected bool movedAnotherBlock = false;

    public enum BlockType
    {
        MovableBlock,
        SlipperyBlock
    }
    
    public BlockType blockType;

    //Only compatible with one player
    public bool IsMovable
    {
        set { isMovable = value; }
        get { return isMovable; }
    }
    public bool IsAnimating
    {
        set { isAnimating = value; }
        get { return isAnimating; }
    }
    public Transform SpriteImage
    {
        set { spriteImage = value; }
        get { return spriteImage; }
    }
    protected virtual void Start()
    {
        player = transform.parent.parent.GetChild(1);
        spriteImage.parent = transform.parent;
        blockType = BlockType.MovableBlock;
    }

    protected virtual void FixedUpdate()
    {
        MoveSpriteTowardsGoal();
        
        if (IsSpriteOnGoal(0.1f) && isMovable)
        {
            isAnimating = false;
        }
    }

    protected bool IsSpriteOnGoal(float threshold = 0f)
    {
        if (threshold == 0.0f) return Vector3.Distance(transform.position, spriteImage.position) == 0f;
        
        return Vector3.Distance(transform.position, spriteImage.position) < threshold;
    }

    protected void MoveSpriteTowardsGoal()
    {
        spriteImage.position = Vector3.MoveTowards(spriteImage.position, transform.position,
            moveSpeed * Time.deltaTime * GameManager.Instance.levelScale);
    }

    public bool AttemptToMove(GameObject colliderObject)
    {
        if (isAnimating || !IsSpriteOnGoal(0.1f)) return false;

        var positionOtherObject = colliderObject.transform.position;
        var positionThisObject = transform.position;
        collisionDirection = new Vector3(
            positionOtherObject.x - positionThisObject.x, 
            positionOtherObject.y - positionThisObject.y).normalized;

        // Move block objective to next tile if possible
        var nextTile = positionThisObject - collisionDirection * GameManager.Instance.levelScale;
        if (!Physics2D.OverlapCircle(nextTile, .2f,whatAllowsMovement)
            || isAnimating)
            return false;
        
        // Attempt to move any blocks that would be pushed by this block. If any of them fail, this block fails.
        var collisions = Physics2D.OverlapCircleAll(nextTile, .2f, whatStopsMovement);
        if (collisions.Length == 0)
        {
            return true;
        }
        foreach (var collision in collisions)
        {
            if (!collision.gameObject.CompareTag("MovableBlock")) return false;;
            if (!collision.gameObject.GetComponent<MovableBlock>().AttemptToMove(gameObject))
                return false;
        }
        
        // Move blocks
        foreach (var collision in collisions)
        {
            if (!collision.gameObject.CompareTag("MovableBlock")) continue;
            collision.gameObject.GetComponent<MovableBlock>().MoveBlock();
        }

        return true;
    }

    /**
     * Must be called after AttemptToMove() in order to move the block in the correct direction
     */
    public virtual void MoveBlock()
    {
        transform.position -= collisionDirection * GameManager.Instance.levelScale;
    }
}
