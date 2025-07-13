using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    public GameObject box;
    public TextMeshProUGUI moneyText;

    private bool haveCargo;
    private float money;

    private void Update()
    {
        HomeWaipointManager();
        CargoManager();
        MoneyManager();
        StationWaipointManager();
    }

    public void setHaveCargo(bool value)
    {
        haveCargo = value;
    }

    public void setMoney(float value)
    {
       money = value;
    }

    private void CargoManager()
    {
        box.SetActive(haveCargo);
    }

    private void MoneyManager()
    {
        moneyText.text = money + " $";
    }

    private void HomeWaipointManager()
    {
        if (Input.GetKeyDown(KeyCode.H))
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

    int idOfStation=-1;
    GameObject[] stationsList;

    private void StationWaipointManager()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            transform.Find("WaipointToBase").GetComponent<WaypointMarker>().setTarget(null);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (stationsList == null)
            {
                stationsList = GameObject.FindGameObjectsWithTag("Station");
            }

            idOfStation++;

            if (idOfStation > stationsList.Length - 1)
            {
                idOfStation = stationsList.Length - 1;
            }

            transform.Find("WaipointToBase").GetComponent<WaypointMarker>().setTarget(stationsList[idOfStation].transform);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (stationsList == null)
            {
                stationsList = GameObject.FindGameObjectsWithTag("Station");
            }

            idOfStation--;
            if (idOfStation < 0)
            {
                idOfStation = 0;
            }

            transform.Find("WaipointToBase").GetComponent<WaypointMarker>().setTarget(stationsList[idOfStation].transform);
        }
    }
}
