using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMaker : MonoBehaviour
{
    public Transform parent;
    public GameObject colliderPF;

    public float yOffset = 5.0f;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        GameObject newCollider2d = Instantiate(colliderPF, parent);
        Vector3 pos = other.ClosestPoint(transform.position);
        pos.y = pos.y - yOffset;
        pos.z = 0.0f;
        newCollider2d.transform.position = pos;
        newCollider2d.tag = "TowerPlatform";
        newCollider2d.name = "Platform - " + other.gameObject.name;
        newCollider2d.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        for(int i = 0;i<parent.childCount;i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
