using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    public Animator animator;

    GaromoController Garomo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Garomo")
        {
            Garomo = other.GetComponent<GaromoController>();
            animator.SetTrigger("Garomo");
            Garomo.gravityAct = false;
            Garomo.transform.parent = transform;
        }
    }

    public void OnGaromoEnter()
    {
        GaromoController.GoToNext(Garomo.life);
    }
}
