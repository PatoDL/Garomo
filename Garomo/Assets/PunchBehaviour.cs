using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : MonoBehaviour
{
    bool collided;
    public bool air = false;
    private void OnEnable()
    {
        collided = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collided = true;
        }
    }

    private void OnDisable()
    {
        if (air)
        {
            if (collided)
            {
                GameManager.Instance.PlaySound("Garomo_Kick_Hit");
            }
            else
            {
                GameManager.Instance.PlaySound("Garomo_Kick");
            }
        }
        else
        {
            if (collided)
            {
                GameManager.Instance.PlaySound("Garomo_Punch_Hit");
            }
            else
            {
                GameManager.Instance.PlaySound("Garomo_Punch");
            }
        }
    }
}
