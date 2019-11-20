using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int livesToPass;

    LevelData actualLevel;

    public static GameManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this);
    }

    public void Start()
    {
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();
        Debug.Log("leveldata installed " + actualLevel.actualLevel);
        GaromoController.GoToNext = NextLevel;
    }


    public void NextLevel(int lives)
    {
        SceneManager.LoadScene(actualLevel.nextLevel);
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();
        Debug.Log(actualLevel.actualLevel);

        GameObject.Find("Garomo").GetComponent<GaromoController>().life = lives;
    }

    public static void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public static void ResumeTime()
    {
        Time.timeScale = 1f;
    }
}
