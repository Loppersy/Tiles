using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private Player jumper;
    private MovableBlock block;
    private Vector3 SpherePos;
    [SerializeField] private float distance;
    private bool isJumping = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.name == "Player"
                                                  && collision.gameObject.GetComponent<Player>() != null
                                                  && !collision.gameObject.GetComponent<Player>().IsCosmetic())
        {
            jumper = collision.gameObject.GetComponent<Player>();
            jumper.IsJumping = true;
            jumper.IsMovementLocked = true;
            SpherePos = jumper.spriteTransform.position;
            if (!isJumping) { StartCoroutine(jump(.75f, distance, jumper)); }
        }
        else if (collision.transform.parent.gameObject.name == "MovingBlocks")
        {
            block = collision.gameObject.GetComponent<MovableBlock>();
            if (block.GetComponent<SlipperyBlock>() != null)
            {
                block.GetComponent<SlipperyBlock>().isSliding = false;
            }
            block.IsMovable = false;
            block.IsAnimating = true;
            SpherePos = block.spriteImage.position;
            if (!isJumping) { StartCoroutine(blockJump(0f, distance, block)); }
        }
    }
    IEnumerator jump(float seconds, float distance, Player jumper)
    {
        isJumping = true;
        yield return new WaitForSecondsRealtime(seconds);
        Vector3 collisionDirection = new Vector3(jumper.transform.position.x - SpherePos.x, jumper.transform.position.y - SpherePos.y).normalized;
        jumper.transform.position += new Vector3((float)(collisionDirection.x * distance * GameManager.Instance.levelScale), (float)(collisionDirection.y * distance* GameManager.Instance.levelScale));
        jumper.IsMovementLocked = false;
        isJumping = false;
    }

    IEnumerator blockJump(float seconds, float distance, MovableBlock jumper)
    {
        isJumping = true;
        yield return new WaitForSecondsRealtime(seconds);
        Vector3 collisionDirection = new Vector3(jumper.transform.position.x - SpherePos.x, jumper.transform.position.y - SpherePos.y).normalized;
        jumper.transform.position += new Vector3((float)(collisionDirection.x * distance* GameManager.Instance.levelScale), (float)(collisionDirection.y * distance* GameManager.Instance.levelScale));
        jumper.IsMovable = true;
        isJumping = false;
        if(jumper.GetComponent<SlipperyBlock>() != null)
        {
            jumper.GetComponent<SlipperyBlock>().isSliding = true;
        }
    }
}
