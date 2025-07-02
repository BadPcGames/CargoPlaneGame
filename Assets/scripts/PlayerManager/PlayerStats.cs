using HeneGames.Airplane;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static HeneGames.Airplane.SimpleAirPlaneController;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float money;

    public GameObject Plane;
    public bool haveCurgo;
    public Cargo currentCargo;
    public Vector3? targetPosition;
    public float reword;
    public Vector3 homePosition;

    public delegate void AirplaneChangedHandler(GameObject newPlane);
    public static event AirplaneChangedHandler OnAirplaneChanged;

    private GameObject playerUi;

    private void Awake()
    {
        playerUi = GameObject.Find("PlayerCanvas");
        Plane = GameObject.FindGameObjectWithTag("PlayerPlane");
        SpawnPlaneOnRunway();
        playerUi.GetComponent<PlayerUi>().setMoney(money);
        playerUi.GetComponent<PlayerUi>().setHaveCargo(false);
    }

    public void SpawnPlaneOnRunway()
    {
        GameObject runway = GameObject.Find("Home Runway");
        if (runway == null)
        {
            Debug.Log("Runway not found!");
            return;
        }

        Plane.transform.position= runway.transform.Find("Landing final pos").transform.position;
        var controller=Plane.GetComponent<SimpleAirPlaneController>();
        runway.GetComponent<Runway>().AddAirplane(controller);
        controller.AddLandingRunway(runway.GetComponent<Runway>());
        controller.airplaneState = AirplaneState.Landing;
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
            playerUi.transform.Find("WaipointToCargo").GetComponent<WaypointMarker>().setTarget(currentCargo.getTarget().getPosition());
            playerUi.GetComponent<PlayerUi>().setHaveCargo(true);
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
        setMoney((float)reword+money);
        currentCargo = null;
        targetPosition = null;
        reword = 0;
        playerUi.transform.Find("WaipointToCargo").GetComponent<WaypointMarker>().setTarget(null);
    }

    public void setMoney(float value)
    {
        money = value;
        if (value < 0)
        {
            value = 0;
        }
        playerUi.GetComponent<PlayerUi>().setMoney(money);
    }

    public float getMoney()
    {
        return money;
    }

    public string tryUnloadPlane(Transform station)
    {

        if (targetPosition!=null)
        {
            if(Vector3.Distance(Plane.transform.position, (Vector3)targetPosition) < 10)
            {
                deliveredCargo();
                playerUi.GetComponent<PlayerUi>().setHaveCargo(false);
                return "Cargo Unload";
            }
            else
            {
                return "Unckorect station for this cargo";
            }
           
        }
        return "";

    }

    public void ChangePlane(GameObject _newPlane)
    {
        GameObject newPlane = Instantiate(_newPlane, Plane.transform.position, Plane.transform.rotation);
        //newPlane.transform.parent = Plane.transform.parent;

        newPlane.tag = "PlayerPlane"; 

        var controller = newPlane.GetComponent<SimpleAirPlaneController>();
        Plane.GetComponent<SimpleAirPlaneController>().GetCurrentRunway().AddAirplane(controller);
        controller.AddLandingRunway(Plane.GetComponent<SimpleAirPlaneController>().GetCurrentRunway());
        controller.airplaneState = AirplaneState.Landing;

        Destroy(Plane);         
        Plane = newPlane;     

        OnAirplaneChanged?.Invoke(newPlane); 
    }


}
