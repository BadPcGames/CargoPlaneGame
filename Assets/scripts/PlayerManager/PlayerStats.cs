using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float money;

    public GameObject Plane;
    public bool haveCurgo;
    public Cargo currentCargo;
    public Vector3? targetPosition;
    public float reword;

    private GameObject playerUi;

    private void Awake()
    {
        playerUi = GameObject.Find("PlayerCanvas");
    }

    public Cargo getCurrentCargo()
    {
        return currentCargo;
    }

    public string setCargo(Cargo value)
    {
        Debug.Log($"[PlayerStats] setCargo on {gameObject.name}, ID: {this.GetInstanceID()}");

        if (targetPosition == null)
        {
            currentCargo = value;
            targetPosition= currentCargo.getTarget().getPosition().position;
            reword = currentCargo.getReword();
            playerUi.GetComponent<WaypointMarker>().setTarget(currentCargo.getTarget().getPosition());
            return $" Cargo set! Target: Station ¹{currentCargo.getTarget().getId()}, Reward: {currentCargo.getReword()}";
        }
        else
        {
           return "You alredy have cargo";
        }
    }

    public string LoadCargoToPlane(Cargo c)
    {
        if (gameObject == null || gameObject.GetComponent<PlayerStats>() == null)
        {
            Debug.LogError("Player or PlayerStats not found!");
        }
        return setCargo(c);
    }


    public void deliveredCargo()
    {
        addMoney((float)reword);
        currentCargo = null;
        targetPosition = null;
        reword = 0;
        playerUi.GetComponent<WaypointMarker>().setTarget(null);
    }

    public void addMoney(float value)
    {
        money+= value;
        if (value < 0)
        {
            value = 0;
        }
        Debug.Log(money);
    }

    public string tryUnloadPlane(Transform station)
    {

        if (targetPosition!=null)
        {
            if(Vector3.Distance(Plane.transform.position, (Vector3)targetPosition) < 10)
            {
                deliveredCargo();
                return "Cargo Unload";
            }
            else
            {
                return "Unckorect station for this cargo";
            }
           
        }
        return "";

    }

  


}
