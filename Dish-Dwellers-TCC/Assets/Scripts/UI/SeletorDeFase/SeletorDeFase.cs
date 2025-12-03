using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SeletorDeFase : MonoBehaviour {
    public bool irParaCena = true;

    public System.Action<SalaInfo> salaSelecionada;

    public UnityEvent OnFechado;

    public void SalaSelecionada(SalaInfo sala) {
        salaSelecionada?.Invoke(sala);
        if (irParaCena) IrParaSala(sala.caminhoParaSala);
    }

    public void IrParaSala(string salaFase){
        if(salaFase != null){
            SceneManager.LoadScene(salaFase, LoadSceneMode.Single);
        }
    }

    public void HandleFechar() {
        OnFechado?.Invoke();
    }

}
