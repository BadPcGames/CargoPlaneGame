using UnityEngine;

public class Cargo : MonoBehaviour
{
    private Station target;
    private float requiredSpace;
    private Station stationCurrent;
    
    
    private float distance;
    private float reword;


    public float getReword()
    {
        return reword;
    }

    public float getDistance()
    {
        return distance;
    }

    public Station getTarget()
    {
        return target;
    }





    public Cargo(GameObject t,float r,GameObject c)
    {
        target = t.GetComponent<Station>();
        requiredSpace = r;
        stationCurrent = c.GetComponent<Station>();

        distance = Vector3.Distance
            (c.transform.position, t.transform.position);
        reword = distance * requiredSpace;
    }

}
