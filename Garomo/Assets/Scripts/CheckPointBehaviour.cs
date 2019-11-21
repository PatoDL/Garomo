using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour
{
    public GameObject image;
    public Texture garomoCheck;
    Texture badGuyCheck;
    Material mat;

    public delegate void OnCheckPointReach(GameObject cp);
    public static OnCheckPointReach ReachCheckPoint;

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
                ReachCheckPoint(this.gameObject);
            }
        }
    }

    void RestartSprite()
    {
        mat.mainTexture = badGuyCheck;
    }
}
