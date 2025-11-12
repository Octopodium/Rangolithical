using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Porta : IResetavel, InteracaoCondicional {
    [Tooltip("Colisor que transporta o jogador quando destrancada")]
    [SerializeField] private GameObject portal;
    [SerializeField] private Animator animator;
    [SerializeField] private float delayParaAtivarOPortal = 1.0f;
    private bool destrancada;
    private AudioSource audioSource;
    public bool trancada => !destrancada;
    public UnityEvent OnDestrancaPorta;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();        
    }

    private void Start() {
        portal.SetActive(false);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Chave")) {
            Destrancar();
            other.gameObject.SetActive(false);
        }
    }

    public override void OnReset() {
        Trancar();
        portal.GetComponent<Portal>().OnReset();
    }

    public void Interagir(Player jogador) {
        if (destrancada) {
            return;
        }

        if (jogador.carregando != null && jogador.carregando.CompareTag("Chave")) {
            Destrancar();
            // Retira a chave do jogador.

            jogador.carregando.gameObject.SetActive(false);
            jogador.carregador.carregado = null;
        }
    }

    public bool PodeInteragir(Player jogador) {
        return trancada || jogador.carregando == null || jogador.carregando.CompareTag("Chave");
    }

    public MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        if (jogador.carregando == null || !jogador.carregando.CompareTag("Chave")) return MotivoNaoInteracao.Trancado;
        return MotivoNaoInteracao.Nenhum;
    }

    public void Destrancar() {
        StartCoroutine(AbrirPorta());

        OnDestrancaPorta?.Invoke();

        // Previne que o jogador possa destrancar a porta duas vezes.
        destrancada = true;
    }

    public void Trancar() {
        StopAllCoroutines();
        portal.SetActive(false);
        animator.SetBool("Aberta", false);

        destrancada = false;
    }

    IEnumerator AbrirPorta() {
        animator.SetBool("Aberta", true);
        float timer = delayParaAtivarOPortal;
        audioSource.Play();

        while (timer > 0) {
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        portal.SetActive(true);
    }
}
