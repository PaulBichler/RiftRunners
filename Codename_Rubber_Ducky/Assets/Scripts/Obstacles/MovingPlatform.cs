using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    public float speed = 1.0F;
    public List<Transform> checkpoints;
    private int nextIndex = 1;
    private Rigidbody2D rb2d;
    private Vector3 previousPos;
    public Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if (checkpoints.Count != 0)
        {
            //initiates the object at the position of the first checkpoint
            transform.position = checkpoints[0].position;
            previousPos = transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (checkpoints.Count != 0)
        {
            //check if next checkpoint was reached
            if (transform.position == checkpoints[nextIndex].position)
            {
                //set next checkpoint to the next one in the list or the first one if the last one was reached
                if (nextIndex == checkpoints.Count - 1)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex++;
                }
            }

            //move platform
            transform.position = Vector3.MoveTowards(transform.position, checkpoints[nextIndex].position, Time.deltaTime * speed);

            if (Time.fixedDeltaTime != 0)
            {
                velocity = (transform.position - previousPos) / Time.fixedDeltaTime;
                previousPos = transform.position;
            }
        }
    }
}