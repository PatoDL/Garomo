using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Garomo")
        {
            gameObject.SetActive(false);
        }
    }
}
