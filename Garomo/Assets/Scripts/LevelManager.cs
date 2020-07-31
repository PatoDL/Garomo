using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject garomo;
    public BossBehaviour boss;

    // Start is called before the first frame update
    void Start()
    {
        boss.sign.StartFight += StartGaromo;
    }

    private void OnDestroy()
    {
        boss.sign.StartFight -= StartGaromo;
    }

    void StartGaromo()
    {
        garomo.SetActive(true);
    }
}
