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

    public bool jumping;

    public float distance;
    void Start()
    {

        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lookingRight = true;
        jumping = false;
        Physics2D.gravity = new Vector2(0,-40f);
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
                spriteRenderer.flipX = false;
            }
            animator.SetBool("RunningR", true);
        }
        else if (hor<0)
        {
            if(lookingRight)
            {
                lookingRight = false;
                spriteRenderer.flipX = true;
            }
            animator.SetBool("RunningR", true);
        }
        else
        {
            animator.SetBool("RunningR", false);
        }

        Vector3 origin = new Vector3(transform.position.x-transform.localScale.x/2,transform.position.y,transform.position.z);
        Vector3 origin2 = new Vector3(transform.position.x + transform.localScale.x / 2, transform.position.y, transform.position.z);

        RaycastHit2D[] raycastHit2D = new RaycastHit2D[2];

        raycastHit2D[0] = Physics2D.Raycast(origin, Vector2.down * distance);
        raycastHit2D[1] = Physics2D.Raycast(origin2, Vector2.down * distance);

        for (int i = 0; i < 2; i++)
        {
            if (raycastHit2D[i])
            {
                if (raycastHit2D[i].transform.tag == "floor" && jumping)
                    jumping = false;
            }
        }

        Debug.DrawRay(origin, Vector2.down * distance);
    }

    void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if (hor == 0)
        {
            Invoke("ResetVelocity", 0.2f);
        }
        else
        {
            CancelInvoke("ResetVelocity");
            rig.velocity = new Vector2(hor * velX * Time.fixedDeltaTime, rig.velocity.y);
        }
     

        if (!jumping && ver > 0)
        {
            rig.AddForce(new Vector2(0, 20f), ForceMode2D.Impulse);
            jumping = true;
        }
    }

    public void ResetVelocity()
    {
        rig.velocity = new Vector2(0, rig.velocity.y);
    }
}
