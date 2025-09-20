using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Perseguidor : Inimigo
{
    private enum State { Patrol, Chase, Attack, Stunned }
    private State currentState = State.Patrol;

    [Header("Foco do alvo")]
    [SerializeField] private float tempoDeFoco = 3f;
    private float tempoRestanteDeFoco;
    private bool temAlvoFixo = false;

    [Header("Reação ao Escudo")]
    [SerializeField] private float tempoCaido = 3f;
     private float tempoCaidoRestante;
    private bool caido = false;

    [Header("Ataque (Dash)")]
    [SerializeField] private float tempoDeAtaque = 3f; 
    private float tempoDeAtaqueRestante;
    [SerializeField] private float velocidadeDeDash = 10f;
    [SerializeField] private float duracaoDoDash = 0.35f;
    [SerializeField] private float distanciaParaDano = 1.2f;

    private Vector3 dashDirection;

    [Header("Config De Patrulha")]
    public Transform[] waypoints;
    private int IndexPosicaoAtual = 0;

    [SerializeField] private AnimatorPerseguidor animator;
    private Interagivel interagivel;
    private Carregavel carregavel;
    private NavMeshAgent navAgent;
    private bool podePerseguir = true;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<AnimatorPerseguidor>();
        interagivel = GetComponentInChildren<Interagivel>();
        carregavel = GetComponentInChildren<Carregavel>();
        if (interagivel) interagivel.enabled = false;

        if (waypoints.Length > 0 && navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(waypoints[IndexPosicaoAtual].position);
        }
    }

    private void Start()
    {
        target = EncontrarPlayerMaisProximo();
        if (target != null)
        {
            temAlvoFixo = true;
            tempoRestanteDeFoco = tempoDeFoco;
            currentState = State.Chase;
        }
    }

    private void FixedUpdate()
    {
        if (caido && !carregavel.sendoCarregado)
        {
            tempoCaidoRestante -= Time.deltaTime;
            if (tempoCaidoRestante <= 0f)
            {
                Recuperar();
                return;
            }
        }

        tempoDeAtaqueRestante -= Time.deltaTime;

        base.ChecagemDeZonas();
        AtualizarAlvo();

        //Mudando esse cara pra state machine
        switch (currentState)
        {
            case State.Patrol:
                Patrulhar();
                if (_playerNoCampoDeVisao && target != null && podePerseguir)
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                Perseguir();
                if (_playerNaZonaDeAtaque && tempoDeAtaqueRestante <= 0f && target != null)
                {
                    StartCoroutine(DashAttack());
                }
                if (!_playerNoCampoDeVisao && target == null)
                {
                    currentState = State.Patrol;
                }
                break;

            case State.Attack:
                //feito na coroutine
                break;

            case State.Stunned:
                //lógica do CaidoPorEscudo
                break;
        }
    }

    #region Patrol & Chase
    public void Perseguir()
    {
        if (!podePerseguir || navAgent == null || !navAgent.isOnNavMesh) return;

        if (_playerNoCampoDeVisao && target != null)
        {
            navAgent.SetDestination(target.position);
            navAgent.updateRotation = false;
            animator.Persegue(true);

            Vector3 dirOlhar = transform.position - target.position;
            dirOlhar.y = 0;
            animator.Olhar(dirOlhar);
        }
    }

    public void Patrulhar()
    {
        if (navAgent == null || waypoints.Length == 0) return;

        if (!navAgent.pathPending && navAgent.remainingDistance < 1f)
        {
            IndexPosicaoAtual = (IndexPosicaoAtual + 1) % waypoints.Length;
            navAgent.SetDestination(waypoints[IndexPosicaoAtual].position);
            animator.Persegue(false);
        }
    }
    #endregion

    #region Ataca com Dash
    private IEnumerator DashAttack()
    {
        currentState = State.Attack;
        tempoDeAtaqueRestante = tempoDeAtaque;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.updateRotation = false;
        }

        animator.Ataca(true);

        dashDirection = (target.position - transform.position).normalized;
        dashDirection.y = 0f;

        float t = 0f;
        bool deuDano = false;

        while (t < duracaoDoDash)
        {
            transform.position += dashDirection * velocidadeDeDash * Time.deltaTime;

            if (!deuDano && target != null)
            {
                float dist = Vector3.Distance(transform.position, target.position);
                if (dist <= distanciaParaDano)
                {
                    
                    //target.GetComponent<PlayerVida>()?.TakeDamage(dano);
                    deuDano = true;
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        animator.Ataca(false);

        if (!caido)
        {
            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.isStopped = false;
                navAgent.updateRotation = true;
            }
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Stunned;
        }
    }
    #endregion

    #region Escudo & Stun
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Escudo") && !caido)
        {
            CaidoPorEscudo();
        }
    }

    public void CaidoPorEscudo()
    {
        interagivel.enabled = true;

        caido = true;
        tempoCaidoRestante = tempoCaido;
        podePerseguir = false;
        currentState = State.Stunned;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.updateRotation = false;
        }

        animator.VirarDeCabecaPraBaixo();
    }

    public void Recuperar()
    {
        if (!carregavel.sendoCarregado)
        {
            caido = false;
            podePerseguir = true;

            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.isStopped = false;
                navAgent.updateRotation = true;
            }

            animator.Desvirar();
            interagivel.enabled = false;

            currentState = (target != null) ? State.Chase : State.Patrol;
        }
    }
    #endregion

    #region Localização de Target
    private Transform EncontrarPlayerMaisProximo()
    {
        Transform maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (var jogador in GameManager.instance.jogadores)
        {
            float distancia = Vector3.Distance(transform.position, jogador.transform.position);
            if (distancia <= campoDeVisao && distancia < menorDistancia)
            {
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
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, campoDeVisao);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, zonaDeAtaque);
    }
}
