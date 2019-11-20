using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GM_CharacterSelection : MonoBehaviour
{
    private List<int> playerControllers;

    [System.Serializable] public struct CharPanelImage
    {
        public Sprite charBackground;
        public GameObject charPose;
        public Sprite charNameTag;
        public GameObject charModel;
    }
    public Sprite emptyBG;

    public List<CharPanelImage> charImageList;
    private List<int> selectedCharIndexList;

    public float selectInputCheckTimer = 0.2f;
    public float idleTimer = 60f;
    public float countdown = 11f;

    private List<CharacterSelector> activatedPanels;
    private float currentIdleTime;
    private string startScreenLevel = "StartScreen";
    private string mainGameLevel = "TestScene";

    private Transform countdownPanel;
    private float currentCountdown;
    

    // Start is called before the first frame update
    void Start()
    {
        
        playerControllers = new List<int>();

        //populates the list with every available controller number
        for (int i = 1; i <= GlobalGameController.playerControllerCount; i++)
        {
            playerControllers.Add(i);
        }

        selectedCharIndexList = new List<int>();

        activatedPanels = new List<CharacterSelector>();

        currentIdleTime = idleTimer;

        countdownPanel = GameObject.FindWithTag("Countdown").transform;
        currentCountdown = countdown;
    }

    // Update is called once per frame
    void Update()
    {
        //checks every frame if an input has been made, 
        //if no input has been made for a certain time (idleTimer), the game switches back to the start screen
        if (Input.anyKeyDown)
        {
            currentIdleTime = idleTimer;
        } else if (currentIdleTime <= 0)
        {
            SceneManager.LoadScene(startScreenLevel);
        } else
        {
            currentIdleTime -= Time.deltaTime;
        }

        //When every player is ready, the function starts and updates the countdown,
        //when the countdown reaches 0, it loads the main Game level
        if (updateCountdownPanel(checkReadyStates(), Mathf.Floor(currentCountdown))) {
            initMainGame();
        }
    }


    //PRIVATE METHODS
    private bool checkReadyStates() {

        int readyCount = 0;

        foreach (CharacterSelector panel in activatedPanels)
        {
            if(!panel.getReadyState())
            {
                return false;
            } else
            {
                readyCount++;
            }
        }

        //The game needs at least 2 players to start
        if (readyCount >= 2)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private bool updateCountdownPanel(bool state, float time)
    {
        TextMeshProUGUI textComp = countdownPanel.Find("Countdown").GetComponent<TextMeshProUGUI>();

        if (state)
        {
            currentCountdown -= Time.deltaTime;
        }
        else
        {
            currentCountdown = countdown;
        }

        if (currentCountdown <= 0)
        {
            return true;
        }

        countdownPanel.gameObject.SetActive(state);
        textComp.text = time.ToString();

        return false;
    }

    private void initMainGame()
    {
        GlobalGameController.PlayerList.Clear();

        foreach (CharacterSelector panel in activatedPanels)
        {
            if (panel.getReadyState())
            {
                GlobalGameController.JoinedPlayers player;
                player.playerController = panel.getAssignedControllerNumber();
                player.charModel = panel.getCharModel();

                GlobalGameController.PlayerList.Add(player);
            }
        }

        SceneManager.LoadScene(mainGameLevel);
    }


    //PUBLIC METHODS
    public List<int> getPlayerControllers()
    {
        //return the list of the available controller numbers
        return playerControllers;
    }

    public void removePlayerController(int controllerNumber)
    {
        //removes a controller number from the list (this happens when a controller has been assigned to a panel)
        playerControllers.Remove(controllerNumber);
    }

    public void addPlayerController(int controllerNumber)
    {
        playerControllers.Add(controllerNumber);
    }

    public void addActivatedPanel(CharacterSelector panel)
    {
        //When a player joins, the panel he is bound to is added to the list
        activatedPanels.Add(panel);
    }

    public void removeActivatedPanel(CharacterSelector panel)
    {
        //When a player leaves, the panel he is bound to is removed from the list
        activatedPanels.Remove(panel);
    }

    public void addSelectedCharIndex(int charIndex)
    {
        //When a character is selected, the index of the character is added to the list
        selectedCharIndexList.Add(charIndex);
    }

    public void removeSelectedCharIndex(int charIndex)
    {
        //When a character is de-selected, the index of the character is removed from the list
        selectedCharIndexList.Remove(charIndex);
    }

    public bool findSelectedCharIndex(int charIndex)
    {
        //checks if the specified character is already selected (in the list)
        return selectedCharIndexList.Contains(charIndex);
    }
}
