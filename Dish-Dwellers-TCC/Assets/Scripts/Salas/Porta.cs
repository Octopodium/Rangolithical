using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Porta : IResetavel, InteracaoCondicional {
    [Tooltip("Colisor que transporta o jogador quando destrancada")]
    [SerializeField] private GameObject portal;
    private Portal portalScript;
    [SerializeField] private Animator animator;
    [SerializeField] private float delayParaAtivarOPortal = 1.0f;
    private bool destrancada;
    public bool trancada => !destrancada;
    private AudioSource audioSource;
    public UnityEvent OnDestrancaPorta;

    private void Awake() {
        audioSource = GetComponentInChildren<AudioSource>();
        portalScript = portal.GetComponent<Portal>();
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
        return trancada && (jogador.carregando == null || jogador.carregando.CompareTag("Chave")) || (destrancada && portalScript.PlayerEstaDentro(jogador));
    }

    public MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        if (trancada && (jogador.carregando == null || !jogador.carregando.CompareTag("Chave"))) return MotivoNaoInteracao.Trancado;
        if (destrancada && portalScript.PlayerEstaDentro(jogador)) return MotivoNaoInteracao.Cancelar;
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
        
        while (timer > 0) {
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        portal.SetActive(true);
    }

    public void AudioSfx() {
        audioSource.Play();
    }
}
