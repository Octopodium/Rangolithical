using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour {
    public Transform[] spawnPoints = new Transform[2];
    [SerializeField] private Collider[] playerColliders = new Collider[2];
    [SerializeField] private LayerMask layerMask;
    [HideInInspector] public BoxCollider col;
    private Vector3 realCheckpointSize;
    bool habilitado = false;


    private void Start() {
        Setup();
    }

    private void OnValidate() {
        Setup();
    }

    private void Setup() {
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0, col.size.y / 2, 0);
        realCheckpointSize = new Vector3(
            transform.localScale.x * col.size.x,
            transform.localScale.y * col.size.y,
            transform.localScale.z * col.size.z
        );
    }

    private void OnTriggerEnter(Collider other) {
        if(habilitado) return;
        if (other.CompareTag("Player")) {
            Physics.OverlapBoxNonAlloc(transform.position + col.center, realCheckpointSize / 2, playerColliders, transform.rotation, layerMask.value);
            for(int i = 0 ; i < 2; i++) {
                if (playerColliders[i] == null) return;
                Debug.Log(playerColliders[i].gameObject.name);
            }
            HabilitarCheckPoint();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);
    }

    private void HabilitarCheckPoint(){
        Debug.Log("Checkpoint habilitado");
        sala sala = GameManager.instance.salaAtual;
        sala.spawnPoints = spawnPoints;

        habilitado = true;
    }
}
