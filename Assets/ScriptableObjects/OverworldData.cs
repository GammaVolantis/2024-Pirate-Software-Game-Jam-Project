using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new OverworldData", menuName = "ScriptableObjects/OverworldData")]
public class OverworldData : ScriptableObject
{

    //Combat Scene Data to Pass
    public struct CombatSceneData 
    {
        private int min;
        private int max;
        private string Environment;
        private GameObject OverWorldIcon;

        public void newCombatIcon(int min, int max, string envo, GameObject OWI) 
        { 
            this.min = min;
            this.max = max;
            this.Environment = envo;
            this.OverWorldIcon = OWI;
        }
        public GameObject GetIcon() 
        {
            return this.OverWorldIcon;
        }
        public string GetEnviornment() 
        { 
            return this.Environment;
        }
        public int GetMaxEnemies() 
        { 
            return this.max;
        }
        public int GetMinEnemies()
        {
            return this.min;
        }
    }

    public List<CombatSceneData> globalCombatSceneData;

    //Player Overworld Position Data
    public Vector3 playerPosition = new Vector3(-5.9f, -3.99f, 0f);

    //Overworld Combat Scene Objects to load


    //Combat Scene Data to Pass

    //Methods to pass data below


}
