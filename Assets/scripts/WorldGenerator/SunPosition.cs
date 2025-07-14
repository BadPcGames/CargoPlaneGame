using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunPosition : MonoBehaviour
{
    //[SerializeField]
    //private int radius;
    //[SerializeField]
    //private int speed;
    //private bool called = false;
    //private Vector3 position;

    //public static int angle=0;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!called)
    //    {
    //        StartCoroutine(CallMethodInSeconds());
    //        called = true;
    //    }
    //}
    //IEnumerator CallMethodInSeconds()
    //{
    //    yield return new WaitForSeconds(1f);
    //    changePosition();
    //    called = false;
    //}

    //private void changePosition()
    //{
    //    position.z = radius * Mathf.Cos((angle * Mathf.PI) / 180);
    //    position.y = radius * Mathf.Sin((angle * Mathf.PI) / 180);
    //    transform.position = position;
    //    transform.rotation = Quaternion.Euler(180-angle, 0f, 0f);
    //    angle+=speed;
    //    if (angle == 360)
    //    {
    //        angle = 0;
    //    }
    //}

    [SerializeField]
    Gradient directionalLight;
    [SerializeField]
    Gradient ambientLight;
    [SerializeField, Range(1, 3600)] 
    float timeDayinSecond = 120;
    
    public static float timeProgress ;

    [SerializeField] Light Light;

    Vector3 defaultAngle;

    private void Start()
    {
        defaultAngle = Light.transform.localEulerAngles; 
    }
    private void Update()
    {
        timeProgress += Time.deltaTime/timeDayinSecond;

        if (timeProgress > 1f)
        {
            timeProgress = 0f;
        }
        Light.color = directionalLight.Evaluate(timeProgress);
        RenderSettings.ambientLight= ambientLight.Evaluate(timeProgress);

        Light.transform.localEulerAngles = new Vector3(360f * timeProgress - 90, defaultAngle.x, defaultAngle.z);
    }

}
