using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player"
            && collision.gameObject.GetComponent<Player>() != null
            && !collision.gameObject.GetComponent<Player>().IsCosmetic()) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<Door>().IsOpened = true;
            }

            GetComponent<SpriteRenderer>().forceRenderingOff = true;
        }
    }
}
