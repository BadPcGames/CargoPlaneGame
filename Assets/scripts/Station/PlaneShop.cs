using Assets.scripts.PlaneController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.Station
{
    public class PlaneShop:MonoBehaviour
    {
        private PlayerStats player;

        public PlaneShop()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }

        public bool tryBuyPlane(PlaneStats plane)
        {
            if (plane.getPrice() <= player.getMoney())
            {
                player.setMoney(player.getMoney()-plane.getPrice());
                return true;
            }
            return false;
        }

    }
}
