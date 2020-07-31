using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garomo3D : MonoBehaviour
{
    public GameObject enemyCol;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            GameObject g = Instantiate(enemyCol);
            g.SetActive(true);
            Vector3 pos = other.transform.position;
            pos.z = 0.0f;
            g.transform.position = pos;
        }
    }
}
