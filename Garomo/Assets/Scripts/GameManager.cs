using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    int livesToPass;

    LevelData actualLevel;

    public void Start()
    {
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();
        GaromoController.GoToNext = NextLevel;
    }


    public void NextLevel(int lives)
    {
        SceneManager.LoadScene(actualLevel.nextLevel);
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();

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
