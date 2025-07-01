using UnityEngine;

public class PlayerUi : MonoBehaviour
{ 
    private void FixedUpdate()
    {
        HomeWaipointManager();
    }


    private void HomeWaipointManager()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (transform.Find("WaipointToBase").GetComponent<WaypointMarker>().target == null)
            {
                transform.Find("WaipointToBase").GetComponent<WaypointMarker>().setTarget(GameObject.FindGameObjectWithTag("Base").transform);
            }
            else
            {
                transform.Find("WaipointToBase").GetComponent<WaypointMarker>().setTarget(null);
            }
        }
    }
}
