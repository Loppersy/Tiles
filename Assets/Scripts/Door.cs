using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isOpened = false;

    public bool IsOpened
    {
        get { return isOpened; }
        set { isOpened = value; }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpened)
        {
            gameObject.layer = 7;
            GetComponent<SpriteRenderer>().forceRenderingOff = true;
        } else if (!isOpened)
        {
            gameObject.layer = 6;
            GetComponent<SpriteRenderer>().forceRenderingOff = false;
        }
    }
}
