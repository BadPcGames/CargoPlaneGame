using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Collections;
using UnityEngine.UI;
using Assets.scripts.PlaneController;
using System.Collections.Generic;
using Assets.scripts.Station;

namespace HeneGames.Airplane
{
    public class HomeRunwayUIManager : MonoBehaviour
    {
        [SerializeField] private Runway runway;
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private GameObject uiContent;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject planeCardPrefab;
        [SerializeField] private Transform planeListContent;
        [SerializeField] List<GameObject> planes;

        private GameObject planeUi;
        private PlaneShop PlaneShop;
        private bool cargoGenerated = false;

        public delegate void CameraRotationAvailable(bool rotationAvailable);
        public static event CameraRotationAvailable OnCameraRotationAvailableChanges;

        private void Awake()
        {
            player = GameObject.Find("player");
            uiContent.SetActive(false);
            planeUi = GameObject.Find("PlaneControllCanvas");
            PlaneShop = new PlaneShop();
        }

        private void FixedUpdate()
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
                    GeneratePlaneCards();
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
            uiContent.SetActive(true);
            planeUi = newPlane.transform.Find("PlaneControllCanvas").gameObject;
            SetChildrenActive(planeUi, false);
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

        private void GeneratePlaneCards()
        {
            foreach (Transform child in planeListContent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < planes.Count; i++)
            {
                int index = i;

                GameObject card = Instantiate(planeCardPrefab, planeListContent);
                Image planeImage=card.transform.Find("PlaneImage").GetComponent<Image>();
                Button acceptButton = card.transform.Find("AcceptButton").GetComponent<Button>();
                Button buyButton = card.transform.Find("BuyPlane").GetComponent<Button>();
                Text nameText = card.transform.Find("PlaneName").GetComponent<Text>();
                Text description=card.transform.Find("Description").GetComponent<Text>();
                Text price = card.transform.Find("Price").GetComponent<Text>();

                if (planes[index].GetComponent<PlaneStats>().getBought())
                {
                    buyButton.gameObject.SetActive(false);
                    price.gameObject.SetActive(false);
                }
                else
                {
                    acceptButton.gameObject.SetActive(false);
                }

                nameText.text = $"{planes[i].GetComponent<PlaneStats>().getName()}";
                description.text = $"{planes[i].GetComponent<PlaneStats>().getDescription()}";
                price.text= $"Price: {planes[i].GetComponent<PlaneStats>().getPrice()} $";
                planeImage.sprite = planes[i].GetComponent<PlaneStats>().getPlaneImage();
                acceptButton.onClick.AddListener(() => OnChoosePlaneButton(planes[index]));
                buyButton.onClick.AddListener(() => OnTryBuyPlaneButton(index));
            }
        }

        private void OnTryBuyPlaneButton(int index)
        {
            debugText.text = "";
            bool resultOfBuy = PlaneShop.tryBuyPlane(planes[index].GetComponent<PlaneStats>());
            if (!resultOfBuy)
            {
                debugText.text = "Noy enoght money";
            }
            planes[index].GetComponent<PlaneStats>().setBought(resultOfBuy);
            GeneratePlaneCards();
        }

        public void OnChoosePlaneButton(GameObject newPlane)
        {
            debugText.text = "";
            if (player.GetComponent<PlayerStats>().Plane.GetComponent<PlaneStats>().getPlaneId()!= newPlane.GetComponent<PlaneStats>().getPlaneId())
            {
                player.GetComponent<PlayerStats>().ChangePlane(newPlane);
            }
        }




    }
}