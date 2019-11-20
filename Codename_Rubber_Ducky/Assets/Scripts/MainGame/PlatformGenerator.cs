using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    
    public Transform generationPoint;
    public float distanceBetween;

    private float platformWidth;

    //public GameObject[] platforms;
    private int platformSelector;
    private float[] platformWidths;

    public ObjectPooler[] objectPools;

    // Start is called before the first frame update
    void Start()
    {
        //platformWidth = platform.GetComponent<BoxCollider2D>().size.x;

        platformWidths = new float[objectPools.Length];
        for(int i = 0; i < platformWidths.Length; i++)
        {
            platformWidths[i] = objectPools[i].pooledObject.GetComponent<BoxCollider2D>().size.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if the platform generator is closer to the player than the generation point, the generator moves towards where the next platform should be
        if(transform.position.x < generationPoint.position.x)
        {
            platformSelector = Random.Range(0, objectPools.Length);

            transform.position = new Vector3(transform.position.x + (platformWidths[platformSelector] / (float)2.0)+ distanceBetween, transform.position.y, transform.position.z);

           

            //Instantiate(platform, transform.position, transform.rotation);

            //Instantiate(platforms[platformSelector], transform.position, transform.rotation);

            GameObject newPlatform =  objectPools[platformSelector].GetPooledObject();

            newPlatform.transform.position = transform.position;
            newPlatform.transform.rotation = transform.rotation;
            newPlatform.SetActive(true);

            transform.position = new Vector3(transform.position.x + (platformWidths[platformSelector] / 2), transform.position.y, transform.position.z);

        }
    }
}
