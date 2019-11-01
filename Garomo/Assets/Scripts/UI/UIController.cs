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

    public Image[] fadingPanels;

    public Image actualFading;

    public GameObject instructionsPanel;

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
        actualFading = fadingPanels[0];
    }

    float fadingTimeMax = 1.5f;
    float fadingTime = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (!fadingPanels[0].gameObject.activeInHierarchy && !fadingPanels[1].gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(true);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                instructionsPanel.SetActive(!instructionsPanel.activeInHierarchy);
                if (instructionsPanel.activeInHierarchy)
                    Time.timeScale = 0f;
                else
                    Time.timeScale = 1f;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                cheatsPanel.gameObject.SetActive(true);
            }

            for (int i = 0; i < garomoController.life; i++)
            {
                muzzleParts[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < garomoController.life; i++)
            {
                if (i == garomoController.life - 1)
                {
                    muzzleParts[i].sprite = lastMuzzle;
                }
                else
                {
                    if (muzzleParts[i].IsActive())
                        muzzleParts[i].sprite = muzzlePiece;
                }
            }

            for (int i = garomoController.life; i < 5; i++)
            {
                muzzleParts[i].gameObject.SetActive(false);
            }
        }
        else
        {
            if (fadingTime > 0)
            {
                Color color = new Color(0, 0, 0, fadingTime / fadingTimeMax);
                actualFading.color = color;
                fadingTime -= Time.deltaTime;
                if(actualFading.transform.childCount > 0)
                {
                    Image childIm = actualFading.transform.Find("Logo").GetComponent<Image>();
                    Color c = new Color(1, 1, 1, fadingTime / fadingTimeMax);
                    childIm.color = c;

                    
                }
                if (fadingTime < 0)
                {
                    fadingTime = 0;
                    actualFading.gameObject.SetActive(false);
                }
            }
            else if(actualFading == fadingPanels[0])
            {
                actualFading = fadingPanels[1];
                fadingTime = 5f;
            }
        }
    }

    public void ShowGameOver()
    {
        GameOverPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        MenuPanel.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        instructionsPanel.SetActive(true);
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
        TurtleController.RestartTurtles();
        FlamencoBehaviour.RestartMosquitos();
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
            if (!PausePanel.gameObject.activeInHierarchy)
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
