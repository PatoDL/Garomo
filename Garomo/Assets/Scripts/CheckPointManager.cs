using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public GameObject firstCheckPoint;

    public GameObject lastCheckPoint;

    public static CheckPointManager instance;

    public GaromoController garomo;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        CheckPointBehaviour.ReachCheckPoint = UpdateLastCheckPoint;
        firstCheckPoint = new GameObject("FirstPos");
        firstCheckPoint.transform.position = garomo.transform.position;
        lastCheckPoint = firstCheckPoint;
    }

    void UpdateLastCheckPoint(GameObject checkPoint)
    {
        lastCheckPoint = checkPoint;
        AkSoundEngine.PostEvent("Checkpoint", gameObject);
    }

    public GameObject GetLastCheckPoint()
    {
        return lastCheckPoint;
    }

    public void RestartLevel()
    {
        lastCheckPoint = firstCheckPoint;
    }
}
