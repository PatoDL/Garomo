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
