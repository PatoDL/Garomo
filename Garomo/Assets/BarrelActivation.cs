using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelActivation : MonoBehaviour
{
    public GameObject barrel;
    Rigidbody2D barrelRig;

    private void Start()
    {
        barrelRig = barrel.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Garomo")
        {
            barrelRig.velocity = new Vector2(-100f, 0f) * Time.deltaTime;
            Debug.Log("entro");
            gameObject.SetActive(false);
        }
    }
}
