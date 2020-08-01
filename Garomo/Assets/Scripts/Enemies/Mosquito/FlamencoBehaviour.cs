using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamencoBehaviour : MonoBehaviour
{
    public int life;

    public Vector3 direction;

    public float speed;

    float changeDirTimer = 0;

    public float shotTimerMax = 2f;
    public float shotTimer = 2f;
    public GameObject shot;

    public float deadBodySpeed;

    Rigidbody2D rig;
    Animator anim;
    SpriteRenderer spr;
    BoxCollider2D col;

    Vector3 startPos;

    public Sprite deadBody;

    public bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    public void Restart()
    {
        anim.enabled = true;
        dead = false;
        life = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            shotTimer -= Time.deltaTime;
            changeDirTimer += Time.deltaTime;

            rig.velocity = new Vector3(direction.x * speed * Time.deltaTime, 0, 0);

            if (changeDirTimer > 5)
            {
                changeDirTimer = 0;
                direction = -direction;
                transform.localScale = new Vector3(-direction.x, 1, 1);
            }

            if (shotTimer <= 0)
            {
                shotTimer = shotTimerMax;
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            rig.velocity += Vector2.down * deadBodySpeed * Time.deltaTime;
        }
    }

    void Shot()
    {
        GameObject go = Instantiate(shot);
        go.transform.position = transform.position;
        go.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -speed * 3/2 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Attack")
        {
            life -= 1;
            if((collision.gameObject.transform.position.x < transform.position.x && transform.localScale.x < 0f) || 
                (collision.gameObject.transform.position.x > transform.position.x && transform.localScale.x > 0f))
            {
                transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            }
            anim.SetTrigger("Damage");
            rig.velocity = Vector3.zero;
            if(life <= 0f)
            {
                spr.sprite = deadBody;
                GameManager.Instance.PlaySound("Flamenco_Death");
                col.isTrigger = true;
                anim.enabled = false;
                dead = true;
            }
        }
        else if (collision.transform.tag == "LimitTrigger")
        {
            gameObject.SetActive(false);
            Restart();
        }
    }

    public void PlaySpitSound()
    {
        //GameManager.Instance.PlaySound("Flamenco_Spit_Egg");
    }
}
