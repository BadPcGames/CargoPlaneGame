using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class Cargomanager : MonoBehaviour
{
    private List<Cargo> cargoList;
    private GameObject currentStation;

    public void GanarateCargo(int count,int currentStationId)
    {
        cargoList = new List<Cargo>();
        List<GameObject> stations = GameObject.FindGameObjectsWithTag("Station").ToList();
     
        List<GameObject> stationsForDelivery=new List<GameObject>();
        foreach(GameObject station in stations)
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
       gameObject.GetComponent<PlayerStats>().setCargo(cargoList[id]);
    }

    public void tryUnloadPlane(Transform station)
    {

        if (gameObject.GetComponent<PlayerStats>().getCurrentCargo() != null&&
            Vector3.Distance(gameObject.transform.position,station.position)<10)
        {
            Debug.Log("Unload");
            gameObject.GetComponent<PlayerStats>().deliveredCargo();
        }

    }

    public void LoadCargoToPlane(int index)
    {
        Debug.Log("Try set cargo");

        if (gameObject == null || gameObject.GetComponent<PlayerStats>() == null)
        {
            Debug.LogError("Player or PlayerStats not found!");
        }

        var stats = gameObject.GetComponent<PlayerStats>();
        Debug.Log($"[LoadCargoToPlane] PlayerStats instance: {stats.GetInstanceID()} on {stats.gameObject.name}");

        stats.setCargo(cargoList[index]);

        if (stats.getCurrentCargo() == null)
        {
            Debug.Log("noo cargo");
        }
    }
}
