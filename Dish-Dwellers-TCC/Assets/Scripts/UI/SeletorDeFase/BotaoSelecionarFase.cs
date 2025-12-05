using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotaoSelecionarFase : MonoBehaviour {
    public SalaInfo sala;
    public TMP_Text nomeSalaTxt;
    public GameObject colecionaveisHolder;
    public GameObject colecionavelPrefab;
    public float opacidadeQuandoAtivo;
    public float opacidadeQuandoInativo;
    public bool salaLiberada = true;

    void Start() {
        Setup(sala);
        SetSalaLiberada(salaLiberada);
    }

    public void Setup(SalaInfo sala) {
        this.sala = sala;
        UpdateVisual();
    }

    public void SetSalaLiberada(bool liberada) {
        salaLiberada = liberada;
        UpdateVisual();
    }

    void UpdateVisual() {
        if (sala != null) {
            nomeSalaTxt.text = sala.nomeNoSeletor;
        } else {
            nomeSalaTxt.text = "";
        }
        
        Color cor = nomeSalaTxt.color;
        cor.a = salaLiberada ? opacidadeQuandoAtivo : opacidadeQuandoInativo;
        nomeSalaTxt.color = cor;

        UpdateColecionaveis();
    }

    void UpdateColecionaveis() {
        foreach (Transform child in colecionaveisHolder.transform) {
            Destroy(child);
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
