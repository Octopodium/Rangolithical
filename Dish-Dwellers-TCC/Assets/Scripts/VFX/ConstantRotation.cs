using UnityEngine;

public class ConstantRotation : MonoBehaviour{

    [SerializeField] private Vector3 rotacaoConstante;


    private void FixedUpdate() {
        transform.Rotate(rotacaoConstante * Time.fixedDeltaTime);
    }
}
