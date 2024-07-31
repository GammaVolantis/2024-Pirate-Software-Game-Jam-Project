using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startingPos;
    private Vector3 startingSize;
    private CardStats cardStats;
    private LocationData locationData;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        startingPos = rectTransform.anchoredPosition;
        CardInfoScript cardInfo = GetComponent<CardInfoScript>();
        cardStats = cardInfo.card;
        locationData = Resources.Load<LocationData>("AllLocationInformation");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;  // Make it slightly transparent
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta /  1;
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Vector3 rayPoint = ray.GetPoint(Vector3.Distance(transform.position, Camera.main.transform.position));
        rayPoint.z= 0;
        transform.position = rayPoint;
        //Debug.Log("CardPos= " + Camera.main.WorldToScreenPoint(transform.position) + "startingPosition = " + startingPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Vector3 finalPos = ray.origin;
        finalPos.z = 0;
        if (cardStats.target == "Player" && !locationData.GetPlayerAction()) 
        {
            Vector3 playerLoc = locationData.GetPlayerReal();
            if (Mathf.Sqrt(Mathf.Pow(finalPos.x-playerLoc.x,2)+Mathf.Pow(finalPos.y - playerLoc.y, 2)) <=1) 
            {
                Debug.Log("Used Card on Player!!!");
                locationData.GetPlayerObject().GetComponent<HealthBar>().HealthBarUpdate(cardStats.actionValue*10);
                playerCommittedAction();

            }
        }
        else if (cardStats.target == "Enemy" && !locationData.GetPlayerAction()) 
        {
            int enemyLoc = FindClosestEnemy(finalPos);
            if (enemyLoc != -1) 
            {
                Vector3Int enemyPos = locationData.GetEnemyVirtual(enemyLoc);
                Debug.Log("enemyPos=" + enemyPos + "   enemyIter= " + enemyLoc);
                Vector3Int playerPos = locationData.GetPlayerVirtual();
                Debug.Log("playerPos" + playerPos);
                float playerToTarget = Mathf.Floor(Mathf.Sqrt(Mathf.Pow(enemyPos.x - playerPos.x, 2) + Mathf.Pow(enemyPos.y - playerPos.y, 2)));
                if (playerToTarget <= cardStats.range)
                {

                    Debug.Log("Attacking Enemy!!! DistanceToTarget= " + playerToTarget);
                    GameObject dieEnemy = locationData.GetEnemyObject(enemyLoc);
                    dieEnemy.GetComponent<EnemyHealth>().EnemyHealthUpdate(-cardStats.actionValue*10);
                    playerCommittedAction();
                    if (dieEnemy.GetComponent<EnemyHealth>().GetHealth() == 0) {
                        if (locationData.GetNumberOfEnemies() == 1)
                        {
                            Destroy(dieEnemy);
                            locationData.RemoveEnemy(enemyLoc);
                            SceneManager.LoadScene("WorldScene");
                        }
                        else
                        {
                            Destroy(dieEnemy);
                            locationData.RemoveEnemy(enemyLoc);
                        }
                    }
                }
            }
        }
        Debug.Log("Card Dropped");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = startingPos;
        

    }


    private int FindClosestEnemy(Vector3 finalPos) 
    {
        List<float> distance = new List<float>();
        float tempDistance;
        foreach (Vector3 enemy in locationData.GetAllEnemyReal())
        {
            tempDistance = Mathf.Sqrt(Mathf.Pow(finalPos.x - enemy.x,2)+ Mathf.Pow(finalPos.y - enemy.y, 2));
            distance.Add(tempDistance);
        }
        float lowestVal = distance[0];
        int lowestValLoc = 0;
        int tempLoc = 0;
        foreach (float len in distance)
        {
            Debug.Log("lowestVal = " + lowestVal + " > " + len + " = Len");
            if (lowestVal > len)
            {
                lowestVal = len;
                lowestValLoc = tempLoc;
            }
            tempLoc++;
        }
        if (lowestVal < 1)
        {
            return lowestValLoc;
        }
        else 
        {
            return -1;
        }
    }
    private void playerCommittedAction() 
    {
        locationData.SetPlayerAction();
        locationData.CheckPlayerStatus();
    }
}
