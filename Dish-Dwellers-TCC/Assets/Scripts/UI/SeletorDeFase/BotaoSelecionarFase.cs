using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotaoSelecionarFase : MonoBehaviour {
    public SalaInfo sala;
    public TMP_Text nomeSalaTxt;
    public GameObject colecionaveisHolder;
    public GameObject colecionavelPrefab;

    void Start() {
        Setup(sala);
    }

    public void Setup(SalaInfo sala) {
        this.sala = sala;
        UpdateVisual();
    }

    void UpdateVisual() {
        if (sala != null) {
            nomeSalaTxt.text = sala.nomeNoSeletor;
        } else {
            nomeSalaTxt.text = "";
        }

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
