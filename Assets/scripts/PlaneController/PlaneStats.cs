using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts.PlaneController
{
    public class PlaneStats:MonoBehaviour
    {
        [SerializeField] private string Name;
        [SerializeField] private int PlaneId;
        [SerializeField] private Sprite PlaneImage;
        [SerializeField] private float MaxHealth;
        [SerializeField] private float CargoSpace;
        [SerializeField] private string Descrition;
        [SerializeField] private float Price;
        [SerializeField] private bool Bought;

        public float health;

        private void Awake()
        {
            health=MaxHealth;
        }

        public void setBought(bool value)
        {
            Bought=value;
        }

        public void ResetHealth()
        {
            health = MaxHealth;
        }

        public string getName()
        {
            return Name;
        }

        public Sprite getPlaneImage()
        {
            return PlaneImage;
        }

        public int getPlaneId()
        {
            return PlaneId;
        }

        public void TakeDamage(float value)
        {
            if (health-value<0)
            {
                health = 0;
            }
            else
            {
                health-=value;
            }
        }

        public string getDescription()
        {
            return Descrition;
        }

        public float getHealth()
        {
            return health;
        }
        public float getMaxHealth()
        {
            return MaxHealth;
        }

        public void setCargoSpace(float value)
        {
            CargoSpace = value;
        }

        public float getCargoSpace()
        {
            return CargoSpace;
        }

        public float getPrice() { return Price; }

        public bool getBought()
        {
            return Bought;
        }

    }
}
