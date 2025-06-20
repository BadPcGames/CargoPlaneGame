using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Collections;
using UnityEngine.UI;

namespace HeneGames.Airplane
{
    public class RunwayUIManager : MonoBehaviour
    {
        [SerializeField] private Runway runway;
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private GameObject uiContent;
        [SerializeField] private GameObject player;

        [SerializeField] private GameObject cargoCardPrefab; // Префаб карточки
        [SerializeField] private Transform cargoListContent; // Контейнер контента

        private GameObject planeUi;
        private Station station;


        private bool cargoGenerated = false;
        private bool cargoUnloaded = false;

        private void Awake()
        {
            player = GameObject.Find("player");
            station = gameObject.GetComponent<Station>();
            uiContent.SetActive(false);
            planeUi = GameObject.Find("PlaneControllCanvas");
        }

        public void OnLoadCargoButton(int index)
        {
            debugText.text = player.GetComponent<PlayerStats>().LoadCargoToPlane(index);
        }

        public void OnUnloadCargoButton()
        {
           debugText.text= player.GetComponent<PlayerStats>().tryUnloadPlane(station.transform);
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
                    player.GetComponent<PlayerStats>().GanarateCargo(20, station.getId());
                    GenerateCargoCards(); 
                    cargoGenerated = true;
                    Debug.Log("[RunwayUI] Cargos generated");
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
            // Очистить старые карточки
            foreach (Transform child in cargoListContent)
            {
                Destroy(child.gameObject);
            }

            var cargoList = player.GetComponent<PlayerStats>().getCargoList();

            for (int i = 0; i < cargoList.Count; i++)
            {
                int index = i; // Копия для замыкания

                GameObject card = Instantiate(cargoCardPrefab, cargoListContent);
                Text descriptionText = card.transform.Find("Description").GetComponent<Text>();
                Button acceptButton = card.transform.Find("AcceptButton").GetComponent<Button>();

                // Настроить описание (можно изменить как угодно)
                descriptionText.text = $"Цель: {cargoList[i].getTarget().getId()}, Награда: {cargoList[i].getReword()}";

                acceptButton.onClick.AddListener(() => OnLoadCargoButton(index));
            }
        }



    }
}