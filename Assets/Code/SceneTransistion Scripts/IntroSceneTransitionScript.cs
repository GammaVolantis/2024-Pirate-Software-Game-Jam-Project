using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneTransitionScript : MonoBehaviour
{
    public string nextScene;
    public float yVal;

    private Vector3 cameraPosition;
    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = Camera.main.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraPosition.y > yVal)
        {
            cameraPosition.y -= Time.deltaTime;
            Camera.main.transform.position = cameraPosition;
        }
        else 
        {
            if (Input.GetKeyDown("t")) 
            {
                SceneManager.LoadSceneAsync(nextScene);
            }
        }
    }
}
