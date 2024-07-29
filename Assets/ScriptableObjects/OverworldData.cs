using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new OverworldData", menuName = "ScriptableObjects/OverworldData")]
public class OverworldData : ScriptableObject
{
    //Player Overworld Position Data
    public Vector3 playerPosition = new Vector3(-5.9f,-3.99f,0f);

    //Overworld Combat Scene Objects to load


    //Combat Scene Data to Pass
    public int minEnemies = 2;
    public int maxEnemies = 5;
    public string CombatSceneEnviornment = "Forest";

    //Methods to pass data below


}
