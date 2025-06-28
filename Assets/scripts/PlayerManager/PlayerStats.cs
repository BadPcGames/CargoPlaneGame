using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public GameObject Plane;
    public bool haveCurgo;

    [SerializeField]
    private float money;

    public Cargo currentCargo;
    private GameObject playerUi;

    public Vector3? targetPosition;
    public float reword;

    private void Awake()
    {
        playerUi = GameObject.Find("PlayerCanvas");
    }

    public Cargo getCurrentCargo()
    {
        return currentCargo;
    }

    public List<Cargo> getCargoList()
    {
        return cargoList;
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

    public string LoadCargoToPlane(int index)
    {
        if (gameObject == null || gameObject.GetComponent<PlayerStats>() == null)
        {
            Debug.LogError("Player or PlayerStats not found!");
        }
        return setCargo(cargoList[index]);
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

    private List<Cargo> cargoList;
    private GameObject currentStation;

    public void GanarateCargo(int count, int currentStationId)
    {
        cargoList = new List<Cargo>();
        List<GameObject> stations = GameObject.FindGameObjectsWithTag("Station").ToList();

        List<GameObject> stationsForDelivery = new List<GameObject>();
        foreach (GameObject station in stations)
        {
            if (station.GetComponent<Station>().getId() != currentStationId)
            {
                stationsForDelivery.Add(station);
            }
            else
            {
                currentStation = station;
            }
        }

        for (int i = 0; i < count; i++)
        {
            cargoList.Add(new Cargo(stationsForDelivery[Random.Range(0, stationsForDelivery.Count())],
                                                        Random.Range(0, 100),
                                                        currentStation));
        }
    }


    public void chooseCargo(int id)
    {
        setCargo(cargoList[id]);
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
