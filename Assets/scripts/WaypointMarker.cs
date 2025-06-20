using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaypointMarker : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform target;               // цель в мире
    public Camera mainCamera;              // основная камера
    public RectTransform canvasRect;       // RectTransform канваса
    public RectTransform markerRect;       // RectTransform маркера
    public GameObject distanceText;          // текст расстояния

    [Header("Настройки")]
    public Vector2 screenPadding = new Vector2(50, 50);  // отступ от краёв экрана
    public float maxDrawDistance = 700f;                 // максимальная дальность прорисовки

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

        // Вычисляем точку для проекции:
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

        // Если точка за камерой — инвертируем экранные координаты
        if (isBehind)
        {
            screenPos *= -1;
        }

        // Проверяем, находится ли в кадре точка-цель (только для useExact)
        bool onScreen = !isBehind
            && screenPos.x >= 0 + screenPadding.x
            && screenPos.x <= Screen.width - screenPadding.x
            && screenPos.y >= 0 + screenPadding.y
            && screenPos.y <= Screen.height - screenPadding.y;

        // Клэмпим в рамки экрана
        Vector3 clamped = screenPos;
        clamped.x = Mathf.Clamp(clamped.x, screenPadding.x, Screen.width - screenPadding.x);
        clamped.y = Mathf.Clamp(clamped.y, screenPadding.y, Screen.height - screenPadding.y);

        // Включаем маркер
        markerRect.gameObject.SetActive(true);

        if (useExact && onScreen)
        {
            // Цель в зоне видимости и ближе 700м — рисуем прямо над ней
            markerRect.position = screenPos;
            markerRect.rotation = Quaternion.identity;
        }
        else
        {
            // Цель далеко или за спиной — рисуем на краю экрана с поворотом к ней
            markerRect.position = clamped;

            // Рассчитываем угол стрелки от центра экрана
            Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 toMarker = ((Vector2)clamped - screenCenter).normalized;
            //float angle = Mathf.Atan2(toMarker.y, toMarker.x) * Mathf.Rad2Deg;
            //markerRect.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        // Обновляем текст расстояния (округлённый)
        distanceText.GetComponent<Text>().text = Mathf.RoundToInt(realDist).ToString() + " m";
    }

    public void setTarget(Transform value)
    {
        target = value;
    }
}
