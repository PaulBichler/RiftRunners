using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorRain : MonoBehaviour
{
    private GM_Main gameModeRef;
    private Camera cameraToFollow;
    private float cameraHeight;
    private SpriteRenderer spritePortal;
    private Vector2 minScale;
    public Vector2 maxScale = new Vector2(0.7f, 0.7f);
    public float lerpSpeed = 2f;
    public float lerpDuration = 2f;
    public GameObject meteorPrefab;
    public float waitTimeMin = 30f;
    public float waitTimeMax = 120f;
    public int meteorCountMin = 5;
    public int meteorCountMax = 10;
    public float meteorSpawnTimeMin = 1f;
    public float meteorSpawnTimeMax = 2f;
    public float meteorSpeed = 10f;
    public float meteorLaunchAngleMin = 225f;
    public float meteorLaunchAngleMax = 315f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        gameModeRef = FindObjectOfType<GM_Main>();
        cameraToFollow = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        spritePortal = GetComponent<SpriteRenderer>();
        minScale = transform.localScale;

        while (!gameModeRef.gameOver)
        {
            yield return new WaitForSecondsRealtime(Random.Range(waitTimeMin, waitTimeMax));
            yield return Lerp(minScale, maxScale, lerpDuration);
            yield return Lerp(maxScale, minScale, lerpDuration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraToFollow.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, cameraToFollow.nearClipPlane));
    }

    public IEnumerator Lerp(Vector2 startSize, Vector2 endSize, float time)
    {
        float i = 0f;
        float rate = (1f / time) * lerpSpeed;

        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            transform.localScale = Vector2.Lerp(startSize, endSize, i);
            yield return null;
        }

        if (endSize == maxScale)
        {
            yield return Rain();
        } else
        {
            yield return null;
        }
    }

    public IEnumerator Rain()
    {
        for (int i = 1; i <= Random.Range(meteorCountMin, meteorCountMax); i++)
        {
            if(meteorPrefab)
            {
                Vector3 v;

                v.x = 0;
                v.y = 0;
                v.z = Random.Range(meteorLaunchAngleMin, meteorLaunchAngleMax);

                Quaternion randomAngle = Quaternion.Euler(v);

                Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-spritePortal.bounds.extents.x, 
                                                                                         spritePortal.bounds.extents.x), 
                                                     transform.position.y, 
                                                     transform.position.z);

                GameObject meteor = Instantiate(meteorPrefab, randomPosition, randomAngle, null);
                meteor.GetComponent<Rigidbody2D>().velocity = meteor.transform.right * meteorSpeed;
            }
            yield return new WaitForSecondsRealtime(Random.Range(meteorSpawnTimeMin, meteorSpawnTimeMax));
        }

        yield return null;
    }


}
