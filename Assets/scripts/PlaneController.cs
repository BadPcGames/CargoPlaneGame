using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Rigidbody))]
public class PlaneController : MonoBehaviour
{
    public float trust = 1000;
    public float trust_multiplayer = 37;
    public float roll_multiplayer = 160;
    public float pitch_multiplayer = 160;
    public float yaw_multiplayer = 120f;
    public float liftCoefficient = 0.3f; 
    public float maxLift = 10000f;
    public float trustProcent = 0.0f;
    public float planeHieght=4;
    public float maxPropellerSpeed = 2000f; 
    public List<GameObject> propellers = new List<GameObject>();
    public List<GameObject> engineSounds = new List<GameObject>();


    private AudioSource engineSound;
    private Rigidbody rb;
    private float pitch;
    private float roll;
    private float yaw;
    private float actualTrust = 0;
    private float speed;


    public float getPitch()
    {
        return pitch;
    }

    public float getRoll()
    {
        return roll;
    }

    public float getTrustProcent()
    {
        return trustProcent;
    }

   public float getTrust()
    {
        return trust;
    }

    public float getActualTrust()
    {
        return actualTrust;
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        engineSound=GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        inputManager();
        planePhysics();
        rotatePropellers();
        soundManager();
    }

    private float trustChangeSpeed = 0.5f;

    private void soundManager()
    {
        float tachometerProcent = Mathf.Clamp01(actualTrust / trust * trustProcent);
        engineSound.volume = tachometerProcent * 0.1f+0.1f;
        if (tachometerProcent > 0)
        {
            foreach (var sounds in engineSounds)
            {
                var sound = sounds.GetComponent<SoundEn>();
                if (sound.start < tachometerProcent && sound.end >= tachometerProcent && engineSound.clip != sound.AudioClip)
                {
                    engineSound.clip = sound.AudioClip;
                    engineSound.loop = true;
                    engineSound.Play();
                }
            }
        }
        else
        {
            engineSound.clip = null;
        }
    }

    private void inputManager()
    {
        pitch = 0f;
        roll = 0f;
        yaw = 0f;

        if (Input.GetKey(KeyCode.W)) pitch = 1f;
        if (Input.GetKey(KeyCode.S)) pitch = -1f;

        if (Input.GetKey(KeyCode.D)) roll = 1f;
        if (Input.GetKey(KeyCode.A)) roll = -1f;

        if (Input.GetKey(KeyCode.E)) yaw = 1f;
        if (Input.GetKey(KeyCode.Q)) yaw = -1f;

        if (Input.GetKey(KeyCode.Space))
        {
            trustProcent += trustChangeSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            trustProcent -= trustChangeSpeed * Time.deltaTime;
        }

        trustProcent = Mathf.Clamp01(trustProcent);
        actualTrust = trustProcent * trust;
    }

    private void Update()
    {
        if (transform.transform.position.y > planeHieght)
        {
            ApplyCustomGravity();
        }
    }

    private void ApplyCustomGravity()
    {
        rb.AddForce(Vector3.down *9.81f * rb.mass); // сила тяжести
    }

    private void planePhysics()
    {
        float altitude = transform.position.y;
        float verticalSpeed = rb.linearVelocity.y;
        speed = rb.linearVelocity.magnitude;
        //Debug.Log(speed);

        float altitudeDragMultiplier = 1f - Mathf.Clamp(verticalSpeed / 100f, -0.3f, 0.3f);
        float adjustedThrust = actualTrust * trust_multiplayer * altitudeDragMultiplier * Time.deltaTime;
        rb.AddRelativeForce(Vector3.forward * adjustedThrust);


        if (speed > 0.1f)
        {
            float speedFactor = Mathf.Clamp01(speed); // нормализуем скорость

            // Ввод игрока
            Vector3 torqueInput = new Vector3(
                pitch * pitch_multiplayer,
                yaw * yaw_multiplayer*2,
                -roll * roll_multiplayer * 2
            ) * speedFactor * Time.deltaTime;

            rb.AddRelativeTorque(torqueInput);

            // Стабилизация: стремление к выравниванию по pitch и roll
            Quaternion currentRotation = transform.localRotation;
            Vector3 localEuler = currentRotation.eulerAngles;

            // Корректируем углы: от 0 до 360 -> от -180 до 180
            localEuler.x = (localEuler.x > 180) ? localEuler.x - 360 : localEuler.x;
            localEuler.z = (localEuler.z > 180) ? localEuler.z - 360 : localEuler.z;

            // Сила стабилизации (настраиваем по вкусу)
            float autoLevelStrength = 1f;

            Vector3 autoLevelTorque = new Vector3(
                -localEuler.x * autoLevelStrength,
                0,
                -localEuler.z * autoLevelStrength
            ) * speedFactor * Time.deltaTime;

            rb.AddRelativeTorque(autoLevelTorque);
        }


        if (speed > 1f)
        {
            Vector3 velocityDir = rb.linearVelocity.normalized;
            Vector3 wingRight = transform.right;
            Vector3 liftDir = Vector3.Cross(velocityDir, wingRight).normalized;

            float angleOfAttack = Vector3.Dot(transform.forward, velocityDir);
            float liftForce = Mathf.Clamp(liftCoefficient * speed * speed * (1 - Mathf.Abs(angleOfAttack)), 0, maxLift);

            rb.AddForce(liftDir * liftForce);
        }
    }

    private void rotatePropellers()
    {
        float rotationSpeed = trustProcent * maxPropellerSpeed * Time.deltaTime;

        foreach (GameObject propeller in propellers)
        {
            if (propeller != null)
            {
                propeller.transform.Rotate(Vector3.forward * rotationSpeed);
            }
        }
    }
}



