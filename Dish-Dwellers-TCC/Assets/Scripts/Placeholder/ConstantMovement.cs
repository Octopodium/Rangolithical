using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector3 initialDirection = Vector3.forward;
    [SerializeField] private bool axisAlignedBounce = true; // Mantém o movimento alinhado aos eixos X/Z

    private Rigidbody rb;
    private bool isStopped = false;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnEnable()
    {
        // Reinicia o movimento sempre que o objeto for reativado
        isStopped = false;
        rb.linearVelocity = initialDirection.normalized * speed;
        transform.rotation = startRotation;
    }

    void FixedUpdate()
    {
        if (!isStopped)
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        else
            rb.linearVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (!collision.gameObject.CompareTag("Sorvete") && !collision.gameObject.CompareTag("Escudo"))
        {
            ResetAndDisable();
            return;
        }

        if (isStopped) return;

        Vector3 normal = collision.contacts[0].normal;
        Vector3 incomingDir = rb.linearVelocity.normalized;

        
        float dot = Vector3.Dot(incomingDir, normal);
        if (dot < -0.9f)
        {
            isStopped = true;
            rb.linearVelocity = Vector3.zero;
            return;
        }

        
        Vector3 newDir = axisAlignedBounce
            ? GetAxisAlignedReflection(incomingDir, normal)
            : Vector3.Reflect(incomingDir, normal);

        rb.linearVelocity = newDir.normalized * speed;
        transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
    }

    private Vector3 GetAxisAlignedReflection(Vector3 velocity, Vector3 normal)
    {
        Vector3 reflected = Vector3.Reflect(velocity, normal);

        
        reflected = new Vector3(
            Mathf.Abs(reflected.x) > Mathf.Abs(reflected.z) ? Mathf.Sign(reflected.x) : 0,
            0,
            Mathf.Abs(reflected.z) > Mathf.Abs(reflected.x) ? Mathf.Sign(reflected.z) : 0
        );

        
        if (reflected == Vector3.zero)
            reflected = -velocity.normalized;

        return reflected;
    }

    private void ResetAndDisable()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.linearVelocity = Vector3.zero;
        isStopped = true;
        gameObject.SetActive(false);
    }
}
