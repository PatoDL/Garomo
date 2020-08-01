using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    int livesToPass;

    public LevelData actualLevel;

    public bool soundOn;
    public bool musicOn;

    string actualMusic;

    public void Start()
    {
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();
        GaromoController.GoToNext = NextLevel;
        soundOn = true;
        musicOn = true;
        actualMusic = "Music_Start";
        AkSoundEngine.PostEvent("Music_Start", gameObject);
        SceneManager.sceneLoaded += OnSceneChange;
    }

    void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        actualLevel = GameObject.Find("LevelData").GetComponent<LevelData>();

        GameObject.Find("Garomo").GetComponent<GaromoController>().life = livesToPass;

        UIController.Instance.garomoController = GameObject.Find("Garomo").GetComponent<GaromoController>();

        ChangeMusic("Lvl_" + (actualLevel.actualLevel+1));
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        if(!musicOn)
            AkSoundEngine.StopAll();
        else
            AkSoundEngine.PostEvent("Music_Start", gameObject);
    }

    public void ToggleSound()
    {
        soundOn = !soundOn;
    }

    public void NextLevel(int lives)
    {
        //AkSoundEngine.StopAll();
        livesToPass = lives;
        SceneManager.LoadScene(actualLevel.nextLevel);   
    }

    public void PlaySound(string sound)
    {
        if(soundOn)
            AkSoundEngine.PostEvent(sound, gameObject);
    }

    public void ChangeMusic(string newMusic)
    {
        if (newMusic == "Lvl_4")
            newMusic = "Lvl_3";

        if (musicOn)
        {
            AkSoundEngine.PostEvent(newMusic, gameObject);
        }
        actualMusic = newMusic;
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
