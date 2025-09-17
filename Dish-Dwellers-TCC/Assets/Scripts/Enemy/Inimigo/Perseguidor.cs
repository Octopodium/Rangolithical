using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Perseguidor : Inimigo {

    [Header("Foco do alvo")]
    [SerializeField] private float tempoDeFoco = 3f;
    private float tempoRestanteDeFoco;
    private bool temAlvoFixo = false;
    private Vector3 direction;
    private Vector3 flipY;

    [Header("Reação ao Escudo")]
    [SerializeField] private float tempoCaido = 3f; 
    private float tempoCaidoRestante;
    private bool caido = false;

    [SerializeField] private AnimatorPerseguidor animator;
    private NavMeshAgent navAgent;
    private Rigidbody rb;
    private bool podePerseguir = true;

    private void Awake() {
        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<AnimatorPerseguidor>();
    }

    private void Start() {
        target = EncontrarPlayerMaisProximo();
        if (target != null) {
            temAlvoFixo = true;
            tempoRestanteDeFoco = tempoDeFoco;
        }
    }

    private void FixedUpdate() {
        if (caido) {
            tempoCaidoRestante -= Time.deltaTime;
            if (tempoCaidoRestante <= 0f) Recuperar();
            return;
        }

        if (navAgent == null || (navAgent != null && !navAgent.isOnNavMesh)) {
            base.ChecagemDeZonas();
            AtualizarAlvo();
            return;
        }

        base.ChecagemDeZonas();
        AtualizarAlvo();
        Perseguir();
    }

      public void Perseguir() {
        if (!podePerseguir) {
            return;
        }

        if (_playerNoCampoDeVisao && target != null && navAgent != null && navAgent.isOnNavMesh) {
            navAgent.SetDestination(target.position);
            navAgent.updateRotation = false;
            animator.Persegue(true);

            direction = transform.position - target.position;
            animator.Olhar(direction);
            direction.y = 0;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("Escudo") && !caido) {
            CaidoPorEscudo();
        }
    }

    private void CaidoPorEscudo() {
        caido = true;
        tempoCaidoRestante = tempoCaido;
        podePerseguir = false;

        if (navAgent != null && navAgent.isOnNavMesh) {
            navAgent.isStopped = true;
            navAgent.updateRotation = false;
        }

        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Recuperar() {
        caido = false;
        podePerseguir = true;

        if (navAgent != null && navAgent.isOnNavMesh) {
            navAgent.isStopped = false;
            navAgent.updateRotation = true;
        }

        if (rb != null) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private Transform EncontrarPlayerMaisProximo() {
        Transform maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (var jogador in GameManager.instance.jogadores) {
            float distancia = Vector3.Distance(transform.position, jogador.transform.position);
            if (distancia <= campoDeVisao && distancia < menorDistancia) {
                menorDistancia = distancia;
                maisProximo = jogador.pontoCentral.transform;
            }
        }

        return maisProximo;
    }

    private void AtualizarAlvo()
    {
        if (temAlvoFixo)
        {
            tempoRestanteDeFoco -= Time.deltaTime;
            if (tempoRestanteDeFoco <= 0f || target == null || !PlayerNaZona(target))
            {
                temAlvoFixo = false;
            }
        }

        if (!temAlvoFixo)
        {
            Transform novoAlvo = EncontrarPlayerMaisProximo();
            if (novoAlvo != null)
            {
                target = novoAlvo;
                temAlvoFixo = true;
                tempoRestanteDeFoco = tempoDeFoco;
            }
            else
            {
                target = null;
            }
        }
    }

    private bool PlayerNaZona(Transform jogador)
    {
        return Vector3.Distance(transform.position, jogador.position) <= zonaDeAtaque;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, campoDeVisao);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, zonaDeAtaque);
    }
}
