using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaromoChecker : MonoBehaviour
{
    public delegate void OnGaromoEnter(GameObject garomo);
    public OnGaromoEnter GaromoEntrance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Garomo")
        {
            GaromoEntrance(other.gameObject);
        }
    }
}
