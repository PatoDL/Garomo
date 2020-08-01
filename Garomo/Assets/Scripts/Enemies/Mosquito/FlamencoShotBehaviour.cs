using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamencoShotBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "LimitTrigger")
        {
            Destroy(this.gameObject);
        }
        if(collision.tag =="Garomo")
        {
            GameManager.Instance.PlaySound("Flamenco_Egg_Hit");
        }
    }
}
