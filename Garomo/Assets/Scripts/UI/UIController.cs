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

    public GameObject MenuPanel;
    public GameObject PausePanel;
    public GameObject CreditsBackground;
    public GameObject CreditsPanel;
    public GameObject WinPanel;
    public GameObject GameOverPanel;
    public GameObject inGameUI;

    // Start is called before the first frame update
    void Start()
    {
        versionText.text = "v" + Application.version;
        GaromoController.GaromoDie = ShowGameOver;
        GaromoController.GaromoWin = OpenCredits;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(true);
        }

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
        {
            cheatsPanel.gameObject.SetActive(true);
        }

        for(int i=0;i<garomoController.life;i++)
        {
            muzzleParts[i].gameObject.SetActive(true);
        }

        for(int i=0;i<garomoController.life;i++)
        {
            if(i == garomoController.life - 1)
            {
                muzzleParts[i].sprite = lastMuzzle;
            }
            else
            {
                if(muzzleParts[i].IsActive())
                    muzzleParts[i].sprite = muzzlePiece;
            }
        }

        for(int i=garomoController.life;i<5;i++)
        {
            muzzleParts[i].gameObject.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        GameOverPanel.gameObject.SetActive(true);
        Restart();
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        MenuPanel.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    public void OpenCredits()
    {
        CreditsBackground.gameObject.SetActive(true);
        if (garomoController.win)
        {
            CreditsPanel.gameObject.SetActive(false);
            WinPanel.gameObject.SetActive(true);
        }
        else
        {
            CreditsPanel.gameObject.SetActive(true);
            WinPanel.gameObject.SetActive(false);
        }
    }

    void ShowRetryButton()
    {
        retryButton.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        garomoController.Restart();
    }

    public void IncreaseButton(GameObject g)
    {
        g.transform.localScale *= 1.2f;
    }

    public void DecreaseButton(GameObject g)
    {
        g.transform.localScale *= 0.8f;
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

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
            PausePanel.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            PausePanel.gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
