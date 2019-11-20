using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM_StartScreen : MonoBehaviour
{
    public string sceneToLoad = "CharacterSelectMenu";
    // Start is called before the first frame update
    void Start()
    {
        GlobalGameController.PlayerList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
