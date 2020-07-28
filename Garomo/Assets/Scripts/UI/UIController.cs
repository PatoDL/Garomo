using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class UIController : MonoBehaviourSingleton<UIController>
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

    bool firstTime;

    // Start is called before the first frame update
    void Start()
    {
        versionText.text = "v" + Application.version;
        GaromoController.GaromoDie = ShowGameOver;
        GaromoController.GaromoWin += OpenCredits;
        actualFading = fadingPanels[0];
        firstTime = true;
    }

    private void OnDestroy()
    {
        GaromoController.GaromoWin -= OpenCredits;
    }

    float fadingTimeMax = 1.5f;
    float fadingTime = 1.5f;

    bool helpInput = false;
    bool backInput = false;
    bool startInput = false;

    public void OnHelpOpen(InputAction.CallbackContext context)
    {
        helpInput = true;
    }

    public void OnBackInput(InputAction.CallbackContext context)
    {
        backInput = true;
    }

    public void OnStartInput(InputAction.CallbackContext context)
    {
        startInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fadingPanels[0].gameObject.activeInHierarchy && !fadingPanels[1].gameObject.activeInHierarchy)
        {
            if(!MenuPanel.activeInHierarchy && !GameOverPanel.activeInHierarchy &&
                !CreditsPanel.activeInHierarchy && !PausePanel.activeInHierarchy && !WinPanel.activeInHierarchy)
            {
                if (backInput && instructionsPanel.activeInHierarchy)
                {
                    instructionsPanel.SetActive(false);
                    if (firstTime)
                    {
                        Restart(true);
                        firstTime = false;
                    }
                    else
                    {
                        GameManager.ResumeTime();
                    }
                    if (GameManager.Instance.soundOn)
                        AkSoundEngine.PostEvent("Close_Help", gameObject);
                }

                if (helpInput)
                {
                    instructionsPanel.SetActive(!instructionsPanel.activeInHierarchy);
                    if (instructionsPanel.activeInHierarchy)
                    {
                        if (GameManager.Instance.soundOn)
                            AkSoundEngine.PostEvent("Open_Help", gameObject);
                        GameManager.PauseTime();
                    }
                    else
                    {
                        if (GameManager.Instance.soundOn)
                            AkSoundEngine.PostEvent("Close_Help", gameObject);
                        GameManager.ResumeTime();
                    }
                    
                }

                if (startInput)
                {
                    PauseGame(true);
                }

                if (Cursor.visible)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                if(!Cursor.visible)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
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
                    if(actualFading.transform.childCount > 0)
                    {
                        GameManager.PauseTime();
                    }
                }
            }
            else if(actualFading == fadingPanels[0])
            {
                actualFading = fadingPanels[1];
                fadingTime = 5f;
            }
        }

        helpInput = false;
        backInput = false;
        startInput = false;
    }

    public void ShowGameOver()
    {
        GameOverPanel.gameObject.SetActive(true);
        GameManager.PauseTime();
    }

    public void StartGame()
    {
        MenuPanel.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        instructionsPanel.SetActive(true);
        if (GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Open_Help", gameObject);
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

    public void Restart(bool fromTheStart)
    {
        if (fromTheStart)
            CheckPointManager.instance.RestartLevel();
        GameManager.ResumeTime();
        garomoController.Restart();
        EnemyManager.instance.RestartEnemies();
        ItemManager.instance.RestartItems();
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
