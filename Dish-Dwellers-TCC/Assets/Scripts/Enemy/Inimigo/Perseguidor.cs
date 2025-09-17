using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Perseguidor : Inimigo {

    [Header("Foco do alvo")]
    [SerializeField] private float tempoDeFoco = 3f;
    private float tempoRestanteDeFoco;
    private bool temAlvoFixo = false;
    private Vector3 direction;

    [Header("Reação ao Escudo")]
    [SerializeField] private float tempoCaido = 3f; 
    private float tempoCaidoRestante;
    private bool caido = false;

    [Header("Confg De Patrulha")]
    public Transform[] waypoints;
    private int IndexPosicaoAtual = 0;

    [SerializeField] private AnimatorPerseguidor animator;
    private NavMeshAgent navAgent;
    private bool podePerseguir = true;

    private void Awake() {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<AnimatorPerseguidor>();

        if (waypoints.Length > 0) {
            navAgent.SetDestination(waypoints[IndexPosicaoAtual].position);
        }
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
        Patrulhar();
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

    public void Patrulhar() {
        if (!navAgent.pathPending && navAgent.remainingDistance < 1) {
            IndexPosicaoAtual = (IndexPosicaoAtual + 1) % waypoints.Length;
            navAgent.SetDestination(waypoints[IndexPosicaoAtual].position);
            animator.Persegue(false);
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

            animator.VirarDeCabecaPraBaixo();
        }
    }

    private void Recuperar() {
        caido = false;
        podePerseguir = true;

        if (navAgent != null && navAgent.isOnNavMesh) {
            navAgent.isStopped = false;
            navAgent.updateRotation = true;

            animator.Desvirar();
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
