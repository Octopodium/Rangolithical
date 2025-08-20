using Mirror;
using UnityEngine;

public class ObjetoDeJogo : MonoBehaviour {
    [Tooltip("Objetos interagiveis são objetos onde o jogador, ao se aproximar, pode interagir com eles de alguma forma. Todo objeto interagivel precisa de um colisor.")]
    public bool interagivel;
    [HideInInspector] public Interagivel interagivelComponent;

    [Tooltip("Objetos carregaveis são aqueles que, ao se aproximar, o player é capaz de segurar. Todos objetos carregaveis são por interagiveis.")]
    public bool carregavel;
    [HideInInspector] public Carregavel carregavelComponent;

    [Tooltip("Objetos ganhaveis são objetos que podem ser presos a um gancho, e caso possuam Rigidbody, podem ser puxado pelo mesmo. Todo objeto ganchavel precisa de um colisor.")]
    public bool ganchavel;
    [HideInInspector] public Ganchavel ganchavelComponent;

    [Tooltip("Objetos instanciados dinamicamente são aqueles objetos que por padrão não estão em uma cena, e sim são instanciados por algum outro objeto. Nem todo prefab é instanciado dinamicamente.")]
    public bool instanciadoDinamicamente;
    [HideInInspector] public NetworkIdentity identityComponent;

    [Tooltip("Por conta do multiplayer, se um objeto tem a habilidade de mover e essa habilidade não é um mecanismo (elevadores são mecanismos, projeteis não), você precisa especificar para ser tratado igualmente entre clientes. Por sincronizar posição, este objeto deve ser instanciado dinamicamente.")]
    public bool sincronizarPosicao;
    [HideInInspector] public NetworkTransformUnreliable netTransformComponent;
    
    protected Sincronizavel sincronizavel;


    public void AtualizarComponentes() {
        if (carregavel) interagivel = true;
        if (sincronizarPosicao) instanciadoDinamicamente = true;

        TratarComponente<Interagivel>(interagivel, out interagivelComponent);
        TratarComponente<Carregavel>(carregavel, out carregavelComponent);
        TratarComponente<Ganchavel>(ganchavel, out ganchavelComponent);

        bool precisaDeSincronizavel = interagivel || ganchavel;
        TratarComponente<Sincronizavel>(precisaDeSincronizavel, out sincronizavel);

        TratarComponente<NetworkIdentity>(instanciadoDinamicamente, out identityComponent);
        TratarComponente<NetworkTransformUnreliable>(sincronizarPosicao, out netTransformComponent);

        if (interagivel) {
            int layer = LayerMask.NameToLayer("Interagivel");
            gameObject.layer = layer;
        }
    }

    void TratarComponente<T>(bool possuiComponente, out T componente) where T : MonoBehaviour{
        componente = gameObject.GetComponent<T>();

        if (possuiComponente && componente == null) {
            componente = gameObject.AddComponent<T>();
        } else if (!possuiComponente && componente != null) {
            DestroyImmediate(componente);
            componente = null;
        }
    }

    public Sincronizavel GetSincronizavel(bool obrigatorio = true) {
        if (sincronizavel != null || !obrigatorio) return sincronizavel;

        sincronizavel = gameObject.GetComponent<Sincronizavel>();
        if (sincronizavel != null) return sincronizavel;

        sincronizavel = gameObject.AddComponent<Sincronizavel>();
        return sincronizavel;
    }
}
