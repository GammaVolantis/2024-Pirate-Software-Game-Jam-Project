using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseSceneTransitionScript : MonoBehaviour
{
    public string nextScene = "Start Scene";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t")) 
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
