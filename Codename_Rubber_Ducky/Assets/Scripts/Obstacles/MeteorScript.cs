using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    public int stunStrength = 1;

    void Update()
    {
        if(transform.position.y < -30)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("player hit!");
            other.gameObject.GetComponent<PlayerPlatformerController>().startStun(stunStrength);
        }

        Destroy(gameObject);
    }
}
