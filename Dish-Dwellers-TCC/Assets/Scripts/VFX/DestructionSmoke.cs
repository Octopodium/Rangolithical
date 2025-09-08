using UnityEngine;

public class DestructionSmoke : MonoBehaviour{

    [SerializeField] private Rigidbody rigidbody;


    private void Awake(){
        rigidbody = GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate(){
        Vector3 smokeDirection = -rigidbody.linearVelocity;
        transform.up = smokeDirection.normalized;
    }

}
