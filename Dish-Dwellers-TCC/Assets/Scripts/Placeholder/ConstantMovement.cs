using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector3 initialDirection = Vector3.forward;
    [SerializeField] private bool axisAlignedBounce = true; // Ativar/desativar modo "quique reto"

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.linearVelocity = initialDirection.normalized * speed;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 newDirection;

        if (axisAlignedBounce)
        {
           
            newDirection = GetAxisAlignedReflection(rb.linearVelocity, normal);
        }
        else
        {
          
            newDirection = Vector3.Reflect(rb.linearVelocity.normalized, normal);
        }

        rb.linearVelocity = newDirection.normalized * speed;
        transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
    }

    private Vector3 GetAxisAlignedReflection(Vector3 velocity, Vector3 normal)
    {
        Vector3 reflected = Vector3.Reflect(velocity.normalized, normal);

        
        reflected = new Vector3(
            Mathf.Abs(reflected.x) > Mathf.Abs(reflected.z) ? Mathf.Sign(reflected.x) : 0,
            0,
            Mathf.Abs(reflected.z) > Mathf.Abs(reflected.x) ? Mathf.Sign(reflected.z) : 0
        );

        
        if (reflected == Vector3.zero)
            reflected = -velocity.normalized;

        return reflected;
    }
}