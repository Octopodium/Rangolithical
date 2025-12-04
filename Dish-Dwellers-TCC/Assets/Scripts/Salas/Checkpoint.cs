using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour {
    public Transform[] spawnPoints = new Transform[2];
    [SerializeField] private Collider[] playerColliders = new Collider[2];
    [SerializeField] private LayerMask layerMask;
    [HideInInspector] public BoxCollider col;
    private Vector3 realCheckpointSize;
    bool habilitado = false;

    [Header("Configurações Visuais : ")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField, ColorUsage(true, true)] private Color[] coresPorQuantidadeDePlayers = new Color[3];
    private MaterialPropertyBlock materialPropertyBlock;
    [SerializeField] private GameObject particulas;
    [SerializeField] private VisualEffect explosao;


    private void Start() {
        Setup();
        materialPropertyBlock = new MaterialPropertyBlock();
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
            int playerAmount = 0;
            foreach( Collider collider in playerColliders)
                if(collider != null) playerAmount++;
            MudarCor(playerAmount);
            if(playerAmount == 2) HabilitarCheckPoint();
        }
    }
    private void OnTriggerExit(Collider other) {
        if(habilitado) return;
        if(other.CompareTag("Player"))
            MudarCor(0);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);
    }

    private void HabilitarCheckPoint(){
        explosao.Play();
        particulas.SetActive(true);
        sala sala = GameManager.instance.salaAtual;
        sala.spawnPoints = spawnPoints;

        habilitado = true;
    }

    private void MudarCor(int quantidadeDePlayers){
        materialPropertyBlock.SetColor("_EmissionColor", coresPorQuantidadeDePlayers[quantidadeDePlayers]);
        foreach( Renderer render in renderers){
            render.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
