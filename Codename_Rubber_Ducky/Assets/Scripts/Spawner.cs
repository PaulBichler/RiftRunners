using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Spawner : MonoBehaviour
{
    public List<string> itemList;

    private bool spawned = false;
    // Start is called before the first frame update
    Spawner(List<string> list)
    {
        itemList = list;
        /* add other items*/
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(!spawned) SpawnItem();
    }

    private void SpawnItem()
    {       
        int size = itemList.Count;
        Random random = new Random();
        int randomNum = UnityEngine.Random.Range(0, size);

        string itemName = itemList.ElementAt(randomNum);
        Debug.Log(randomNum);
        ItemPickup pickup = FindObjectOfType<ItemPickup>();
        if (pickup != null)
        {
            spawned = true;
            pickup.makeItem(itemName,-1,transform.position,new Vector2(0,-2));
        }
        
    }
}