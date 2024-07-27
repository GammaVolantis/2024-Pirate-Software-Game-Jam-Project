using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationChecker : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Objects Location= " + Camera.main.WorldToScreenPoint(transform.position));
    }
}
