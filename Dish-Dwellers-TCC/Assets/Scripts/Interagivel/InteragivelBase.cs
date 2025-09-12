using UnityEngine;

public abstract class InteragivelBase : MonoBehaviour {
    [Header("Indicador de interação")]
    [HideInInspector] public Indicador indicador;
    //GameObject indicador;
    public Vector3 offsetIndicador = Vector3.up;


    
    public abstract void Interagir(Player jogador);
    public abstract bool PodeInteragir(Player jogador);
    public abstract MotivoNaoInteracao NaoPodeInteragirPois(Player jogador);


    protected virtual void Start() {
        GameManager.instance.controle.OnIndicadorChange += OnIndicadorChange;
        indicador = GameManager.instance.controle.indicadorAtual;

    }

    protected virtual void OnDestroy() {
        if (GameManager.instance == null || GameManager.instance.controle == null) return;
        GameManager.instance.controle.OnIndicadorChange -= OnIndicadorChange;
    }



    public virtual void MostrarIndicador(bool mostrar, MotivoNaoInteracao motivo = MotivoNaoInteracao.Nenhum) {
        if (indicador) {
            if (mostrar) indicador.Mostrar(this, motivo);
            else indicador.Esconder(this);
        }
    }

    public virtual void OnIndicadorChange(Indicador novoIndicador) {
        if (novoIndicador == indicador) return;

        if (indicador != null && indicador.interagivel == this) {
            indicador.Esconder(this);
            indicador = novoIndicador;
            indicador.Mostrar(this);
        } else {
            indicador = novoIndicador;
        }
    }

    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + offsetIndicador, Vector3.one * 0.1f);
    }
}
