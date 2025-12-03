using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ComportamentoDeColecionavel))]
public class ColecionavelNoSeletor : MonoBehaviour {
    public ComportamentoDeColecionavel comportamento;
    public Image img;
    public Color naoEncontradoColor;
    public Color jaEncontradoColor;
    public ColecionavelData colecionavel;
    bool setouEventos = false;

    public void SetColecionavel(ColecionavelData colecionavel) {
        this.colecionavel = colecionavel;

        if (setouEventos) {
            comportamento.OnTemColecionavel.RemoveListener(SetEncontrado);
            comportamento.OnNaoTemColecionavel.RemoveListener(SetNaoEncontrado);
        }

        if (colecionavel == null) {
            img.gameObject.SetActive(false);
            return;
        }

        img.gameObject.SetActive(true);
        img.sprite = colecionavel.sprite;

        comportamento.OnTemColecionavel.AddListener(SetEncontrado);
        comportamento.OnNaoTemColecionavel.AddListener(SetNaoEncontrado);
        comportamento.SetColecionavel(colecionavel);
        setouEventos = true;

        if (comportamento.encontrado) SetEncontrado();
        else SetNaoEncontrado();
    }

    public void SetNaoEncontrado() {
        img.color = naoEncontradoColor;
    }

    public void SetEncontrado() {
        img.color = jaEncontradoColor;
    }
}
