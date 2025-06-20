using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static HeneGames.Airplane.SimpleAirPlaneController;

namespace HeneGames.Airplane
{
    public class SimpleAirplaneCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SimpleAirPlaneController airPlaneController;
        [SerializeField] private Camera cam;

        [Header("Camera values")]
        [SerializeField] private float cameraDefaultFov = 60f;
        [SerializeField] private float cameraTurboFov = 40f;

        [Header("Camera positions")]
        [SerializeField] List<Transform> cameraPositions;

        private bool isCrash=false;
        private int currentPosition;

        private void Awake()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            cam.transform.SetParent(transform, false);
            cam.transform.position = cameraPositions[0].position;
            cam.transform.rotation = cameraPositions[0].rotation;
        }

        private void OnEnable()
        {
            airPlaneController.crashAction += Crash;
        }

        private void OnDisable()
        {
            airPlaneController.crashAction -= Crash;
        }

        private void Update()
        {
            CameraFovUpdate();
            CameraPositionUpdate();

            if (isCrash) 
            {
                DeathCam();
            }
        }

        private void CameraPositionUpdate()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                currentPosition++;

                if (currentPosition >= cameraPositions.Count)
                {
                    currentPosition = 0;
                }

                cam.transform.position = cameraPositions[currentPosition].position;
                cam.transform.rotation = cameraPositions[currentPosition].rotation;
            }
        }

        private void CameraFovUpdate()
        {
            if(!airPlaneController.PlaneIsDead() && airPlaneController.airplaneState == AirplaneState.Flying)
            {
                if (Input.GetKey(KeyCode.LeftShift) && !airPlaneController.TurboOverheating())
                {
                    ChangeCameraFov(cameraTurboFov);
                }
                else
                {
                    ChangeCameraFov(cameraDefaultFov);
                }
            }
            else
            {
                ChangeCameraFov(cameraDefaultFov);
            }
        }

        public void ChangeCameraFov(float _fov)
        {
            float _deltatime = Time.deltaTime * 100f;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _fov, 0.05f * _deltatime);
        }

        private void Crash()
        {
            isCrash = true;
        }



        private void DeathCam()
        {
            cam.transform.SetParent(null, false);
            cam.transform.rotation = Quaternion.Euler(90, 0, 0);
            cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, transform.position.x,0.1f),
               Mathf.Lerp(cam.transform.position.y, transform.position.y+20, 0.1f),
               Mathf.Lerp(cam.transform.position.z, transform.position.z, 0.1f)
               );
        }
    }
}