using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class BotaoSelecionarFase : MonoBehaviour {
    public SalaInfo sala;
    public TMP_Text nomeSalaTxt;
    public Button botao;
    public GameObject colecionaveisHolder;
    public GameObject colecionavelPrefab;
    public float opacidadeQuandoAtivo;
    public float opacidadeQuandoInativo;
    public bool salaLiberada = true;

    void Start() {
        Setup(sala);

        ChecarProgresso();
        ProgressManager.Instance.OnProgressChange += ChecarProgresso;

        UpdateVisual();
    }

    void OnDestroy() {
        ProgressManager.Instance.OnProgressChange -= ChecarProgresso;
    }

    public void Setup(SalaInfo sala) {
        this.sala = sala;
        UpdateVisual();
    }

    void ChecarProgresso() {
        SetSalaLiberada(ProgressManager.Instance.SalaJaVisitada(sala));
    }

    public void SetSalaLiberada(bool liberada) {
        salaLiberada = liberada;
        UpdateVisual();
    }

    void UpdateVisual() {
        if (sala != null) {
            nomeSalaTxt.text = sala.nomeNoSeletor;
            var localizedString = new LocalizedString(){
                TableReference = "LocalizationTable",
                TableEntryReference = sala.nomeNoSeletor
            };
            localizedString.StringChanged += (translatedText) => //evento do localization
            {
                nomeSalaTxt.text = translatedText;
            };
        } else {
            nomeSalaTxt.text = "";
        }


        
        Color cor = nomeSalaTxt.color;
        cor.a = salaLiberada ? opacidadeQuandoAtivo : opacidadeQuandoInativo;
        nomeSalaTxt.color = cor;
        botao.interactable = salaLiberada;


        UpdateColecionaveis();
    }

    void UpdateColecionaveis() {
        foreach (Transform child in colecionaveisHolder.transform) {
            Destroy(child.gameObject);
        }

        if (sala == null) {
            return;
        }

        foreach (ColecionavelData col in sala.colecionaveis) {
            GameObject colInstance = Instantiate(colecionavelPrefab);

            ColecionavelNoSeletor colNoS = colInstance.GetComponent<ColecionavelNoSeletor>();
            colNoS.SetColecionavel(col);

            colInstance.transform.SetParent(colecionaveisHolder.transform);

        }

    }


    public void HandleSelecionada() {
        SeletorDeFase seletor = GetComponentInParent<SeletorDeFase>();
        if (seletor != null) seletor.SalaSelecionada(sala);
    }

}
