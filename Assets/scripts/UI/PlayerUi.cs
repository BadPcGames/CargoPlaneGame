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
