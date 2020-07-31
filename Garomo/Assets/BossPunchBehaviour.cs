using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPunchBehaviour : MonoBehaviour
{
    public delegate void OnEnemyBeingHit();
    public static OnEnemyBeingHit HitEnemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Garomo" && transform.tag == "Enemy")
        {
            transform.tag = "Untagged";
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else if (collision.tag == "Attack" && transform.tag == "Untagged")
        {
            if (HitEnemy != null)
                HitEnemy();
        }
    }
}
