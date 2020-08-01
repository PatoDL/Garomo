using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public Transform Garomo;

    public float rotationSpeed;

    private Vector3 checkPointRotation;
    Vector3 electricRingInitialPos;
    public GameObject electricRing;
    public float electricRingVelocityY;
    public GameObject RingCollider;
    private void Start()
    {
        checkPointRotation = transform.eulerAngles;
        GaromoController.AdjustTowerRotation = AdjustTowerRotation;
        electricRingInitialPos = electricRing.transform.position;
    }

    private void OnDestroy()
    {
        GaromoController.AdjustTowerRotation -= AdjustTowerRotation;
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

        electricRing.transform.position += new Vector3(0f, electricRingVelocityY * Time.deltaTime, 0f);
        RingCollider.transform.rotation = Quaternion.identity;
        Vector3 ringcolpos = RingCollider.transform.position;
        ringcolpos.x = transform.position.x;
        ringcolpos.z = 0f;
        RingCollider.transform.position = ringcolpos;
    }

    void AdjustTowerRotation()
    {
        transform.position = new Vector3(Garomo.position.x, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.identity;
        electricRing.transform.position = electricRingInitialPos;
    }
}
