using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class customSoundManagerScript : MonoBehaviour
{

    public static AudioClip bombTick, deselect, select, pickUp, shot, slowMoStart, slowMoEnd, menuSwitch, walking;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        bombTick = Resources.Load<AudioClip>("bombtick");
        deselect = Resources.Load<AudioClip>("deselect");
        select = Resources.Load<AudioClip>("select");
        pickUp = Resources.Load<AudioClip>("gunpickup");
        shot = Resources.Load<AudioClip>("shot");
        slowMoStart = Resources.Load<AudioClip>("slowmoend");
        slowMoEnd = Resources.Load<AudioClip>("slowmostart");
        menuSwitch = Resources.Load<AudioClip>("menuswitch");
        walking = Resources.Load<AudioClip>("walking");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string Clip)
    {
        switch(Clip)
        {
            case "bombtick":
                audioSrc.PlayOneShot(bombTick);
                break;
            case "deselect":
                audioSrc.PlayOneShot(deselect);
                break;
            case "select":
                audioSrc.PlayOneShot(select);
                break;
            case "pickup":
                audioSrc.PlayOneShot(pickUp);
                break;
            case "shot":
                audioSrc.PlayOneShot(shot);
                break;
            case "slowmoend":
                audioSrc.PlayOneShot(slowMoEnd);
                break;
            case "slowmostart":
                audioSrc.PlayOneShot(slowMoStart);
                break;
            case "menuswitch":
                audioSrc.PlayOneShot(menuSwitch);
                break;
            case "walking":
                audioSrc.PlayOneShot(walking);
                break;
        }

    }
}
