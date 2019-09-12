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

    public int life = 100;

    public float rayDistance;

    Vector3 rollInitPos;
    Vector3 rollFinalPos;
    public bool rolling;
    public float rollVel;

    public GameObject punchCol;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lookingRight = true;
        jumping = false;
        Physics2D.gravity = new Vector2(0,-40f);
        rollInitPos = transform.position;
        rollFinalPos = transform.position;
    }

    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        //movement and sprite flipping

        if(hor>0)
        {
            if(!lookingRight)
            {
                lookingRight = true;
                Vector3 scale = spriteRenderer.transform.localScale;
                scale.x *= -1;
                spriteRenderer.transform.localScale = scale;
            }
            animator.SetBool("RunningR", true);
        }
        else if (hor<0)
        {
            if(lookingRight)
            {
                lookingRight = false;
                Vector3 scale = spriteRenderer.transform.localScale;
                scale.x *= -1;
                spriteRenderer.transform.localScale = scale;
            }
            animator.SetBool("RunningR", true);
        }
        else
        {
            animator.SetBool("RunningR", false);
        }

        //jumping and obstacle checking

        Vector3 origin = new Vector3(transform.position.x-transform.localScale.x/2,transform.position.y,transform.position.z);
        Vector3 origin2 = new Vector3(transform.position.x + transform.localScale.x / 2, transform.position.y, transform.position.z);

        RaycastHit2D[] raycastHit2D = new RaycastHit2D[2];

        raycastHit2D[0] = Physics2D.Raycast(origin, Vector2.down * rayDistance);
        raycastHit2D[1] = Physics2D.Raycast(origin2, Vector2.down * rayDistance);

        RaycastHit2D r = Physics2D.Raycast(new Vector3(transform.position.x + spriteRenderer.size.x / 2+spriteRenderer.size.x/10, transform.position.y + spriteRenderer.size.y / 2, transform.position.z), transform.right, rayDistance);

        if (r && rolling)
            rolling = false;

        for (int i = 0; i < 2; i++)
        {
            if (raycastHit2D[i])
            {
                if (raycastHit2D[i].transform.tag == "floor" && jumping)
                    jumping = false;
            }
        }

        //punching

        if(Input.GetKeyUp(KeyCode.F))
        {
            GameObject g = Instantiate(punchCol);
            g.transform.position = new Vector3(transform.position.x + spriteRenderer.size.x * 3 / 4, transform.position.y + spriteRenderer.size.y * 3 / 4, transform.position.z);
        }
    }

    void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("Roll");
            rolling = true;

            Vector3 direction;
            if (lookingRight)
                direction = transform.right;
            else
                direction = -transform.right;

            rollInitPos = transform.position;
            rollFinalPos = transform.position + direction * Time.fixedDeltaTime * rollVel;
        }

        if (rolling)
        {
            rig.MovePosition(transform.position+(rollFinalPos-rollInitPos)*1/6);
            if (Vector3.Distance(transform.position, rollFinalPos) < 1f)
                rolling = false;
        }

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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Enemy")
        {
            life -= 10;
            Debug.Log(life);
            if (life <= 0)
            {

            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Goal")
            Debug.Log("You Win");
    }
}
