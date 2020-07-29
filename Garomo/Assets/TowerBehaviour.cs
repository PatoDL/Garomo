using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public Transform Garomo;

    public float rotationSpeed;

    private Vector3 checkPointRotation;

    private void Start()
    {
        checkPointRotation = transform.eulerAngles;
        GaromoController.AdjustTowerRotation = AdjustTowerRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x != Garomo.position.x)
        {
            float garomoX = Garomo.position.x;
            float towerX = transform.position.x;

            if(garomoX > towerX)
            {
                transform.eulerAngles += new Vector3(0.0f, (garomoX - towerX) + rotationSpeed * Time.deltaTime, 0.0f);
            }
            else
            {
                transform.eulerAngles += new Vector3(0.0f, (garomoX - towerX) - rotationSpeed * Time.deltaTime, 0.0f);
            }
            
            
            transform.position = new Vector3(Garomo.position.x, transform.position.y, transform.position.z);
        }
    }

    void AdjustTowerRotation(string whatHappened)
    {
        transform.position = new Vector3(Garomo.position.x, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.identity;
    }
}
