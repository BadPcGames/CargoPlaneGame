using UnityEngine;

public class MiniMapPosition : MonoBehaviour
{
    public GameObject plane;

    private void FixedUpdate()
    {
        transform.position=plane.transform.position+new Vector3(0,200,0);
    }
}
