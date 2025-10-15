using UnityEngine;

public interface IRecebeTemplate {
    void RecebeTemplate(GameObject template);
}


[RequireComponent(typeof(Sincronizavel))]
public class ControladorDeObjeto : IResetavel, SincronizaMetodo {
    [Header("</color=green>Componentes : </color>")]
    [Space(10)]
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject prefabOnline;
    public Vector3 respawnPos = new Vector3(0, 6, 0);
    [SerializeField] private GameObject triggerSpawnChave;

    [Space(15)]
    [Header("Objeto controlado :")]
    [Space(10)]
    public GameObject objeto;

    [Space(15)]
    [Header("Configurações")]
    [Space(10)]
    [SerializeField] private bool spawnNoInicio = false;
    [Tooltip("Caso o objeto prefab seja 'IRecebeTemplate', ele recebera o objeto 'template' como parâmetro de 'RecebeTemplate'. Util para replicar valores base nos objetos recém instanciados.")]
    public GameObject template;

    Sincronizavel sinc;


    private void Start() {
        if (template != null) template.SetActive(false);

        sinc = GetComponent<Sincronizavel>();
        SetupSpawner();

        if (spawnNoInicio)
            Spawn();
    }

    bool spawnerSetted = false;
    void SetupSpawner() {
        if (!GameManager.instance.isOnline) return;
        if (spawnerSetted) return;

        GameObject prefabToUse = prefabOnline != null ? prefabOnline : prefab;
        if (sinc == null) sinc = GetComponent<Sincronizavel>();

        Sincronizador.instance.RegistrarSpawner(prefabToUse, respawnPos, transform.rotation, sinc, AposSpawn);
        spawnerSetted = true;
    }

    void OnDestroy() {
        if (objeto != null) {
            Destrutivel destrutivel = objeto.GetComponent<Destrutivel>();
            if (destrutivel != null) {
                destrutivel.OnDestruido.RemoveListener(Respawn);
            }

            Destroy(objeto);
            objeto = null;
        }
    }

    public override void OnReset() {
        Reiniciar();
    }

    /// <summary>
    /// Caso não exista nenhum objeto atribuido ao campo do objeto controlado, instancia um novo objeto com base no prefab.
    /// </summary>
    [Sincronizar]
    public void Spawn() {
        if (objeto != null) return;

        gameObject.Sincronizar();
        
        if (!GameManager.instance.isOnline) AposSpawn(Instantiate(prefab, transform.TransformPoint(respawnPos), transform.rotation));
        else {
            SetupSpawner();
            GameObject prefabToUse = prefabOnline != null ? prefabOnline : prefab;
            Sincronizador.instance.InstanciarNetworkObject(prefabToUse, sinc);
        }
    }

    void AposSpawn(GameObject objeto) {

        if (objeto != null && objeto != this.objeto) {
            if (this.objeto != null) Destroy(this.objeto);

            Destrutivel destrutivel = objeto.GetComponent<Destrutivel>();
            destrutivel?.OnDestruido.AddListener(Respawn);
            this.objeto = objeto;

            if (template != null) {
                IRecebeTemplate recebeTemplate = objeto.GetComponent<IRecebeTemplate>();
                if (recebeTemplate != null) {
                    recebeTemplate.RecebeTemplate(template);
                }
            }
        }
    }


    /// <summary>
    /// Transporta o objeto controlado para o ponto de respawn atribuido no componente e ativa ele.
    /// </summary>
    [Sincronizar]
    public void Respawn(){
        gameObject.Sincronizar();
        objeto.transform.position = transform.TransformPoint(respawnPos);

        if(!objeto.activeInHierarchy)
            objeto.SetActive(true);
    }

    /// <summary>
    /// Destroi o objeto controlado e reinicia o sistema
    /// </summary>
    [Sincronizar]
    public void Reiniciar() {
        gameObject.Sincronizar();

        if (objeto != null) {
            // Essa parte é exclusiva pra esse código, instanciar e desinstanciar 
            objeto.SetActive(false);

            Sincronizavel sincronizavel = objeto.GetComponent<Sincronizavel>();
            if (sincronizavel != null) {
                sincronizavel.PreDestroy();
            }

            Destroy(objeto);
            objeto = null;
        }

        if (spawnNoInicio)
            Spawn();
    }

}
