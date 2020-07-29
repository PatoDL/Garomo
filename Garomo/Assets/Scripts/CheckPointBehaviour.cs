using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointBehaviour : MonoBehaviour
{
    public GameObject image;
    public Texture garomoCheck;
    Texture badGuyCheck;
    Material mat;

    public GameObject SpotLight;
    public GameObject checkLight;

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
                SpotLight.SetActive(true);
                checkLight.SetActive(true);
                ReachCheckPoint(this.gameObject);
                //GaromoController.AdjustTowerRotation("checkpoint");
            }
        }
    }

    public void RestartSprite()
    {
        mat.mainTexture = badGuyCheck;
        SpotLight.SetActive(false);
        checkLight.SetActive(false);
    }
}
