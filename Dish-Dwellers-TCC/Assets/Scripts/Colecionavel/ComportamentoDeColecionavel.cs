using UnityEngine;
using UnityEngine.Events;

public class ComportamentoDeColecionavel : MonoBehaviour {
    public ColecionavelData colecionavel;
    [Tooltip("Se verdadeiro, irá checar a condição apenas no Start (ou quando o comportamento for setado). Se falso, mesmo chamando 'NaoTem' no inicio, chamará 'Tem' caso o jogador colete durante a fase.")]
    public bool checarUmaUnicaVez = false;

    public UnityEvent OnTemColecionavel;
    public UnityEvent OnNaoTemColecionavel;

    public bool encontrado { get { return jaEncontrado; }}

    bool setouEvento = false;
    bool jaEncontrado = false;

    public void Start() {
        if (ColecionavelController.instance != null && colecionavel != null) {
            CheckSeTem();

            if (!checarUmaUnicaVez && !setouEvento) {
                ColecionavelController.instance.onCarregado += OnColecionaveisCarregados;
                setouEvento = true;
            }
        }
    }

    void OnDestroy() {
        if (setouEvento) {
            ColecionavelController.instance.onCarregado -= OnColecionaveisCarregados;
            setouEvento = false;
        }
    }

    public void SetColecionavel(ColecionavelData colecionavel) {
        if (this.colecionavel == colecionavel) return;
        this.colecionavel = colecionavel;

        if (ColecionavelController.instance != null && colecionavel != null) {
            CheckSeTem();

            if (!checarUmaUnicaVez && !setouEvento) {
                ColecionavelController.instance.onCarregado += OnColecionaveisCarregados;
                setouEvento = true;
            }
        }
    }

    void OnColecionaveisCarregados(ColetavelSave _vals) {
        CheckSeTem();
    }

    public void CheckSeTem() {
        if (colecionavel != null && ColecionavelController.instance.Tem(colecionavel)) {
            jaEncontrado = true;
            OnTemColecionavel?.Invoke();
        }
    }
}
