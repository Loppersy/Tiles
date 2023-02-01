using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyBlock : MovableBlock
{
    private bool isSliding = false;
    private Vector3 slidingDirection;
    private SlipperyBlock slipperyCollision;
    private bool hitSlipperyBlock = false;
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player" && !isAnimating)
        {
            isSliding = true;
            slidingDirection = collisionDirection;
        }

    }

    protected override void FixedUpdate()
    {


        base.FixedUpdate();

        var collisions = Physics2D.OverlapCircleAll(transform.position - slidingDirection, .2f, whatStopsMovement);
        if (isSliding && !isAnimating )
        {
            for (int i = 0; i < collisions.Length; i++)
            {

                if (collisions[i].gameObject.tag == "SlipperyBlock")
                {
                    slipperyCollision = Physics2D.OverlapCircle(transform.position - slidingDirection, .2f, whatStopsMovement).gameObject.GetComponent<SlipperyBlock>();
                    gameObject.layer = 6;
                    isSliding = false;
                    hitSlipperyBlock = true;
                }

            }
        }

        if (Physics2D.OverlapCircle(transform.position - slidingDirection, .2f, whatStopsMovement) && !isAnimating)
        {
            isSliding = false;
        }

        if (isSliding && Vector3.Distance(transform.position, sphereSprite.position) == 0f && !isAnimating) 
        {
            transform.position -= slidingDirection;

        }

        if (hitSlipperyBlock && Vector3.Distance(transform.position, sphereSprite.position) == 0f)
        {
            slipperyCollision.slidingDirection = slidingDirection;
            slipperyCollision.isSliding = true;
            hitSlipperyBlock = false;
        }
    }
}
