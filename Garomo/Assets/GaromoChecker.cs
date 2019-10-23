using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaromoChecker : MonoBehaviour
{
    private TurtleController tc;

    private void Start()
    {
        tc = GetComponentInParent<TurtleController>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Garomo")
        {
            tc.haveToAttack = true;
            Debug.Log("entro");
        }
    }
}
