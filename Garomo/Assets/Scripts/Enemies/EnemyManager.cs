using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject levelEnemies;

    Vector3[] turtlesPos;
    Vector3[] mosquitosPos;
    Vector3[] foxesPos;

    List<GameObject> turtles;

    List<GameObject> mosquitos;

    List<GameObject> foxes;

    public static EnemyManager instance;
    public bool startDisabled = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        turtles = new List<GameObject>();
        mosquitos = new List<GameObject>();
        foxes = new List<GameObject>();

        for(int i = 0; i<levelEnemies.transform.childCount; i++)
        {
            if(levelEnemies.transform.GetChild(i).GetComponent<FlamencoBehaviour>())
            {
                mosquitos.Add(levelEnemies.transform.GetChild(i).gameObject);
            }
            else if(levelEnemies.transform.GetChild(i).GetComponent<TurtleController>())
            {
                turtles.Add(levelEnemies.transform.GetChild(i).gameObject);
            }
            else
            {
                foxes.Add(levelEnemies.transform.GetChild(i).gameObject);
            }
        }

        turtlesPos = new Vector3[turtles.Count];
        mosquitosPos = new Vector3[mosquitos.Count];
        foxesPos = new Vector3[foxes.Count];

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

        j = 0;

        foreach(GameObject g in foxes)
        {
            foxesPos[j] = g.transform.position;
        }

        if (startDisabled)
            DisableAll();
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

        i = 0;

        foreach (GameObject g in foxes)
        {
            g.transform.position = foxesPos[i];
            g.SetActive(true);
            i++;
        }

        if (startDisabled)
            DisableAll();
    }

    public void DisableAll()
    {
        int i = 0;

        foreach (GameObject g in turtles)
        { 
            g.SetActive(false);
            i++;
        }

        i = 0;

        foreach (GameObject g in mosquitos)
        {
            g.SetActive(false);
            i++;
        }

        i = 0;

        foreach (GameObject g in foxes)
        {
            g.SetActive(false);
            i++;
        }
    }

    public List<GameObject> GetList(string enemyType)
    {
        switch(enemyType)
        {
            case "Fox":
                return foxes;
            case "Turtle":
                return turtles;
            case "Mosquito":
                return mosquitos;
            default:
                return null;
        }
    }
}
