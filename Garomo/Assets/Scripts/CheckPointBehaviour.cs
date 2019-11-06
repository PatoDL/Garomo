using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour
{
    public GameObject image;
    public Texture garomoCheck;
    Texture badGuyCheck;
    Material mat;

    private void Start()
    {
        mat = image.GetComponent<MeshRenderer>().material;
        GaromoController.GaromoWin += RestartSprite;
    }

    private void OnDestroy()
    {
        GaromoController.GaromoWin -= RestartSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Garomo")
        {
            if (mat.mainTexture != garomoCheck)
            {
                badGuyCheck = mat.mainTexture;
                mat.mainTexture = garomoCheck;
            }
        }
    }

    void RestartSprite()
    {
        mat.mainTexture = badGuyCheck;
    }
}
