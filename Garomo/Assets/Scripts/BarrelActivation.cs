using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelActivation : MonoBehaviour
{
    public GameObject barrel;
    Rigidbody2D barrelRig;

    Vector3 barrelStartPos;

    public float barrelMaxReturnTime;
    public float barrelReturnTimer;
    public bool barrelActivated = false;

    private void Start()
    {
        barrelRig = barrel.GetComponent<Rigidbody2D>();

        barrelStartPos = barrel.transform.position;

        barrelReturnTimer = barrelMaxReturnTime;
    }

    private void Update()
    {
        if(barrelActivated)
        {
            barrelReturnTimer -= Time.deltaTime;
            if(barrelReturnTimer<=0f)
            {
                barrelReturnTimer = barrelMaxReturnTime;
                barrelActivated = false;
                RestartBarrel();
            }
        }
    }

    public void RestartBarrel()
    {
        barrel.transform.position = barrelStartPos;
        barrelRig.velocity = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Garomo" && !barrelActivated)
        {
            barrelRig.velocity = new Vector2(-100f, 0f) * Time.deltaTime;
            Debug.Log("entro");
            barrelActivated = true;
        }
    }
}
