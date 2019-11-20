using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM_win : MonoBehaviour
{
    public GlobalGameController.JoinedPlayers winner;
    public GameObject spawnPoint;
    private string startScene = "StartScreen";

    // Start is called before the first frame update
    void Start()
    {
       winner = GlobalGameController.PlayerList[0];
        GameObject playerInstance = Instantiate(winner.charModel,
                                                        spawnPoint.transform.position,
                                                        spawnPoint.transform.rotation
                                                        );
        playerInstance.GetComponent<PlayerPlatformerController>().setControllerInputs(winner.playerController);
        StartCoroutine(endGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator endGame()
    {
        yield return new WaitForSecondsRealtime(3);
        SceneManager.LoadScene(startScene);
    }
}

