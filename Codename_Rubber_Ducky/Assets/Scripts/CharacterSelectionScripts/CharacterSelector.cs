using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelector : MonoBehaviour
{
    private GM_CharacterSelection gameModeReference;
    private ControllerAssigner controllerAssignerRef;
    private int panelNumber;
    private int assignedControllerNumber;
    private float inputCheckTimer;
    private float currentInputCheckTimer;
    private int charImageIndex;
    private bool isReady;

    private Image charBackground;
    private Transform charPoseSpawn;
    private GameObject charPose;
    private Image charNameTag;
    private Image readyOverlay;
    private TextMeshProUGUI buttonInstructions;

    private string notJoined = "Press Jump To Join!";
    private string joined = "Select your Character! Press Action to leave!";
    private string ready = "Ready! \n Press Jump to unready!";

    // Start is called before the first frame update
    void Start()
    {
        gameModeReference = FindObjectOfType<GM_CharacterSelection>();
        controllerAssignerRef = GetComponentInParent<ControllerAssigner>();
        panelNumber = GetComponentInParent<ControllerAssigner>().panelNumber;
        inputCheckTimer = gameModeReference.selectInputCheckTimer;
        currentInputCheckTimer = inputCheckTimer;
        charImageIndex = 0;
        isReady = false;

        charBackground = GetComponentInParent<Image>();
        charNameTag = transform.Find("NameTag").GetComponent<Image>();
        charPoseSpawn = transform.Find("CharacterPoseSpawn");
        readyOverlay = transform.Find("ReadyOverlay").GetComponent<Image>();
        buttonInstructions = transform.Find("ButtonInstructions").GetComponent<TextMeshProUGUI>();
        buttonInstructions.text = notJoined;
    }

    // Update is called once per frame
    void Update()
    {
        if (assignedControllerNumber > 0 && currentInputCheckTimer <= 0)
        {
            if (!isReady)
            {
                if (gameModeReference.findSelectedCharIndex(charImageIndex))
                {
                    getNextCharacter();
                }

                if (Input.GetAxis("P" + assignedControllerNumber + "Horizontal") == 1f)
                {
                    getNextCharacter();
                    currentInputCheckTimer = inputCheckTimer;

                }
                else if (Input.GetAxis("P" + assignedControllerNumber + "Horizontal") == -1f)
                {
                    getPreviousCharacter();
                    currentInputCheckTimer = inputCheckTimer;

                }
                else if (Input.GetButtonDown("P" + assignedControllerNumber + "Jump"))
                {
                    setReadyState(true);
                }
                else if (Input.GetButtonDown("P" + assignedControllerNumber + "Action"))
                {
                    deactivate();
                }
            } else
            {
                //when a character is selected
                if(Input.GetButtonDown("P" + assignedControllerNumber + "Jump"))
                {
                    setReadyState(false);
                }
            }
        }
        else
        {
            currentInputCheckTimer -= Time.deltaTime;
        }
    }

    public void activate(int controllerNumber)
    {
        assignedControllerNumber = controllerNumber;
        setCharacter();
        charNameTag.color = new Color(255f, 255f, 255f, 1f);
        buttonInstructions.text = joined;
        gameModeReference.addActivatedPanel(this);
    }

    public void deactivate()
    {
        gameModeReference.addPlayerController(assignedControllerNumber);
        assignedControllerNumber = 0;
        unsetCharacter();
        charNameTag.color = new Color(255f, 255f, 255f, 0f);
        buttonInstructions.text = notJoined;
        gameModeReference.removeActivatedPanel(this);
        controllerAssignerRef.unassign();
    }

    private void getNextCharacter()
    {
        if(charImageIndex == gameModeReference.charImageList.Count - 1)
        {
            charImageIndex = 0;
        } else
        {
            charImageIndex++;
        }

        if (gameModeReference.findSelectedCharIndex(charImageIndex))
        {
            getNextCharacter();
        } else
        {
            setCharacter();
        }
    }

    private void getPreviousCharacter()
    {
        if(charImageIndex == 0)
        {
            charImageIndex = gameModeReference.charImageList.Count - 1;
        } else
        {
            charImageIndex--;
        }

        if (gameModeReference.findSelectedCharIndex(charImageIndex))
        {
            getPreviousCharacter();
        }
        else
        {
            setCharacter();
        }
    }

    private void setCharacter()
    {
        charBackground.sprite = gameModeReference.charImageList[charImageIndex].charBackground;
        charNameTag.sprite = gameModeReference.charImageList[charImageIndex].charNameTag;
        Destroy(charPose);
        charPose = Instantiate(gameModeReference.charImageList[charImageIndex].charPose,
                                charPoseSpawn.position,
                                charPoseSpawn.rotation);
    }

    private void unsetCharacter()
    {
        charBackground.sprite = gameModeReference.emptyBG;
        charNameTag.sprite = null;
        Destroy(charPose);
    }

    private void setReadyState(bool state)
    {
        if(state)
        {
            gameModeReference.addSelectedCharIndex(charImageIndex);
            buttonInstructions.text = ready;
        } else
        {
            gameModeReference.removeSelectedCharIndex(charImageIndex);
            buttonInstructions.text = joined;
        }
        
        charPose.GetComponent<Animator>().enabled = !state;
        readyOverlay.enabled = state;
        isReady = state;
    }

    public bool getReadyState()
    {
        return isReady;
    }

    public GameObject getCharModel()
    {
        return gameModeReference.charImageList[charImageIndex].charModel;
    }

    public int getAssignedControllerNumber()
    {
        return assignedControllerNumber;
    }
}
