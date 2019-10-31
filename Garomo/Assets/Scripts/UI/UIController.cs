using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    int touches = 0;
    public GameObject cheatsPanel;
    public Text versionText;

    public Image[] muzzleParts;
    public Sprite muzzlePiece;
    public Sprite lastMuzzle;

    public GaromoController garomoController;

    public GameObject retryButton;

    // Start is called before the first frame update
    void Start()
    {
        versionText.text = "v" + Application.version;
        GaromoController.GaromoDie = ShowRetryButton;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
        {
            cheatsPanel.gameObject.SetActive(true);
        }

        for(int i=0;i<garomoController.life;i++)
        {
            if(i== garomoController.life - 1)
            {
                muzzleParts[i].sprite = lastMuzzle;
            }
            else
            {
                muzzleParts[i].sprite = muzzlePiece;
            }
        }

        for(int i=garomoController.life;i<5;i++)
        {
            muzzleParts[i].gameObject.SetActive(false);
        }
    }

    void ShowRetryButton()
    {
        retryButton.SetActive(true);
    }

    public void Restart()
    {
        garomoController.Restart();
        SceneManager.LoadScene("Level1");
        garomoController.Restart();
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
