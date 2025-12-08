using UnityEngine;
using UnityEngine.Events;

public class ChaoQueMexe : MonoBehaviour
{
    public float speed = 2.0f; 
    private Vector3 destination;
    private bool isMoving = false;
    private int ativacao;
    [SerializeField] private Renderer decal;
    private MaterialPropertyBlock materialPropertyBlock;
    [SerializeField, ColorUsage(true, true)] private Color corAtivada, corDesativada;

    [Header("Posições:")]
    public Vector3 posO = new Vector3();
    public Vector3 posF = new Vector3();

    [Space(15)]
    [Header("Eventos:")]
    public UnityEvent onAtivado;
    
    public UnityEvent onDesativado;

    void Awake(){
        transform.position = posO;
        materialPropertyBlock = new MaterialPropertyBlock();

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
        ativacao++;
        onAtivado?.Invoke();
        if(decal != null)TrocarCorDoDecalque(corAtivada);
        destination = posF;
        isMoving = true;
    }
 
    private void TrocarCorDoDecalque(Color col) {
        materialPropertyBlock.SetColor("_EmissionColor", col);
        decal.SetPropertyBlock(materialPropertyBlock);
    }

    public void ReturnToStart(){
        ativacao--;
        if(ativacao > 0)return;
        if(decal!= null) TrocarCorDoDecalque(corDesativada);
        onDesativado?.Invoke();
        destination = posO;
        isMoving = true;
    }
}