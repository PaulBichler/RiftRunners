using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerAssigner : MonoBehaviour
{
    public int panelNumber;
    private GM_CharacterSelection gameModeReference;
    private int assignedControllerNumber;
    private bool isAssigned;
    private ControllerAssigner previousPanel;

    // Start is called before the first frame update
    void Start()
    {
        gameModeReference = FindObjectOfType<GM_CharacterSelection>();
        isAssigned = false;

        if (panelNumber > 1)
        {
            foreach (ControllerAssigner panel in FindObjectsOfType<ControllerAssigner>())
            {
                //sets the previous panel only if this panel is not the first one
                if(panelNumber == panel.panelNumber + 1)
                {
                    previousPanel = panel;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //checks if a controller is already assigned to this panel
        if (!isAssigned) {
    
            if (previousPanel && previousPanel.getAssigned() || panelNumber == 1)
            {
                //The panel only checks for input if the previous panel is already assigned 
                //(or if the panel number is 1, since the first panel has no "previous panel")
                foreach (int controller in FindObjectOfType<GM_CharacterSelection>().getPlayerControllers())
                {
                    //the panel checks the input of every controller that hasn't been assigned yet
                    if (Input.GetButtonDown("P" + controller + "Jump"))
                    {
                        //if the panel detects input from a controller, that controller gets assigned to this panel
                        //and is removed from the controller list
                        gameModeReference.removePlayerController(controller);
                        isAssigned = true;
                        //the panel also tells the "selector" which controller to listen to, when selecting the character
                        GetComponentInParent<CharacterSelector>().activate(controller);
                        break;
                    }
                }
            }
        }
    }

    public bool getAssigned()
    {
        //returns true if the panel has a controller assigned 
        //(this is used to tell the next panel that it can start listening to input)
        return isAssigned;
    }

    public void setPreviousPanel(ControllerAssigner prevPanel)
    {
        previousPanel = prevPanel;
    }

    public void unassign()
    {
        isAssigned = false;

        if(panelNumber > 1)
        {
            foreach (ControllerAssigner panel in FindObjectsOfType<ControllerAssigner>())
            {
                //sets the previous panel only if this panel is not the first one
                if (!panel.getAssigned() && panel.panelNumber > panelNumber)
                {
                    panel.setPreviousPanel(this);
                    break;
                }
            }
        }
    }

}
