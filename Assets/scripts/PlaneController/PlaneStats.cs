using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.PlaneController
{
    public class PlaneStats:MonoBehaviour
    {
        [SerializeField] private float MaxHealth;
        [SerializeField] private float CargoSpace;

        private float health;

        private void Awake()
        {
            health=MaxHealth;
        }

        public void ResetHealth()
        {
            health = MaxHealth;
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


    }
}
