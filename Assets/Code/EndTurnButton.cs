using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    private LocationData locationData;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        locationData = Resources.Load<LocationData>("AllLocationInformation");
        button.onClick.AddListener(TaskOnClick);
    }
    void Update()
    {
        //if ()
        //{
        //    locationData.SetPlayerAction();
        //    locationData.SetPlayerMove();
        //    Debug.Log("Setting Player to Turn Over");
        //}
    }
    void TaskOnClick() 
    {
        locationData.SetPlayerAction();
        locationData.SetPlayerMove();
        locationData.CheckPlayerStatus();
        Debug.Log("Setting Player to Turn Over");
    }
}
