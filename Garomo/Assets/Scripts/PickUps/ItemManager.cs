using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject items;

    List<GameObject> itemList = new List<GameObject>();

    public static ItemManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for(int i=0;i<items.transform.childCount;i++)
        {
            itemList.Add(items.transform.GetChild(i).gameObject);
        }
    }

    public void RestartItems()
    {
        foreach(GameObject g in itemList)
        {
            g.SetActive(true);
        }
    }
}
