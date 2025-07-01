using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointMarker : MonoBehaviour
{
    [Header("Links")]
    public Transform target;               
    public Camera mainCamera;              
    public RectTransform canvasRect;      
    public RectTransform markerRect;       
    public GameObject distanceText;          

    [Header("Settings")]
    public Vector2 screenPadding = new Vector2(50, 50);  
    public float maxDrawDistance = 700f;                 

    private void Start()
    {
        mainCamera = Camera.main;
        canvasRect = GameObject.Find("PlayerCanvas").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null)
        {
            markerRect.gameObject.SetActive(false);
            return;
        }

        Vector3 camPos = mainCamera.transform.position;
        Vector3 dirToTarget = (target.position - camPos);
        float realDist = dirToTarget.magnitude;

        
        Vector3 worldPos;
        bool useExact = realDist <= maxDrawDistance;
        if (useExact)
        {
            worldPos = target.position;
        }
        else
        {
            worldPos = camPos + dirToTarget.normalized * maxDrawDistance;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        bool isBehind = screenPos.z < 0;


        if (isBehind)
        {
            screenPos *= -1;
        }

        bool onScreen = !isBehind
            && screenPos.x >= 0 + screenPadding.x
            && screenPos.x <= Screen.width - screenPadding.x
            && screenPos.y >= 0 + screenPadding.y
            && screenPos.y <= Screen.height - screenPadding.y;

        Vector3 clamped = screenPos;
        clamped.x = Mathf.Clamp(clamped.x, screenPadding.x, Screen.width - screenPadding.x);
        clamped.y = Mathf.Clamp(clamped.y, screenPadding.y, Screen.height - screenPadding.y);

        markerRect.gameObject.SetActive(true);

        if (useExact && onScreen)
        {
            markerRect.position = screenPos;
            markerRect.rotation = Quaternion.identity;
        }
        else
        {
            markerRect.position = clamped;
            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 toMarker = ((Vector2)clamped - screenCenter).normalized;
        }

        distanceText.GetComponent<Text>().text = Mathf.RoundToInt(realDist).ToString() + " m";
    }

    public void setTarget(Transform value)
    {
        target = value;
    }
}
