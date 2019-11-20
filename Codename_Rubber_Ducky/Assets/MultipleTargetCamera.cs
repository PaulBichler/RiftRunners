using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = .5f;
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomLimiter = 10f;
    public float cameraSpeed = 80f;
    public float playerSpeed = 8f;

    private Vector3 velocity;
    private Camera cam;
    private Vector3 xOffset;
    private float originalCamSpeed;
    private GM_Main gameModeRef;

    void Start()
    {
        gameModeRef = FindObjectOfType<GM_Main>();
        cam = GetComponent<Camera>();
        originalCamSpeed = cameraSpeed;

        foreach (GameObject playerInstance in gameModeRef.playerInstances)
        {
            targets.Add(playerInstance.transform);
        }
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
        {
            //return;
        }

        Move();

        Zoom();
    }

    Vector3 GetCenterPoint()
    {
        //if there are no targets the center point stay at the cameras position

        if (targets.Count == 0)
        {
            return gameObject.transform.position;
        }

        else if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();



        float furthestPlayerX = 0;

        if (targets.Count != 0)
        {
            furthestPlayerX = targets[0].position.x;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (furthestPlayerX < targets[i].position.x)
            {
                furthestPlayerX = targets[i].position.x;
            }
        }

        float camHeight = 2f * cam.orthographicSize;

        float camWidth = camHeight * cam.aspect;

        float border = transform.position.x + camWidth * 0.35f;

        Vector3 newPosition;

        if (furthestPlayerX >= border)
        {
            cameraSpeed = playerSpeed;
        }
        else
        {
            cameraSpeed = originalCamSpeed;
        }
        newPosition = offset + xOffset;
        //Debug.Log(newPosition);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

        xOffset += new Vector3(cameraSpeed, 0, 0) * Time.deltaTime;
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(minZoom, maxZoom, GetGreatestDistance() / zoomLimiter);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    private float GetGreatestDistance()
    {
        if (targets.Count != 0)
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);


            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }

            return bounds.size.x;
        }

        else
            return 1;
    }

    public void RemoveCharacter(Transform playerTransform)
    {

        if (targets.Contains(playerTransform))
        {
            targets.Remove(targets.Find(p => p == playerTransform));
            //Debug.Log("Object removed from list");

        }

    }


    public void AddCharacter(Transform playerTransform)
    {

        if (!targets.Contains(playerTransform))
        {
            targets.Add(playerTransform);
            //Debug.Log("object added to camera list");
        }
       // else
            //Debug.Log("object already in camera list");

    }
}

