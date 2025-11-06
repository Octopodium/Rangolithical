using UnityEngine;
using UnityEngine.Events;

public class ChaoQueMexe : MonoBehaviour
{
    public float speed = 2.0f; 
    private Vector3 destination;
    private bool isMoving = false;

    [Header("Posições:")]
    public Vector3 posO = new Vector3();
    public Vector3 posF = new Vector3();

    [Space(15)]
    [Header("Eventos:")]
    public UnityEvent onAtivado;
    
    public UnityEvent onDesativado;

    void Awake(){
        transform.position = posO;
    }

    void FixedUpdate(){
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveToTarget(){
        onAtivado?.Invoke();
        destination = posF;
        isMoving = true;
    }

    public void ReturnToStart(){
        onDesativado?.Invoke();
        destination = posO;
        isMoving = true;
    }
}