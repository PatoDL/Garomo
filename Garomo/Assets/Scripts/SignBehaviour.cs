using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignBehaviour : MonoBehaviour
{
    public delegate void OnFightStart();
    public OnFightStart StartFight;

    public GameObject garomo;

    private void OnEnable()
    {
        garomo.SetActive(false);
    }

    public void Fight()
    {
        StartFight();
        gameObject.SetActive(false);
    }
}
