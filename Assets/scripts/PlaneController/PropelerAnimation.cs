using UnityEngine;

public class PropelerAnimation : MonoBehaviour
{
    public Material blurMaterial;
    public float rotationSpeed; // в градусах в секунду

    void Update()
    {
        // Вращение объекта
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Передача силы размытия в шейдер
        float blurStrength = Mathf.Clamp01(rotationSpeed / 1000f);
        blurMaterial.SetFloat("_BlurStrength", blurStrength);
    }
}
