using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamencoBehaviour : MonoBehaviour
{
    public int life;

    public Vector3 direction;

    public float speed;

    float changeDirTimer = 0;

    float shotTimer=0;

    public GameObject shot;

    Rigidbody2D rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        shotTimer += Time.deltaTime;
        changeDirTimer += Time.deltaTime;

        rig.velocity = new Vector3(direction.x * speed * Time.deltaTime, 0, 0);

        if (changeDirTimer > 5)
        {
            changeDirTimer = 0;
            direction = -direction;
        }

        if(shotTimer>2)
        {
            shotTimer = 0;
            GameObject go = Instantiate(shot);
            go.transform.position = transform.position;
            go.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 100 * Time.deltaTime, 0);
        }
    }
}
