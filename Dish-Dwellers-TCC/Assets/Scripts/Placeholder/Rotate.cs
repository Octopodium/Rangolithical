using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Eixo de rotação
    [SerializeField] private float rotationAngle = 90f;         // Quantos graus vai girar

    public void doRotate()
    {
        transform.Rotate(rotationAxis.normalized * rotationAngle, Space.Self);
    }
}