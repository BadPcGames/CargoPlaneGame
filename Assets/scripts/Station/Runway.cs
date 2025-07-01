using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.Airplane
{
    public class Runway : MonoBehaviour
    {
        private bool landingCompleted;
        private float landingSpeed;
        private SimpleAirPlaneController landingAirplaneController;
        private Vector3 landingAdjusterStartLocalPos;

        [Header("Input")]
        [SerializeField] private KeyCode launchKey = KeyCode.Space;

        [Header("Runway references")]
        public string runwayName = "Runway";
        [SerializeField] private LandingArea landingArea;
        public Transform landingAdjuster;
        [SerializeField] private Transform landingfinalPos;

        private void Start()
        {
            landingSpeed = 1f;
            landingAdjusterStartLocalPos = landingAdjuster.localPosition;
        }

        private void Update()
        {
            if(landingAirplaneController != null)
            {
                landingAirplaneController.transform.SetParent(landingAdjuster.transform);

                if(!landingCompleted)
                {
                    landingSpeed += Time.deltaTime;
                    landingAdjuster.localPosition = Vector3.Lerp(landingAdjuster.localPosition, landingfinalPos.localPosition, landingSpeed * Time.deltaTime);

                    float _distanceToLandingFinalPos = Vector3.Distance(landingAdjuster.position, landingfinalPos.position);
                    if (_distanceToLandingFinalPos < 0.1f)
                    {
                        landingCompleted = true;
                    }
                }
                else
                {
                    landingAdjuster.localPosition = Vector3.Lerp(landingAdjuster.localPosition, landingfinalPos.localPosition, landingSpeed * Time.deltaTime);

                    if (landingAirplaneController.airplaneState == SimpleAirPlaneController.AirplaneState.Flying)
                    {
                        landingAirplaneController.transform.SetParent(null);
                        landingAirplaneController = null;
                        landingCompleted = false;
                        landingSpeed = 1f;
                        landingAdjuster.localPosition = landingAdjusterStartLocalPos;
                    }
                }
            }
        }

        public void TakeOf()
        {
            if(landingAirplaneController != null && landingCompleted)
            {
                landingAirplaneController.airplaneState = SimpleAirPlaneController.AirplaneState.Takeoff;
            }
        }

        public void AddAirplane(SimpleAirPlaneController _simpleAirPlane)
        {
            landingAirplaneController = _simpleAirPlane;
        }

        public bool AirplaneLandingCompleted()
        {
            if (landingAirplaneController != null)
            {
                if (landingAirplaneController.airplaneState != SimpleAirPlaneController.AirplaneState.Takeoff)
                {
                    return landingCompleted;
                }
            }

            return false;
        }

        public bool AirplaneIsLanding()
        {
            if(landingAirplaneController != null && !landingCompleted)
            {
                return true;
            }

            return false;
        }

        public bool AriplaneIsTakingOff()
        {
            if (landingAirplaneController != null)
            {
                if(landingAirplaneController.airplaneState == SimpleAirPlaneController.AirplaneState.Takeoff)
                {
                    return true;
                }
            }

            return false;
        }
    }
}