using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamencoBehaviour : MonoBehaviour
{
    public int life;

    public Vector3 direction;

    public float speed;

    float changeDirTimer = 0;

    private float normalizedHorizontalSpeed = 0;

    public float shotTimerMax = 2f;
    public float shotTimer = 2f;
    public GameObject shot;

    Rigidbody2D rig;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
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

        if(shotTimer <= 0)
        {
            shotTimer = shotTimerMax;
            anim.SetTrigger("Attack");
        }
    }

    void Shot()
    {
        GameObject go = Instantiate(shot);
        go.transform.position = transform.position;
        go.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -speed * 3/2 * Time.deltaTime, 0);
    }
}
