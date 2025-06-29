using Assets.scripts.Cargo;
using UnityEngine;

public class Cargo
{ 
    private Station target;
    private float requiredSpace;
    private Station stationCurrent;
    private string name;
    private float valueKoficient;
    private float distance;
    private float reword;
    private CargoNamesList cargoNamesList;


    public float getReword()
    {
        return reword;
    }

    public float getDistance()
    {
        return distance;
    }

    public string getName()
    {
        return name;
    }

    public Station getTarget()
    {
        return target;
    }

    public float getRequiredSpace()
    {
        return requiredSpace;
    }



    public Cargo(GameObject t,float r,GameObject c)
    {
        cargoNamesList = new CargoNamesList();
        int indexOfCargo = Random.RandomRange(0, cargoNamesList.CargoNames.Count);
        name = cargoNamesList.CargoNames[indexOfCargo].Name;
        valueKoficient = cargoNamesList.CargoNames[indexOfCargo].ValueKof;
        target = t.GetComponent<Station>();
        requiredSpace = r;
        stationCurrent = c.GetComponent<Station>();

        distance = Vector3.Distance
            (c.transform.position, t.transform.position);
        reword = distance * requiredSpace*valueKoficient;
    }

}
