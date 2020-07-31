using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignBehaviour : MonoBehaviour
{
    public delegate void OnFightStart();
    public OnFightStart StartFight;

    public void Fight()
    {
        StartFight();
        gameObject.SetActive(false);
    }
}
