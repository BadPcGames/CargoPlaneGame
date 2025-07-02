using Assets.scripts.PlaneController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Slider healthBar;
    public float healthProcent;
    public float lerpedSpeed=0.05f;

    private void Start()
    {
        healthProcent = 100f;
    }

    private void Update()
    {
        if (healthProcent != healthBar.value)
        {
            healthBar.value=healthProcent;
        }
    }

    private void OnEnable()
    {
        PlaneStats.OnAirplaneChangedHealth += Updatehealth;
    }

    private void OnDisable()
    {
        PlaneStats.OnAirplaneChangedHealth -= Updatehealth;
    }

    private void Updatehealth(float health)
    {
        healthProcent=health;
    }

}
