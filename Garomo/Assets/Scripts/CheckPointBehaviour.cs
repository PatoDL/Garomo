using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour
{
    public GameObject image;
    public Texture garomoCheck;
    Texture badGuyCheck;
    Material mat;

    public GameObject decoration;
    public GameObject checkLight;
    Color decoOrigColor;

    public delegate void OnCheckPointReach(GameObject cp);
    public static OnCheckPointReach ReachCheckPoint;

    private void Start()
    {
        mat = image.GetComponent<MeshRenderer>().material;
        GaromoController.GaromoWin += RestartSprite;
        decoOrigColor = decoration.GetComponent<MeshRenderer>().material.color;
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
                decoration.GetComponent<MeshRenderer>().material.color = Color.green;
                decoration.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.green);
                checkLight.SetActive(true);
                ReachCheckPoint(this.gameObject);
                GaromoController.AdjustTowerRotation("checkpoint");
            }
        }
    }

    public void RestartSprite()
    {
        mat.mainTexture = badGuyCheck;
        decoration.GetComponent<MeshRenderer>().material.color = decoOrigColor;
        decoration.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", decoOrigColor);
        checkLight.SetActive(false);
    }
}
