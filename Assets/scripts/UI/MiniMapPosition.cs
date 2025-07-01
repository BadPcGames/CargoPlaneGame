using UnityEngine;

public class MiniMapPosition : MonoBehaviour
{
    public GameObject plane;

    private void Awake()
    {
        plane = GameObject.FindGameObjectWithTag("PlayerPlane");
    }

    private void FixedUpdate()
    {
        if(plane!=null)
        transform.position=plane.transform.position+new Vector3(0,200,0);
    }

    private void OnEnable()
    {
        PlayerStats.OnAirplaneChanged += UpdateTarget;
    }

    private void OnDisable()
    {
        PlayerStats.OnAirplaneChanged -= UpdateTarget;
    }

    private void UpdateTarget(GameObject newPlane)
    {
        plane = newPlane;
    }
}
