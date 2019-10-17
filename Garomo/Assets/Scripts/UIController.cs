using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    int touches = 0;
    public GameObject cheatsPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
        {
            cheatsPanel.gameObject.SetActive(true);
        }
    }

    public void Touch()
    {
        touches++;
        if(touches==3)
        {
            cheatsPanel.gameObject.SetActive(true);
            touches = 0;
        }
    }
}
