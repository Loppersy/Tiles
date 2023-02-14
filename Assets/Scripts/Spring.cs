using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    private Player jumper;
    private MovableBlock block;
    private Vector3 SpherePos;
    [SerializeField] private float distance;
    [SerializeField] private GameObject VFX;
    private bool isJumping = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SphereScript>() != null 
            && !collision.gameObject.GetComponent<SphereScript>().OgPlayer.GetComponent<Player>().IsCosmetic())
        {
            jumper = collision.gameObject.GetComponent<SphereScript>().OgPlayer.GetComponent<Player>();
            jumper.IsJumping = true;
            jumper.IsMovementLocked = true;
            SpherePos = jumper.spriteTransform.position;
            if (!isJumping) { StartCoroutine(jump(0.25f, distance, jumper)); }
        }

        if (collision.gameObject.GetComponent<MovableBlockSprite>() != null)
        {
            block = collision.gameObject.GetComponent<MovableBlockSprite>().BlockAttachedTo.GetComponent<MovableBlock>();
            if (block.GetComponent<SlipperyBlock>() != null)
            {
                block.GetComponent<SlipperyBlock>().isSliding = false;
            }
            block.IsMovable = false;
            block.IsAnimating = true;
            SpherePos = block.spriteImage.position;
            if (!isJumping) { StartCoroutine(blockJump(0.25f, distance, block)); }
        }
    }
    IEnumerator jump(float seconds, float distance, Player jumper)
    {
        isJumping = true;
        yield return new WaitForSecondsRealtime(seconds);
        VFX.GetComponent<Animator>().SetTrigger("Expand");
        Vector3 collisionDirection = new Vector3(jumper.transform.position.x - SpherePos.x, jumper.transform.position.y - SpherePos.y).normalized;
        jumper.animator.SetTrigger("Jump");
        jumper.transform.position += new Vector3((float)(collisionDirection.x * distance * GameManager.Instance.levelScale), (float)(collisionDirection.y * distance* GameManager.Instance.levelScale));
        jumper.IsMovementLocked = false;
        isJumping = false;
    }

    IEnumerator blockJump(float seconds, float distance, MovableBlock jumper)
    {
        isJumping = true;
        yield return new WaitForSecondsRealtime(seconds);
        VFX.GetComponent<Animator>().SetTrigger("Expand");
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
