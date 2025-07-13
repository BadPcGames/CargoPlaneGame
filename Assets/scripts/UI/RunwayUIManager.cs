using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Collections;
using UnityEngine.UI;
using Assets.scripts.PlaneController;

namespace HeneGames.Airplane
{
    public class RunwayUIManager : MonoBehaviour
    {
        [SerializeField] private Runway runway;
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private GameObject uiContent;
        [SerializeField] private GameObject player;

        [SerializeField] private GameObject cargoCardPrefab;
        [SerializeField] private Transform cargoListContent;

        private GameObject planeUi;
        private StationStats station;


        private bool cargoGenerated = false;
        private bool cargoUnloaded = false;

        private void Awake()
        {
            player = GameObject.Find("player");
            station = gameObject.GetComponent<StationStats>();
            uiContent.SetActive(false);
            planeUi = GameObject.Find("PlaneControllCanvas");
        }

        private void OnEnable()
        {
            PlayerStats.OnAirplaneChanged += UpdateTarget;
        }

        private void OnDisable()
        {
            PlayerStats.OnAirplaneChanged -= UpdateTarget;
        }

        private void UpdateTarget(GameObject newPlane)
        {
            planeUi = newPlane.transform.Find("PlaneControllCanvas").gameObject;
        }

        public void OnLoadCargoButton(int index)
        {
            player.GetComponent<Cargomanager>().chooseCargo(index);
        }

        public void OnUnloadCargoButton()
        {
            debugText.text = player.GetComponent<PlayerStats>().tryUnloadPlane(station.transform);
        }

        private void Update()
        {
            if (runway.AirplaneIsLanding())
            {
                SetChildrenActive(planeUi, false);
                debugText.text = "";
            }
            else if (runway.AirplaneLandingCompleted())
            {
                uiContent.SetActive(true);
                if (!cargoGenerated)
                {
                    player.GetComponent<Cargomanager>().GanarateCargo(40, station.getId());
                    GenerateCargoCards();
                    cargoGenerated = true;
                }
            }
            else if (runway.AriplaneIsTakingOff())
            {
                SetChildrenActive(planeUi, true);
                uiContent.SetActive(false);
                cargoGenerated = false;
            }
            else
            {
                uiContent.SetActive(false);
            }
        }

        private void SetChildrenActive(GameObject parent, bool active)
        {
            foreach (Transform child in parent.transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        public void LetFly()
        {
            runway.TakeOf();
            planeUi.SetActive(true);
            uiContent.SetActive(false);
        }

        private void GenerateCargoCards()
        {
            foreach (Transform child in cargoListContent)
            {
                Destroy(child.gameObject);
            }

            var cargoList = player.GetComponent<Cargomanager>().getCargoList();

            for (int i = 0; i < cargoList.Count; i++)
            {
                int index = i;

                GameObject card = Instantiate(cargoCardPrefab, cargoListContent);
                Text descriptionText = card.transform.Find("Description").GetComponent<Text>();
                Text priceText = card.transform.Find("Price").GetComponent<Text>();
                Text distanceText = card.transform.Find("Distance").GetComponent<Text>();
                Text nameText = card.transform.Find("CargoName").GetComponent<Text>();
                Button acceptButton = card.transform.Find("AcceptButton").GetComponent<Button>();

                nameText.text = $"{cargoList[i].getName()}";
                descriptionText.text = $"Deliver the cargo {cargoList[i].getName()}, which requires {cargoList[i].getRequiredSpace()} space, to the station: {cargoList[i].getTarget().getId()}";
                priceText.text = $"Reward: {cargoList[i].getReword()}";
                distanceText.text = $"Distance: {cargoList[i].getDistance()}";

                bool canGetCargo = player.GetComponent<PlayerStats>().Plane.GetComponent<PlaneStats>().getCargoSpace() > cargoList[i].getRequiredSpace();
                if (!canGetCargo)
                {
                    descriptionText.text = descriptionText.text + "\nThe cargo is too large for your plane";
                }
                acceptButton.interactable = canGetCargo;

                acceptButton.onClick.AddListener(() => OnLoadCargoButton(index));
            }
        }



    }
}