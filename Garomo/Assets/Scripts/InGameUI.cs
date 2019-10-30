using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject lifeBar;
    public PlayerController playerController;

    public float originalXScale;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lifeBar.transform.localScale = new Vector3(originalXScale *playerController.GetLife()/10, lifeBar.transform.localScale.y, lifeBar.transform.localScale.z);
    }
}
