using Assets.scripts.PlaneController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject healthBar;
    public float healthProcent;
    public float lerpedSpeed=0.05f;

    private void Start()
    {
        healthProcent = 100f;
    }

    private void Update()
    {
        healthBar.transform.localScale = new Vector3(healthProcent / 100, 1f, 1f);
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
