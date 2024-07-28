using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    [SerializeField] Camera mouseCam;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 mousePos = mouseCam.ScreenToWorldPoint(Input.mousePosition);
        //mousePos.z = 0f;
        //transform.position = mousePos;
        //Debug.Log(mousePos);

        Ray mousePos = mouseCam.ScreenPointToRay(Input.mousePosition);
        Vector3 visMousePos = mousePos.GetPoint(distance);
        visMousePos.z = 0;
        transform.position = visMousePos;
        Debug.Log(mousePos);
    }
}
