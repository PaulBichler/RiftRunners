using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Game Mode Class for the Main Game (used to initialise the game and keep scores if necessary)
public class GM_Main : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject[] uiWidgets;
    private string winScene = "Win Screen";

    //[HideInInspector]
    public List<GameObject> playerInstances;

    //[HideInInspector]
    public bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInstances = new List<GameObject>();
        spawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawnPlayers()
    {
        int SPIndex = 0;

        foreach (GlobalGameController.JoinedPlayers player in GlobalGameController.PlayerList)
        {
            if (spawnPoints[SPIndex])
            {
                GameObject playerInstance = Instantiate(player.charModel,
                                                        spawnPoints[SPIndex].transform.position,
                                                        spawnPoints[SPIndex].transform.rotation
                                                        );
                playerInstance.GetComponent<PlayerPlatformerController>().setControllerInputs(player.playerController);
                playerInstance.GetComponent<PlayerPlatformerController>().bindUIWidget(uiWidgets[SPIndex]);
                playerInstances.Add(playerInstance);

                SPIndex++;
            } else
            {
                Debug.LogError("Missing spawn point! Unable to spawn player" + player.charModel + "!");
            }
        }
    }


    //returns pointer to player instances
    public List<GameObject> getPlayers()
    {
        return playerInstances;
    }

    public void setGameOver()
    {
        Debug.Log("YOU WIN");
        gameOver = true;
        SceneManager.LoadScene(winScene);
    }

    public void removePlayerInstance(GameObject player)
    {
        playerInstances.Remove(player);
    }

    public void checkGameState()
    {
        if(playerInstances.Count == 1)
        {
            setGameOver();
        }
    }
}
