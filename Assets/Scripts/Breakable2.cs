using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable2 : Breakable
{
    int passed = 0;
    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if(passed == 0 && touched && collision.gameObject.name == "Player" 
           && collision.gameObject.GetComponent<Player>() != null
           && !collision.gameObject.GetComponent<Player>().IsCosmetic())
        {
            passed++;
            touched = false;
            rend.color = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        } else if (passed == 1 && touched && collision.gameObject.name == "Player" 
                   && collision.gameObject.GetComponent<Player>() != null
                   && !collision.gameObject.GetComponent<Player>().IsCosmetic())
        {
            animator.SetBool("Despawn", true);
            tag = "IsBroken";
            gameObject.layer = 6;
            broken = true;
            touched = false;
        }

    }
    
    protected override Color GetBackgroundColor(int backgroundNumber)
    {
        Color current_color = Color.white;

        return current_color;
        
    }
}
