using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject levelEnemies;

    Vector3[] turtlesPos;
    Vector3[] mosquitosPos;

    List<GameObject> turtles;

    List<GameObject> mosquitos;

    public static EnemyManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        turtles = new List<GameObject>();
        mosquitos = new List<GameObject>();

        for(int i = 0; i<levelEnemies.transform.childCount; i++)
        {
            if(levelEnemies.transform.GetChild(i).GetComponent<FlamencoBehaviour>())
            {
                mosquitos.Add(levelEnemies.transform.GetChild(i).gameObject);
            }
            else
            {
                turtles.Add(levelEnemies.transform.GetChild(i).gameObject);
            }   
        }

        turtlesPos = new Vector3[turtles.Count];
        mosquitosPos = new Vector3[mosquitos.Count];

        int j = 0;

        foreach (GameObject g in turtles)
        {
            turtlesPos[j] = g.transform.position;
            j++;
        }

        j = 0;

        foreach (GameObject g in mosquitos)
        {
            mosquitosPos[j] = g.transform.position;
            j++;
        }
    }

    public void RestartEnemies()
    {
        int i = 0;

        foreach(GameObject g in turtles)
        {
            g.transform.position = turtlesPos[i];
            g.SetActive(true);
            i++;
        }

        i = 0;

        foreach (GameObject g in mosquitos)
        {
            g.transform.position = mosquitosPos[i];
            g.SetActive(true);
            i++;
        }
    }
}
