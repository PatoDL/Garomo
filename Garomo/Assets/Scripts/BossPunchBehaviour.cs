using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPunchBehaviour : MonoBehaviour
{
    public delegate void OnEnemyAction();
    public static OnEnemyAction HitEnemy;

    public float punchableTimer;
    public float punchableTimerMax;
    public bool punchable = false;

    private void OnEnable()
    {
        punchable = false;
        transform.tag = "Enemy";
        gameObject.layer = LayerMask.NameToLayer("CollisionLayer");
        punchableTimer = punchableTimerMax;
    }

    private void Update()
    {
        if(!punchable)
        {
            punchableTimer -= Time.deltaTime;
            if(punchableTimer<=0.0f)
            {
                SwitchModeToPunchable();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (punchable && collision.tag == "Attack")
        {
            if (HitEnemy != null)
                HitEnemy();
        }
        else if(collision.tag == "Garomo")
        {
            if (collision.GetComponent<GaromoController>().immunityPowerUp)
            {
                SwitchModeToPunchable();
                Vector3 newGPos = collision.transform.position;
                newGPos.x += collision.transform.localScale.x * GetComponent<Collider2D>().bounds.extents.x*1.5f;
                collision.transform.position = newGPos;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Garomo" && !punchable)
        {
            SwitchModeToPunchable();
        }
    }

    void SwitchModeToPunchable()
    {
        punchable = true;
        transform.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
