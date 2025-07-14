using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.scripts.PlaneController;
using System.Collections.Generic;
using System.Linq;


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
        private List<Cargo> cargoList;

        public delegate void CameraRotationAvailable(bool rotationAvailable);
        public static event CameraRotationAvailable OnCameraRotationAvailableChanges;

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

        public void OnSortByValue(bool fromLowToMax)
        {
            SortByValue(fromLowToMax);
        }

        public void OnSortBySpace(bool fromLowToMax)
        {
            SortBySpace(fromLowToMax);
        }

        public void OnUnloadCargoButton()
        {
            debugText.text = player.GetComponent<PlayerStats>().tryUnloadPlane(station.transform);
        }

        public void OnRemoveCargoButton()
        {
            debugText.text = player.GetComponent<PlayerStats>().tryRemoveCargo();
        }

        private void Update()
        {
            if (runway.AirplaneIsLanding())
            {
                SetChildrenActive(planeUi, false);
                debugText.text = "";
                OnCameraRotationAvailableChanges?.Invoke(false);
            }
            else if (runway.AirplaneLandingCompleted())
            {
                uiContent.SetActive(true);
                if (!cargoGenerated)
                {
                    player.GetComponent<Cargomanager>().GanarateCargo(40, station.getId());
                    cargoList= player.GetComponent<Cargomanager>().getCargoList();
                    ShowCargoCards();
                    cargoGenerated = true;
                }
            }
            else if (runway.AriplaneIsTakingOff())
            {
                SetChildrenActive(planeUi, true);
                uiContent.SetActive(false);
                cargoGenerated = false;
                OnCameraRotationAvailableChanges?.Invoke(true);
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

        private void ShowCargoCards()
        {
            foreach (Transform child in cargoListContent)
            {
                Destroy(child.gameObject);
            }

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

        private void SortByValue(bool fromLowToMax)
        {

            if (fromLowToMax)
            {
                cargoList = cargoList.OrderBy(w => w.getReword()).ToList();
            }
            else
            {
                cargoList = cargoList.OrderByDescending(w => w.getReword()).ToList();
            }
            ShowCargoCards();
        }

        private void SortBySpace(bool fromLowToMax)
        {
            if (fromLowToMax)
            {
                cargoList = cargoList.OrderBy(w => w.getRequiredSpace()).ToList();
            }
            else
            {
                cargoList = cargoList.OrderByDescending(w => w.getRequiredSpace()).ToList();
            }
            ShowCargoCards();
        }


    }
}