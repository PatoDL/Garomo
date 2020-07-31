using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBehaviour : MonoBehaviour
{
    public GameObject Base;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Garomo")
        {
            Base.SetActive(false);
        }
    }
}
