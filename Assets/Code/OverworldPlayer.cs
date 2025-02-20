using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPlayer : MonoBehaviour
{
    private OverworldData overworldData;
    public float speed = 2f;
    private float doubleSpeed;
    private float normalSpeed;
    // Start is called before the first frame update
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;
    void Start()
    {
        overworldData = Resources.Load<OverworldData>("OverWorldData");
        this.gameObject.transform.position = overworldData.GetPlayerPosition();
        doubleSpeed = speed * 2;
        normalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w")) 
        {
            this.gameObject.transform.position += new Vector3(0f, speed*Time.deltaTime, 0f);
        }
        if (Input.GetKey("s"))
        {
            this.gameObject.transform.position += new Vector3(0f, -speed * Time.deltaTime, 0f);
        }
        if (Input.GetKey("d"))
        {
            this.gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey("a"))
        {
            this.gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey("left shift"))
        {
            speed = doubleSpeed;
        }
        else
        { 
            speed = normalSpeed;
        }

    }
}
