using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rig;

    public float velX;
    public float velY;

    bool lookingRight;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lookingRight = true;
    }

    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if(hor>0)
        {
            if(!lookingRight)
            {
                lookingRight = true;
                spriteRenderer.transform.localScale += new Vector3(2, 0, 0);
            }
            animator.SetBool("RunningR", true);
        }
        else if (hor<0)
        {
            if(lookingRight)
            {
                lookingRight = false;
                spriteRenderer.transform.localScale -= new Vector3(2, 0, 0);
            }
            animator.SetBool("RunningR", true);
        }
        else
        {
            animator.SetBool("RunningR", false);
        }
    }

    void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if(hor!=0)
            rig.velocity = new Vector2(hor * velX * Time.fixedDeltaTime, rig.velocity.y);
        if(ver!=0)
            rig.velocity = new Vector2(rig.velocity.x, ver * velY * Time.fixedDeltaTime);
    }
}
