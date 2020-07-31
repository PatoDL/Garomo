using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject items;

    List<GameObject> itemList = new List<GameObject>();

    public static ItemManager instance;

    public bool startDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for(int i=0;i<items.transform.childCount;i++)
        {
            itemList.Add(items.transform.GetChild(i).gameObject);
        }

        if (startDisabled)
            DisableItems();
    }

    private void DisableItems()
    {
        foreach (GameObject g in itemList)
        {
            g.SetActive(false);
        }
    }

    public void RestartItems()
    {
        foreach(GameObject g in itemList)
        {
            g.SetActive(true);
        }

        if (startDisabled)
            DisableItems();
    }

    public GameObject GetItemOfTag(string itemTag)
    {
        foreach(GameObject g in itemList)
        {
            if(g.tag == itemTag)
            {
                return g;
            }
        }

        return null;
    }

    public List<GameObject> GetListOfTag(string itemTag)
    {
        List<GameObject> toReturn = new List<GameObject>();

        foreach (GameObject g in itemList)
        {
            if (g.tag == itemTag)
            {
                toReturn.Add(g);
            }
        }

        return toReturn;
    }
}
