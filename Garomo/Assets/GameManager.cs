using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public static void ResumeTime()
    {
        Time.timeScale = 1f;
    }
}
