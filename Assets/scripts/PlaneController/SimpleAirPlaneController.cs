﻿using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.scripts.PlaneController;

namespace HeneGames.Airplane
{
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleAirPlaneController : MonoBehaviour
    {
        public enum AirplaneState
        {
            Flying,
            Landing,
            Takeoff,
        }

        public Action crashAction;
        public Action explouse;

        #region Private variables

        private List<SimpleAirPlaneCollider> airPlaneColliders = new List<SimpleAirPlaneCollider>();


        private float debafs = 0f;
        private float maxSpeed = 0.6f;
        private float speedMultiplier;
        private float currentYawSpeed;
        private float currentPitchSpeed;
        private float currentRollSpeed;
        private float currentSpeed;
        private float currentEngineLightIntensity;
        private float currentEngineSoundPitch;
        private float lastEngineSpeed;

        private bool planeIsDead;

        private Rigidbody rb;
        private Runway currentRunway;

        //Input variables
        protected float inputH;
        protected float inputV;
        protected bool inputTurbo;
        protected bool inputYawLeft;
        protected bool inputYawRight;
        protected float trustProcent=0.8f;

        private GameObject PlaneUi;

        #endregion

        public AirplaneState airplaneState;

        [Header("Control Mode")]
        [SerializeField]
        protected bool isAIPlane = false;
        [SerializeField]
        protected Vector3 target;
        [SerializeField]
        private bool stabilisation;

        [Header("Particles")]
        [SerializeField] private List<GameObject> engineHeatEffects;

        [Header("Wing trail effects")]
        [Range(0.01f, 1f)]
        [SerializeField] private float trailThickness = 0.045f;
        [SerializeField] private TrailRenderer[] wingTrailEffects;

        [Header("Rotating speeds")]
        [Range(5f, 500f)]
        [SerializeField] private float yawSpeed = 50f;

        [Range(5f, 500f)]
        [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)]
        [SerializeField] private float rollSpeed = 200f;

        [Header("Rotating speeds multiplers when turbo is used")]
        [Range(0.1f, 5f)]
        [SerializeField] private float yawTurboMultiplier = 0.3f;

        [Range(0.1f, 5f)]
        [SerializeField] private float pitchTurboMultiplier = 0.5f;

        [Range(0.1f, 5f)]
        [SerializeField] private float rollTurboMultiplier = 1f;

        [Header("Moving speed")]
        [Range(5f, 800f)]
        [SerializeField] private float defaultSpeed = 10f;

        [Range(0.1f, 50f)]
        [SerializeField] private float accelerating = 10f;

        [Range(0.1f, 50f)]
        [SerializeField] private float deaccelerating = 5f;

        [Header("Turbo settings")]
        [Range(0f, 100f)]
        [SerializeField] private float turboHeatingSpeed;

        [Range(0f, 100f)]
        [SerializeField] private float turboCooldownSpeed;

        [Header("Turbo heat values")]
        [Tooltip("Real-time information about the turbo's current temperature (do not change in the editor)")]
        [Range(0f, 100f)]
        [SerializeField] private float turboHeat;

        [Tooltip("You can set this to determine when the turbo should cease overheating and become operational again")]
        [Range(0f, 100f)]
        [SerializeField] private float turboOverheatOver;

        [SerializeField] private bool turboOverheat;

        [Header("Sideway force")]
        [Range(0.1f, 15f)]
        [SerializeField] private float sidewaysMovement = 15f;

        [Range(0.001f, 0.05f)]
        [SerializeField] private float sidewaysMovementXRot = 0.012f;

        [Range(0.1f, 5f)]
        [SerializeField] private float sidewaysMovementYRot = 1.5f;

        [Range(-1, 1f)]
        [SerializeField] private float sidewaysMovementYPos = 0.1f;

        [Header("Engine sound settings")]
        [SerializeField] private AudioSource engineSoundSource;
        [SerializeField] private List<SoundEn> engineSounds;

        [SerializeField] private float maxEngineSound = 1f;

        [SerializeField] private float defaultSoundPitch = 1f;

        [SerializeField] private float turboSoundPitch = 1.5f;

        [Header("Engine propellers settings")]
        [Range(10f, 10000f)]
        [SerializeField] private float propelSpeedMultiplier = 100f;

        [SerializeField] private GameObject[] propellers;

        [Header("Turbine light settings")]
        [Range(0.1f, 20f)]
        [SerializeField] private float turbineLightDefault = 1f;

        [Range(0.1f, 20f)]
        [SerializeField] private float turbineLightTurbo = 5f;

        [SerializeField] private Light[] turbineLights;

        [Header("Colliders")]
        [SerializeField] private Transform crashCollidersRoot;

        [Header("Takeoff settings")]
        [Tooltip("How far must the plane be from the runway before it can be controlled again")]
        [SerializeField] private float takeoffLenght = 30f;

        [Header("Crush Sound")]
        [SerializeField] private AudioClip danger;
        public GameObject explosionEffectPrefab;



        private float turboSpeed;

        protected virtual void Start()
        {
            //Setup speeds
            maxSpeed = defaultSpeed;
            currentSpeed = defaultSpeed*trustProcent;
            ChangeSpeedMultiplier(1f);
            PlaneUi = GameObject.Find("PlaneControllCanvas");
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            RestartPlane();
            SetupColliders(crashCollidersRoot);
        }

        private void Update()
        {
            AudioSystem();
            HandleInputs();

            switch (airplaneState)
            {
                case AirplaneState.Flying:
                    FlyingUpdate();
                    break;

                case AirplaneState.Landing:
                    LandingUpdate();
                    break;

                case AirplaneState.Takeoff:
                    TakeoffUpdate();
                    break;
            }
        }

        #region Flying State

        private void FlyingUpdate()
        {
            UpdatePropellersAndLights();
            recalculateDebafs();
            //Airplane move only if not dead
            if (!planeIsDead)
            {
                Movement();
                SidewaysForceCalculation();
            }
            else
            {
                ChangeWingTrailEffectThickness(0f);
            }

            if( HitSometing())
            {
                Explouse();
            }
        }

        private void SidewaysForceCalculation()
        {
            float _mutiplierXRot = sidewaysMovement * sidewaysMovementXRot;
            float _mutiplierYRot = sidewaysMovement * sidewaysMovementYRot;

            float _mutiplierYPos = sidewaysMovement * sidewaysMovementYPos;

            //Right side 
            if (transform.localEulerAngles.z > 270f && transform.localEulerAngles.z < 360f)
            {
                float _angle = (transform.localEulerAngles.z - 270f) / (360f - 270f);
                float _invert = 1f - _angle;

                transform.Rotate(Vector3.up * (_invert * _mutiplierYRot) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

                transform.Translate(transform.up * (_invert * _mutiplierYPos) * Time.deltaTime);
            }

            //Left side
            if (transform.localEulerAngles.z > 0f && transform.localEulerAngles.z < 90f)
            {
                float _angle = transform.localEulerAngles.z / 90f;

                transform.Rotate(-Vector3.up * (_angle * _mutiplierYRot) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);

                transform.Translate(transform.up * (_angle * _mutiplierYPos) * Time.deltaTime);
            }

            //Right side down
            if (transform.localEulerAngles.z > 90f && transform.localEulerAngles.z < 180f)
            {
                float _angle = (transform.localEulerAngles.z - 90f) / (180f - 90f);
                float _invert = 1f - _angle;

                transform.Translate(transform.up * (_invert * _mutiplierYPos) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_invert * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
            }

            //Left side down
            if (transform.localEulerAngles.z > 180f && transform.localEulerAngles.z < 270f)
            {
                float _angle = (transform.localEulerAngles.z - 180f) / (270f - 180f);

                transform.Translate(transform.up * (_angle * _mutiplierYPos) * Time.deltaTime);
                transform.Rotate(Vector3.right * (-_angle * _mutiplierXRot) * currentPitchSpeed * Time.deltaTime);
            }
        }

        private void Movement()
        {
            //Move forward
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            currentSpeed = defaultSpeed * trustProcent * (1-debafs);
            turboSpeed = currentSpeed * 1.5f;
            
       
            //Store last speed
            lastEngineSpeed = currentSpeed;

            //Rotate airplane by inputs
            transform.Rotate(Vector3.forward * -inputH * currentRollSpeed * Time.deltaTime);
            transform.Rotate(Vector3.right * inputV * currentPitchSpeed * Time.deltaTime);

            //Rotate yaw
            if (inputYawRight)
            {
                transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
            }
            else if (inputYawLeft)
            {
                transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
            }

            //Turbo
            if (inputTurbo && !turboOverheat)
            {
                //Turbo overheating
                if(turboHeat > 100f)
                {
                    turboHeat = 100f;
                    turboOverheat = true;
                    OverHeatParticles(true);
                }
                else
                {       
                    //Add turbo heat
                    turboHeat += Time.deltaTime * turboHeatingSpeed;
                }

                //Set speed to turbo speed and rotation to turbo values
                maxSpeed = turboSpeed;
                currentSpeed = turboSpeed;
                currentYawSpeed = yawSpeed * yawTurboMultiplier;
                currentPitchSpeed = pitchSpeed * pitchTurboMultiplier;
                currentRollSpeed = rollSpeed * rollTurboMultiplier;

                //Engine lights
                currentEngineLightIntensity = turbineLightTurbo;

                //Effects
                ChangeWingTrailEffectThickness(trailThickness);

                //Audio
                currentEngineSoundPitch = turboSoundPitch;
            }
            else
            {
                //Turbo cooling down
                if(turboHeat > 0f)
                {
                    turboHeat -= Time.deltaTime * turboCooldownSpeed;
                }
                else
                {
                    turboHeat = 0f;
                }

                //Turbo cooldown
                if (turboOverheat)
                {
                   if(turboHeat <= turboOverheatOver)
                   {
                        turboOverheat = false;
                        OverHeatParticles(false);
                   }
                }

                //Speed and rotation normal
                maxSpeed = defaultSpeed * speedMultiplier;

                currentYawSpeed = yawSpeed;
                currentPitchSpeed = pitchSpeed;
                currentRollSpeed = rollSpeed;

                //Engine lights
                currentEngineLightIntensity = turbineLightDefault;

                //Effects
                ChangeWingTrailEffectThickness(0f);

                //Audio
                currentEngineSoundPitch = defaultSoundPitch;
            }

            if (stabilisation)
            {
                Vector3 localEuler = transform.localEulerAngles;

                // Преобразование в диапазон -180..180
                localEuler.x = (localEuler.x > 180) ? localEuler.x - 360 : localEuler.x;
                localEuler.z = (localEuler.z > 180) ? localEuler.z - 360 : localEuler.z;

                float autoLevelStrength = 0.5f;

                float pitchCorrection = -localEuler.x * autoLevelStrength * Time.deltaTime;
                float rollCorrection = -localEuler.z * autoLevelStrength * Time.deltaTime;

                transform.Rotate(pitchCorrection, 0f, rollCorrection, Space.Self);
            }
        }

        #endregion

        #region Landing State

        public void AddLandingRunway(Runway _landingThisRunway)
        {
            currentRunway = _landingThisRunway;
        }

        //My trasform is runway landing adjuster child
        private void LandingUpdate()
        {
            UpdatePropellersAndLights();

            ChangeWingTrailEffectThickness(0f);

            //Stop speed
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime);

            //Set local rotation to zero
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f,0f,0f), 2f * Time.deltaTime);

            RestartPlane();
            
        }

        #endregion

        #region Takeoff State

        private void TakeoffUpdate()
        {
            UpdatePropellersAndLights();

            //Reset colliders
            foreach (SimpleAirPlaneCollider _airPlaneCollider in airPlaneColliders)
            {
                _airPlaneCollider.collideSometing = false;
            }

            //Accelerate
            if (currentSpeed < defaultSpeed)
            {
                currentSpeed += (accelerating * 4f) * Time.deltaTime;
            }

            //Move forward
            transform.Translate((Vector3.forward+new Vector3(0,0.3f,0)) * currentSpeed * Time.deltaTime);

            //Far enough from the runaway go back to flying state
            float _distanceToRunway = Vector3.Distance(transform.position, currentRunway.transform.position);
            if(_distanceToRunway > takeoffLenght)
            {
                currentRunway = null;
                airplaneState = AirplaneState.Flying;
            }
        }

        #endregion

        #region Audio
        private void AudioSystem()
        {
            if (engineSoundSource == null)
                return;

            if (airplaneState == AirplaneState.Flying)
            {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, currentEngineSoundPitch, 10f * Time.deltaTime);

                if (planeIsDead&&!isAIPlane)
                {
                    var sound = danger;
                    if (engineSoundSource.clip != sound)
                    {
                        engineSoundSource.clip = sound;
                        engineSoundSource.loop = true;
                        engineSoundSource.Play();
                    }
                    engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound/2, 10f * Time.deltaTime);
                }
                else
                {
                    foreach (var sounds in engineSounds)
                    {
                        var sound = sounds.GetComponent<SoundEn>();
                        float speedRatio = currentSpeed / maxSpeed;
                        if (sound.start < speedRatio && sound.end >= speedRatio && engineSoundSource.clip != sound.AudioClip)
                        {
                            engineSoundSource.clip = sound.AudioClip;
                            engineSoundSource.loop = true;
                            engineSoundSource.Play();
                        }
                    }
                    engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
                }
            }
            else if (airplaneState == AirplaneState.Landing)
            {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, defaultSoundPitch, 1f * Time.deltaTime);
                engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, 1f * Time.deltaTime);
            }
            else if (airplaneState == AirplaneState.Takeoff)
            {
                engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, turboSoundPitch, 1f * Time.deltaTime);
                engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, maxEngineSound, 1f * Time.deltaTime);
            }
        }

        #endregion

        #region Private methods

        private void RestartPlane()
        {
            gameObject.GetComponent<PlaneStats>().ResetHealth();
            turboOverheat = false;
            turboHeat = 0;
            OverHeatParticles(false);
            recalculateDebafs();
        }

        private void OverHeatParticles(bool state)
        {
            foreach(GameObject par in engineHeatEffects)
            {
                if (state)
                {
                    par.GetComponent<ParticleSystem>().Play();
                }
                else
                {
                    par.GetComponent<ParticleSystem>().Stop();
                }
               
            }
        }

        private void UpdatePropellersAndLights()
        {
            if(!planeIsDead)
            {
                //Rotate propellers if any
                if (propellers.Length > 0)
                {
                    RotatePropellers(propellers, currentSpeed * propelSpeedMultiplier);
                }

                //Control lights if any
                if (turbineLights.Length > 0)
                {
                    ControlEngineLights(turbineLights, currentEngineLightIntensity);
                }
            }
            else
            {
                //Rotate propellers if any
                if (propellers.Length > 0)
                {
                    RotatePropellers(propellers, 0f);
                }

                //Control lights if any
                if (turbineLights.Length > 0)
                {
                    ControlEngineLights(turbineLights, 0f);
                }
            }
        }

        private void SetupColliders(Transform _root)
        {
            if (_root == null)
                return;

            Collider[] colliders = _root.GetComponentsInChildren<Collider>();

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = true;

                GameObject _currentObject = colliders[i].gameObject;

                SimpleAirPlaneCollider _airplaneCollider = _currentObject.AddComponent<SimpleAirPlaneCollider>();
                airPlaneColliders.Add(_airplaneCollider);

                _airplaneCollider.controller = this;

                Rigidbody _rb = _currentObject.AddComponent<Rigidbody>();
                _rb.useGravity = false;
                _rb.isKinematic = true;
                _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            }
        }

        private void RotatePropellers(GameObject[] _rotateThese, float _speed)
        {
            for (int i = 0; i < _rotateThese.Length; i++)
            {
                _rotateThese[i].transform.Rotate(Vector3.forward * -_speed * Time.deltaTime);
            }
        }

        private void ControlEngineLights(Light[] _lights, float _intensity)
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                if(!planeIsDead)
                {
                    _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, _intensity, 10f * Time.deltaTime);
                }
                else
                {
                    _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, 0f, 10f * Time.deltaTime);
                }
               
            }
        }

        private void ChangeWingTrailEffectThickness(float _thickness)
        {
            for (int i = 0; i < wingTrailEffects.Length; i++)
            {
                wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);
            }
        }

        private bool HitSometing()
        {
            for (int i = 0; i < airPlaneColliders.Count; i++)
            {
                if (airPlaneColliders[i].collideSometing)
                {
                    foreach(SimpleAirPlaneCollider _airPlaneCollider in airPlaneColliders)
                    {
                        _airPlaneCollider.collideSometing = false;
                    }

                    return true;
                }
            }

            return false;
        }

        private void recalculateDebafs()
        {
            float health = gameObject.GetComponent<PlaneStats>().getHealth();
            float maxHealth = gameObject.GetComponent<PlaneStats>().getMaxHealth();
            debafs = 1-(health / maxHealth);
            if (health == 0)
            {
                Crash();
            }
        }

        #endregion

        #region Public methods

        public float getPitch()
        {
            return inputV;
        }

        public bool GetStabilisation()
        {
            return stabilisation;
        }

        public Runway GetCurrentRunway()
        {
            return currentRunway;
        }

        public float getRoll()
        {
            return inputH;
        }

        public float getTrustProcent()
        {
            return trustProcent;
        }

        public float getDefaultSpeed()
        {
            return defaultSpeed;
        }

        public float getCurrentSpeed()
        {
            return currentSpeed;
        }

        public virtual void Crash()
        {
            crashAction?.Invoke();

            //Set rigidbody to non cinematic
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            rb.AddForce(transform.forward * lastEngineSpeed*0.005f, ForceMode.VelocityChange);

            planeIsDead = true;      
        }

        private void Explouse()
        {
            if (!planeIsDead)
            {
                Crash();
            }

            for (int i = 0; i < airPlaneColliders.Count; i++)
            {
                airPlaneColliders[i].GetComponent<Collider>().isTrigger = false;
                Destroy(airPlaneColliders[i].GetComponent<Rigidbody>());
            }
            if (explosionEffectPrefab != null)
            {
                GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 5f); 
            }

            explouse?.Invoke();
            Destroy(gameObject);
        }



        #endregion

        #region Variables

        /// <summary>
        /// Returns a percentage of how fast the current speed is from the maximum speed between 0 and 1
        /// </summary>
        /// <returns></returns>
        public float PercentToMaxSpeed()
        {
            float _percentToMax = (currentSpeed * speedMultiplier) / turboSpeed;

            return _percentToMax;
        }

        public bool PlaneIsDead()
        {
            return planeIsDead;
        }

        public bool UsingTurbo()
        {
            if(maxSpeed == turboSpeed)
            {
                return true;
            }

            return false;
        }

        public float CurrentSpeed()
        {
            return currentSpeed * speedMultiplier;
        }

        /// <summary>
        /// Returns a turbo heat between 0 and 100
        /// </summary>
        /// <returns></returns>
        public float TurboHeatValue()
        {
            return turboHeat;
        }

        public bool TurboOverheating()
        {
            return turboOverheat;
        }

        /// <summary>
        /// With this you can adjust the default speed between one and zero
        /// </summary>
        /// <param name="_speedMultiplier"></param>
        public void ChangeSpeedMultiplier(float _speedMultiplier)
        {
            if(_speedMultiplier < 0f)
            {
                _speedMultiplier = 0f;
            }

            if(_speedMultiplier > 1f)
            {
                _speedMultiplier = 1f;
            }

            speedMultiplier = _speedMultiplier;
        }

        #endregion

        #region Inputs

        private float trustChangeSpeed = 0.3f;

        protected virtual float GetInputHorizontal() => Input.GetAxisRaw("Horizontal");
        protected virtual float GetInputVertical() => Input.GetAxisRaw("Vertical");
        protected virtual bool GetInputTurbo() => Input.GetButton("Turbo");
        protected virtual bool GetInputYawLeft() => Input.GetButton("YawLeft");
        protected virtual bool GetInputYawRight() => Input.GetButton("YawRight");

        private void HandleInputStabilisation()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                stabilisation = !stabilisation;
                Debug.Log("Stabilisation toggled: " + stabilisation);
            }
        }

        protected virtual float GetTrustDelta()
        {
            float delta = 0f;
            if (Input.GetKey(KeyCode.Space)) delta += trustChangeSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftControl)) delta -= trustChangeSpeed * Time.deltaTime;
            return delta;
        }

        protected float minDistanceForSlowdown = 1f;
        protected float controlAngleThreshold = 0.001f;

        protected virtual void HandleInputs()
        {
            if (!isAIPlane)
            {
                inputH = GetInputHorizontal();
                inputV = GetInputVertical();
                inputYawLeft = GetInputYawLeft();
                inputYawRight = GetInputYawRight();
                inputTurbo = GetInputTurbo();
                trustProcent += GetTrustDelta();
                trustProcent = Mathf.Clamp(trustProcent, 0.2f, 1f);
                HandleInputStabilisation();
            }
            else
            {
                AiHandler();
                Fire();
            }
        }

        #endregion

        #region AI (стаблонные методы, могут быть переопределены)

        [SerializeField] protected LayerMask obstacleMask = ~0;
        [SerializeField] protected float detectDistance = 100f;

        protected virtual void AiHandler()
        {
            // Пустой шаблон — реализуется в подклассе AI
        }

        protected virtual Vector3 ComputeDesiredDirection()
        {
            // Пустой шаблон — реализуется в подклассе AI
            return transform.forward;
        }

        // Стрелять — виртуальный метод для AI
        protected virtual void Fire()
        {
        
        }

   

        #endregion
    }
}