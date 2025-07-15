using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static HeneGames.Airplane.SimpleAirPlaneController;
using Unity.VisualScripting;

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
        [SerializeField] private float sensivity = 10f;

        [Header("Camera positions")]
        [SerializeField] List<Transform> cameraPositions;

        public float maxYAngle = 80f;

        private Vector2 currentRotation;
        private int currentPosition;
        private bool cameaFreeMode=false;

        private void Awake()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            cam.transform.SetParent(transform, false);
            cam.transform.position = cameraPositions[0].position;
            cam.transform.rotation = cameraPositions[0].rotation;
        }

        private void OnEnable()
        {
            airPlaneController.explouse += Explouse;
            HomeRunwayUIManager.OnCameraRotationAvailableChanges += ChangeCameraMod;
            RunwayUIManager.OnCameraRotationAvailableChanges += ChangeCameraMod;
        }

        private void OnDisable()
        {
            airPlaneController.explouse -= Explouse;
            HomeRunwayUIManager.OnCameraRotationAvailableChanges -= ChangeCameraMod;
            RunwayUIManager.OnCameraRotationAvailableChanges -= ChangeCameraMod;
        }

        private void Update()
        {
            CameraFovUpdate();
            CameraPositionUpdate();
            if(cameaFreeMode)
            CameraRotate();
        }

        private void Explouse()
        {
            cam.gameObject.transform.SetParent(null);
        }
    
        private void CameraRotate()
        {
            currentRotation.x += Input.GetAxis("Mouse X") *sensivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * sensivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            Quaternion baseRotation = transform.rotation;
            Quaternion mouseRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
            Camera.main.transform.rotation = baseRotation * mouseRotation;

            
            if (Input.GetMouseButtonDown(2))
            {
                ResetCameraRotation();
            }
        }

        private void ChangeCameraMod(bool mode)
        {
            if (!mode)
            {
                ResetCameraRotation();
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            cameaFreeMode= mode;
        }

        private void ResetCameraRotation()
        {
            currentRotation = new Vector2();
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
    }
}