using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    int livesToPass;

    LevelData actualLevel;

    public bool soundOn;
    public bool musicOn;

    public void Start()
    {
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();
        GaromoController.GoToNext = NextLevel;
        soundOn = true;
        musicOn = true;
        AkSoundEngine.PostEvent("Music_start", gameObject);
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        if(!musicOn)
            AkSoundEngine.StopAll();
        else
            AkSoundEngine.PostEvent("Music_start", gameObject);
    }

    public void ToggleSound()
    {
        soundOn = !soundOn;
    }

    public void NextLevel(int lives)
    {
        //AkSoundEngine.StopAll();
        SceneManager.LoadScene(actualLevel.nextLevel);
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();

        GameObject.Find("Garomo").GetComponent<GaromoController>().life = lives;

        
    }

    public void GoToLevel(int level)
    {
        if(actualLevel.actualLevel != level)
            SceneManager.LoadScene(level);
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
