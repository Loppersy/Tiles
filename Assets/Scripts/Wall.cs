using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private bool top = false;
    private bool bottom = false;
    private bool left = false;
    private bool right = false;

    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite topSprite;
    public Sprite bottomSprite;
    public Sprite lefttopSprite;
    public Sprite leftbottomSprite;
    public Sprite righttopSprite;
    public Sprite rightbottomSprite;
    public Sprite leftrightSprite;
    public Sprite topbottomSprite;
    public Sprite allLeftSprite;
    public Sprite allrightSprite;
    public Sprite alltopSprite;
    public Sprite allbottomSprite;
    public Sprite allSprite;

    private SpriteMask rend;
    void Update()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(1f, 0f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                right = true;
                rend.forceRenderingOff = false;
                break;
            }
            right = false;
        }
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(-1f, 0f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                left = true;
                rend.forceRenderingOff = false;
                break;
            }
            left = false;
        }
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(0f, 1f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                top = true;
                rend.forceRenderingOff = false;
                break;
            }
            top = false;
        }
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(0f, -1f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                bottom = true;
                rend.forceRenderingOff = false;
                break;
            }
            bottom = false;
        }

        if (left && right && top && bottom)
        {
            rend.sprite = allSprite;
        }
        else if (!left && right && top && bottom)
        {
            rend.sprite = allLeftSprite;
        }
        else if (left && !right && top && bottom)
        {
            rend.sprite = allrightSprite;
        }
        else if (left && right && !top && bottom)
        {
            rend.sprite = alltopSprite;
        }
        else if (left && right && top && !bottom)
        {
            rend.sprite = allbottomSprite;
        }
        else if (!left && !right && top && bottom)
        {
            rend.sprite = topbottomSprite;
        }
        else if (left && !right && !top && bottom)
        {
            rend.sprite = leftbottomSprite;
        }
        else if (left && right && !top && !bottom)
        {
            rend.sprite = leftrightSprite;
        }
        else if (!left && right && top && !bottom)
        {
            rend.sprite = righttopSprite;
        }
        else if (!left && right && !top && bottom)
        {
            rend.sprite = rightbottomSprite;
        }
        else if (left && !right && top && !bottom)
        {
            rend.sprite = lefttopSprite;
        }
        else if (left && !right && !top && !bottom)
        {
            rend.sprite = leftSprite;
        }
        else if (!left && right && !top && !bottom)
        {
            rend.sprite = rightSprite;
        }
        else if (!left && !right && top && !bottom)
        {
            rend.sprite = topSprite;
        }
        else if (!left && !right && !top && bottom)
        {
            rend.sprite = bottomSprite;

        } else if (!left && !right && !top && !bottom)
        {
            rend.forceRenderingOff = true;
        }

    }

    private void Start()
    {
        rend = GetComponent<SpriteMask>();
    }
}
