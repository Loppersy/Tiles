using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBlock : MonoBehaviour
{
    protected bool isMovable = true;
    protected bool isAnimating = false;
    [SerializeReference] public Transform sphereSprite;
    [SerializeReference] public LayerMask whatAllowsMovement;
    [SerializeReference] public float moveSpeed = 5f;
    [SerializeReference] public Transform player;
    protected Vector3 collisionDirection;

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
    public Transform SphereSprite
    {
        set { sphereSprite = value; }
        get { return sphereSprite; }
    }
    private void Start()
    {
        player = transform.parent.parent.GetChild(1);
        sphereSprite.parent = transform.parent;
    }

    protected virtual void FixedUpdate()
    {
        sphereSprite.position = Vector3.MoveTowards(sphereSprite.position, transform.position, moveSpeed * Time.deltaTime* GameManager.Instance.levelScale);
        if (0.7 *GameManager.Instance.levelScale < Vector3.Distance(transform.position, player.position) && Vector3.Distance(transform.position, player.position) < GameManager.Instance.levelScale *1.1f)
        {
            collisionDirection =
                new Vector3(player.position.x - transform.position.x, player.position.y - transform.position.y)
                    .normalized;
            Debug.Log(Physics2D.OverlapCircle(transform.position - collisionDirection * GameManager.Instance.levelScale, .2f,
                whatAllowsMovement));

            gameObject.layer = Physics2D.OverlapCircle(transform.position - collisionDirection * GameManager.Instance.levelScale, .2f,
                whatAllowsMovement) ? 7 : 6;
        }
        if (Vector3.Distance(transform.position, sphereSprite.position) == 0f && isMovable)
        {
            isAnimating = false;
        }


    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Player" || isAnimating ||
            collision.gameObject.GetComponent<Player>().IsJumping ||
            Vector3.Distance(transform.position, sphereSprite.position) != 0f) return;

        // Move block objective to next tile
        transform.position -= (collisionDirection * GameManager.Instance.levelScale);
    }
}
