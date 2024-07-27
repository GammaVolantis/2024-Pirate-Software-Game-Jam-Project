using UnityEngine;
using UnityEngine.EventSystems;

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
        Debug.Log("CardPos= " + Camera.main.WorldToScreenPoint(transform.position) + "startingPosition = " + startingPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        Vector3 finalPos = ray.origin;
        finalPos.z = 0;
        if (cardStats.target == "Player") 
        { 

        }
        else if (cardStats.target == "Enemy") 
        {
            
        }
        Debug.Log("Card Dropped");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.anchoredPosition = startingPos;
        

    }
}
