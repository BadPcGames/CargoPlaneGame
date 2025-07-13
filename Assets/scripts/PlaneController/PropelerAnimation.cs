using UnityEngine;

public class PropelerAnimation : MonoBehaviour
{
    public Material blurMaterial;
    public float rotationSpeed; // � �������� � �������

    void Update()
    {
        // �������� �������
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // �������� ���� �������� � ������
        float blurStrength = Mathf.Clamp01(rotationSpeed / 1000f);
        blurMaterial.SetFloat("_BlurStrength", blurStrength);
    }
}
