using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour
{
    public GameObject image;
    public Texture garomoCheck;

    Material mat;

    private void Start()
    {
        mat = image.GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Garomo")
        {
            if(mat.mainTexture!= garomoCheck)
                mat.mainTexture = garomoCheck;
        }
    }
}
