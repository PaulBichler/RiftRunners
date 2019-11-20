using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalGameController
{
    //This is a static Game Controller Class. This script does not need to be bound to a GameObject, the varibales can be accessed like this: 
    //GlobalGameController.myVariable

    public static int playerControllerCount = 4;

    [System.Serializable] public struct JoinedPlayers
    {
        public int playerController;
        public GameObject charModel;
    }

    public static List<JoinedPlayers> PlayerList = new List<JoinedPlayers>();
}
