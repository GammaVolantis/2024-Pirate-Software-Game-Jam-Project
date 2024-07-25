using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 storedPos = character.transform.position;
        storedPos.z = 0;
        storedPos.y += 5;
        transform.position = storedPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 storedPos = character.transform.position;
        storedPos.z = 0;
        storedPos.y += .5f;
        transform.position = storedPos;
    }
}
