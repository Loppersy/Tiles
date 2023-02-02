using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Breakable : MonoBehaviour
{
    protected SpriteRenderer rend;
    protected Transform _transform;
    protected Animator animator;
    protected bool broken;
    protected bool touched;

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
    public Sprite noneSprite;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        _transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        Color current_color = GetBackgroundColor(GameManager.Instance.currentBackground);
        rend.color = current_color;

    }

    protected virtual Color GetBackgroundColor(int backgroundNumber)
    {
        Color current_color = Color.white;
        switch (backgroundNumber)
        {
            case 0:
                current_color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
                break;
            case 1:
                current_color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
                break;
            case 2:
                current_color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
                break;
            case 3:
                current_color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
                break;
        }
        return current_color;
        
    }
    

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (touched && collision.gameObject.name == "Player" 
                    && collision.gameObject.GetComponent<Player>() != null
                    && !collision.gameObject.GetComponent<Player>().IsCosmetic())
        {
            gameObject.layer = 6;
            broken = true;
            tag = "IsBroken";
            animator.SetBool("Despawn", true);
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player"
           && collision.gameObject.GetComponent<Player>() != null
           && !collision.gameObject.GetComponent<Player>().IsCosmetic()){
            touched = true;
        }

    }

    public bool Broken
    {
        set { broken = value; }
        get { return broken; }
    }

    void Update()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(1f, 0f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                right = true;
                break;
            }
            right = false;
        }
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(-1f, 0f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                left = true;
                break;
            }
            left = false;
        }
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(0f, 1f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                top = true;
                break;
            }
            top = false;
        }
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position + new Vector3(0f, -1f, 0f), .2f))
        {
            if (collider.tag == "IsBreakable")
            {
                bottom = true;
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

        }
        else if (!left && !right && !top && !bottom)
        {
            rend.sprite = noneSprite;
        }
    }
    }
