using HeneGames.Airplane;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlaneInterfaceControll : MonoBehaviour
{
    public GameObject Wheel;
    public GameObject Trust;
    public GameObject Techometer;
    public GameObject Hieght;
    public GameObject Plane;
    public GameObject BrokenMap;
    public GameObject Heat;
    public GameObject DangerLight;
    public GameObject StabLight;

    private Vector3 StartSize;
    private Vector3 currentScale;

    private SimpleAirPlaneController controller;

    private bool isDead=false;

    private void Start()
    {
        BrokenMap.SetActive(false);
        StartSize = new Vector3(2.6f, 2f, 1f);
        Wheel.transform.localScale = StartSize;
    }

    private void Awake()
    {
        Plane = GameObject.FindGameObjectWithTag("PlayerPlane");
        controller = Plane.GetComponent<SimpleAirPlaneController>();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            dead();
        }
        else
        {
            WheelManager();
            TrustManager();
            HieghtManager();
            HeatManager();
            StabilisationManager();
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
        Plane = newPlane;
        controller=Plane.GetComponent<SimpleAirPlaneController>();
    }


    private void HeatManager()
    {
        if (controller.TurboOverheating())
        {
            DangerLight.GetComponent<Image>().color= Color.red;
        }
        else
        {
            DangerLight.GetComponent<Image>().color = Color.black;
        }
        float heat = controller.TurboHeatValue()/100.0f;
        Heat.transform.rotation = Quaternion.Euler(0, 0,(-260*heat)+ 130);
    }


    private void StabilisationManager()
    {
        if (controller.GetStabilisation())
        {
            StabLight.GetComponent<Image>().color = new Color32(198, 255, 71, 255);
        }
        else
        {
            StabLight.GetComponent<Image>().color = new Color32(58, 58, 58, 255);
        }
    }

    private void HieghtManager()
    {
        if (Plane != null)
        {
            float hieght = Plane.transform.position.y;
            Hieght.GetComponent<Text>().text = hieght.ToString();
        }
    }

    private void TrustManager()
    {
        float trustProcent = controller.getTrustProcent();
        Trust.transform.GetComponent<Slider>().value = trustProcent;

        float tachometerProcent = Mathf.Clamp01(controller.getCurrentSpeed() / controller.getDefaultSpeed() * trustProcent);

        Techometer.transform.rotation = Quaternion.Euler(0, 0, 
            -260 * (tachometerProcent + tachometerProcent * Random.Range(-0.01f,0.01f))+130);
    }


    private void WheelManager()
    {
        float roll = controller.getRoll();
        float pitch = controller.getPitch();

        Wheel.transform.rotation = Quaternion.Lerp(Wheel.transform.rotation, Quaternion.Euler(0, 0, -60 * roll), Time.deltaTime);

        Vector3 targetScale = StartSize * (1 + (-pitch * 0.3f));
        currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime);

        Wheel.transform.localScale = currentScale;
    }

    private void dead()
    {
        DangerLight.GetComponent<Image>().color = Color.red;

        Heat.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-130, 130));
        Techometer.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-130, 130));

        BrokenMap.SetActive(true);
    }

    public void DeadPlaneInterface()
    {
        isDead=true;
    }
}
