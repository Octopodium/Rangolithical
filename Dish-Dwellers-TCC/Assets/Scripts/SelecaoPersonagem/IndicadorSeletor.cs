using UnityEngine;
using UnityEngine.UI;

public class IndicadorSeletor : MonoBehaviour {
    public Image fundo;
    public Text texto;

    public string nome { get { return texto.text; } }
    public Color cor { get { return fundo.color; } }

    
    public System.Action onSelecionou, onConfirmou, onPronto, onSumiu, onDesconfirmou, onEsquerda, onDireita, onCima, onBaixo;

    public QualPersonagem personagemSelecionado {get; protected set;}
    public bool selecionandoNada {get; protected set;}

    public bool estaConfirmado {get; protected set;} = false;
    public bool estaPronto {get; protected set;} = false;

    public void SetCor(Color cor) {
        fundo.color = cor;
    }

    public void SetTexto(string text) {
        texto.text = text;
    }

    public void SelecionarDireita() {
        onDireita?.Invoke();
    }

    public void SelecionarEsquerda() {
        onEsquerda?.Invoke();
    }

    public void SelecionarCima() {
        onCima?.Invoke();
    }

    public void SelecionarBaixo() {
        onBaixo?.Invoke();
    }

    public void Selecionar(QualPersonagem personagem) {
        if (estaPronto || estaConfirmado) {
            onDesconfirmou?.Invoke();
        }

        estaPronto = false;
        estaConfirmado = false;
        selecionandoNada = false;
        personagemSelecionado = personagem;
        onSelecionou?.Invoke();
    }

    public void SelecionandoNada() {
        if (estaPronto || estaConfirmado) Desconfirmar();
        selecionandoNada = true;
        estaPronto = false;
        estaConfirmado = false;
    }

    /// <summary>
    /// Quando um personagem está selecionado, confirmar é definir que este é o personagem o qual o usuário quer utilizar
    /// </summary>
    public void Confirmar() {
        if (estaConfirmado && !estaPronto) DarPronto();
        else {
            estaConfirmado = true;
            estaPronto = false;
            onConfirmou?.Invoke();
        }
    }

    public void Desconfirmar() {
        if (estaPronto || estaConfirmado) {
            onDesconfirmou?.Invoke();
        }

        estaPronto = false;
        estaConfirmado = false;
    }

    /// <summary>
    /// Quando um personagem está selecionado e confirmado, dar pronto é definir que já pode encerrar a seleção (requer que os dois jogadores confirmados dêem pronto para encerrar)
    /// </summary>
    public void DarPronto() {
        if (!estaConfirmado) Confirmar();
        else {
            estaPronto = true;
            onPronto?.Invoke();
        }
    }

    public void Sumir() {
        onSumiu?.Invoke();
        Destroy(gameObject);
    }
}
