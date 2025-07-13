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

        public delegate void AirplaneTakeChangeHealth(float health);
        public static event AirplaneTakeChangeHealth OnAirplaneChangedHealth;

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
            OnAirplaneChangedHealth.Invoke((100f * health) / MaxHealth);
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
            OnAirplaneChangedHealth.Invoke((100f * health) / MaxHealth);

            Debug.Log(health);
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
