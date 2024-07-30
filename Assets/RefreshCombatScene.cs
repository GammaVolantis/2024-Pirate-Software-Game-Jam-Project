using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RefreshCombatScene : MonoBehaviour
{
    public bool LoadedScene = false;
    public float timer = 4;
    // Update is called once per frame
    void Update()
    {
        timer-=Time.deltaTime;
        if (!LoadedScene && timer<0) 
        {
            SceneManager.LoadSceneAsync("WorkingCAFinal");
            LoadedScene = true;
        }
    }
}
