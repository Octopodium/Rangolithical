using UnityEngine;

/// <summary>
/// Um PontoInteragivel serve apenas como um colisor para outro Interagivel. Quando o jogador se aproxima/interage com o ponto, a interação é redirecionada para o Interagivel referenciado.
/// </summary>
[RequireComponent(typeof(SubSincronizavel))]
public class PontoInteragivel : InteragivelBase {
    SubSincronizavel _subSincronizavel;
    [HideInInspector] public SubSincronizavel subSincronizavel {
        get {
            if (_subSincronizavel == null) _subSincronizavel = GetComponent<SubSincronizavel>();
            return _subSincronizavel;
        }
    }

    public Interagivel interagivelParaRedirecionar { get; protected set; }
    public bool utilizarIndicadorProprio = true;

    void Awake() {
        if (interagivelParaRedirecionar != null) {
            interagivelParaRedirecionar.AddPonto(this);
        }
    }

    public void SetInteragivelParaRedirecionar(Interagivel interagivel) {
        if (interagivelParaRedirecionar != null) {
            interagivelParaRedirecionar.RemovePonto(this);
        }

        interagivelParaRedirecionar = interagivel;
        interagivelParaRedirecionar.AddPonto(this);
    }

    public override void Interagir(Player jogador) {
        interagivelParaRedirecionar.ultimoPonto = this;
        interagivelParaRedirecionar.Interagir(jogador);
    }

    public override bool PodeInteragir(Player jogador) {
        return interagivelParaRedirecionar.PodeInteragir(jogador);
    }

    public override MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        return interagivelParaRedirecionar.NaoPodeInteragirPois(jogador);
    }
    
    public override void MostrarIndicador(bool mostrar, Indicador indicador, MotivoNaoInteracao motivo = MotivoNaoInteracao.Nenhum) {
        if (utilizarIndicadorProprio) base.MostrarIndicador(mostrar, indicador, motivo);
        else interagivelParaRedirecionar.MostrarIndicador(mostrar, indicador, motivo);
    }

    protected override void OnDrawGizmosSelected() {
        if (utilizarIndicadorProprio) base.OnDrawGizmosSelected();
    }
}
