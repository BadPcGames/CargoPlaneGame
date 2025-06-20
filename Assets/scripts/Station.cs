using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField]
    private int Id;
    [SerializeField]
    private string name;


    private Vector3 position;
    private List<Cargo> cargos;

    private void Start()
    {
        name = "Station" + Id;
    }

    public Transform getPosition()
    {
        return transform;
    }

    public int getId()
    {
        return Id;
    }



}

