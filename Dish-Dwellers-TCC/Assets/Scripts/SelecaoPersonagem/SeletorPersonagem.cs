using UnityEngine;
using UnityEngine.UI;

public class SeletorPersonagem : MonoBehaviour {
    public QualPersonagem personagem;

    public Transform indicadoresHolder;
    public Text playerText;
    public Image fundo;

    public GameObject pronto;

    public Color naoSelecionadoFundo;

    public IndicadorSeletor indicadorAtual {get; protected set;}
    public bool estaPronto {get {return indicadorAtual != null && indicadorAtual.estaPronto; }}

    public void Reset() {
        Confirmar(null);

        foreach (Transform child in indicadoresHolder) {
            Destroy(child.gameObject);
        }
    }

    public void Selecionar(IndicadorSeletor indicador) {
        indicador.gameObject.SetActive(true);
        indicador.transform.SetParent(indicadoresHolder);
    }

    public void Confirmar(IndicadorSeletor indicador) {
        if (indicadorAtual != indicador && indicadorAtual != null && !indicadorAtual.gameObject.activeSelf) {
            indicadorAtual.gameObject.SetActive(true);
            indicadorAtual.Selecionar(personagem);
        }

        indicadorAtual = indicador;
        AtualizarVisual();
    }

    public void Pronto(IndicadorSeletor indicador) {
        if (indicador != indicadorAtual) return;
        AtualizarVisual();
    }

    public void AtualizarVisual() {
        if (indicadorAtual != null) {
            indicadorAtual.gameObject.SetActive(false);
            playerText.text = indicadorAtual.nome;
            fundo.color = indicadorAtual.cor;
            pronto.SetActive(indicadorAtual.estaPronto);
        } else {
            playerText.text = "";
            fundo.color = naoSelecionadoFundo;
            pronto.SetActive(false);
        }
    }
}
