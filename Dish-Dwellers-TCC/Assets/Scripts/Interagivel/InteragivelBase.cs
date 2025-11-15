using System.Collections.Generic;
using UnityEngine;

public abstract class InteragivelBase : MonoBehaviour {
    [Header("Indicador de interação")]
    [HideInInspector] public HashSet<Indicador> indicadores = new HashSet<Indicador>();
    //GameObject indicador;
    public Vector3 offsetIndicador = Vector3.up;
    
    [Tooltip("Sobreescrever o transform base onde o indicador será parenteado em sua exibição")]
    public Transform overrideIndicadorTransform;

    public Transform indicadorTransform { get { return overrideIndicadorTransform == null ? transform : overrideIndicadorTransform; }}


    
    public abstract void Interagir(Player jogador);
    public abstract bool PodeInteragir(Player jogador);
    public abstract MotivoNaoInteracao NaoPodeInteragirPois(Player jogador);


    protected virtual void Start() { }

    protected virtual void OnDestroy() { }

    public virtual void MudarIndicadorTransform(Transform overrideTransform, Player jogadorDoIndicador = null) {
        Transform oldTrans = overrideIndicadorTransform;
        overrideIndicadorTransform = overrideTransform;

        if (oldTrans != overrideTransform && indicadores.Count > 0) {
            foreach (var indicador in indicadores) {
                if (jogadorDoIndicador == null || indicador.jogador == jogadorDoIndicador)
                    indicador.Refresh();
            }
        }
    }



    public virtual void MostrarIndicador(bool mostrar, Indicador indicador, MotivoNaoInteracao motivo = MotivoNaoInteracao.Nenhum) {
        if (indicador) {
            if (mostrar) {
                indicadores.Add(indicador);
                indicador.Mostrar(this, motivo);
            } else {
                indicador.Esconder(this);
                indicadores.Remove(indicador);
            }
        }
    }

    public Indicador ProximoIndicador(Indicador exceto = null) {
        foreach (Indicador i in indicadores) {
            if (i != exceto) return i;
        }

        return null;
    }

/*
            public virtual void OnIndicadorChange(Indicador novoIndicador) {
                if (novoIndicador == indicador) return;

                if (indicador != null && indicador.interagivel == this) {
                    indicador.Esconder(this);
                    indicador = novoIndicador;
                    indicador.Mostrar(this);
                }
                else {
                    indicador = novoIndicador;
                }
            }
            */

    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + offsetIndicador, Vector3.one * 0.1f);
    }
}
