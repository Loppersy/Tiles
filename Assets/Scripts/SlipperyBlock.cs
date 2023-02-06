using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyBlock : MovableBlock
{
    public bool isSliding = false;
    private Vector3 slidingDirection;
    private SlipperyBlock slipperyCollision;
    
    protected override void Start()
    {
        base.Start();
        blockType = BlockType.SlipperyBlock;
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        //Check if the next tile allows movement. If not, stop sliding.
        if (isSliding && !isAnimating && !Physics2D.OverlapCircle(transform.position - slidingDirection* GameManager.Instance.levelScale, .2f, whatAllowsMovement))
        {
            isSliding = false;
            Debug.Log("Hit Wall");
        }
        
        //Check if the next tile has any movable blocks. If so, move them and stop sliding.
        if (isSliding 
            && !isAnimating 
            && IsSpriteOnGoal(0.1f)
            && Physics2D.OverlapCircle(transform.position - slidingDirection* GameManager.Instance.levelScale, .2f, whatStopsMovement))
        {
            AttemptMovingObjects(whatStopsMovement);
            isSliding = false;
            Debug.Log("Hit Movable Block");
        }

        //Move the block if it is sliding.
        if (isSliding && !isAnimating && IsSpriteOnGoal(0.1f))
        {
            MoveBlock();
        }
    }
    
    private bool AttemptMovingObjects(LayerMask layerMask)
    {
        
        var collisions = Physics2D.OverlapCircleAll(
                transform.position - slidingDirection * GameManager.Instance.levelScale, .2f, layerMask);
        
        if (collisions.Length == 0) return false;

        foreach (var collision in collisions)
        {
            if (!collision.gameObject.CompareTag("MovableBlock")) continue;
            if (!collision.gameObject.GetComponent<MovableBlock>().AttemptToMove(this.gameObject))
            {
                return false;
            }
        }

        foreach (var collision in collisions)
        {
            if (!collision.gameObject.CompareTag("MovableBlock")) continue;
            collision.gameObject.GetComponent<MovableBlock>().MoveBlock();
        }
        return true;
    }
    
    public override void MoveBlock()
    {
        slidingDirection = collisionDirection;
        if(Physics2D.OverlapCircle(transform.position - slidingDirection* GameManager.Instance.levelScale, .2f, whatAllowsMovement)
           && !Physics2D.OverlapCircle(transform.position - slidingDirection* GameManager.Instance.levelScale, .2f, whatStopsMovement)) isSliding = true;
        base.MoveBlock();

        
    }
}
