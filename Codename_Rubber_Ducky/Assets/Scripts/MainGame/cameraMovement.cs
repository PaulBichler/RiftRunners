﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public PlayerPlatformerController player;

    private Vector3 lastPlayerPosition;

    private float distanceToMove;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerPlatformerController>();
        lastPlayerPosition = player.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        distanceToMove = player.transform.position.x - lastPlayerPosition.x;

        transform.position = new Vector3(transform.position.x + distanceToMove, transform.position.y, transform.position.z);

        lastPlayerPosition = player.transform.position;
    }
}
