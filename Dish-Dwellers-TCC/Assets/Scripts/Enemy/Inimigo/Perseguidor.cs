using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Perseguidor : Inimigo, IRecebeTemplate 
{
    private enum State { Patrol, Chase, Attack, Stunned }
    private State currentState = State.Patrol;

    [Header("Foco do alvo")] [Space(10)]
    [SerializeField] private float tempoDeFoco = 3f;
    private float tempoRestanteDeFoco;
    private bool temAlvoFixo = false;

    [Header("Reação ao Escudo")]
    [SerializeField] private float tempoCaido = 3f;
    private float tempoCaidoRestante;
    private bool caido = false;

    [Header("Knockback")] [Space(10)]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.3f;

    [Header("Ataque (Dash)")] [Space(10)]
    [SerializeField] private float tempoDeAtaque = 3f; 
    private float tempoDeAtaqueRestante;
    [SerializeField] private float velocidadeDeDash = 10f;
    [SerializeField] private float duracaoDoDash = 0.35f;
    [SerializeField] private float distanciaParaDano = 1.2f;

    private Vector3 dashDirection;

    [Header("Config De Patrulha")] [Space(10)]
    public Transform[] waypoints;
    private int IndexPosicaoAtual = 0;

    [SerializeField] private AnimatorPerseguidor animator;
    private Interagivel interagivel;
    private Carregavel carregavel;
    private NavMeshAgent navAgent;
    private Collider collider;
    private bool podePerseguir = true;


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<AnimatorPerseguidor>();
        interagivel = GetComponentInChildren<Interagivel>();
        carregavel = GetComponentInChildren<Carregavel>();
        collider = GetComponentInChildren<BoxCollider>();

        if (interagivel) interagivel.enabled = false;

    }
    
    private void OnEnable() {
        ResetarParaEstadoInicial();
    }

    private void Start()
    {
        if (waypoints.Length > 0 && navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(waypoints[IndexPosicaoAtual].position);
        }
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
        switch (currentState) {
            case State.Patrol:
                Patrulhar();
                if (_playerNoCampoDeVisao && target != null && podePerseguir) {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                Perseguir();
                if (_playerNaZonaDeAtaque && tempoDeAtaqueRestante <= 0f && target != null) {
                    StartCoroutine(DashAttack());
                }
                if (!_playerNoCampoDeVisao && target == null) {
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

    public void RecebeTemplate(GameObject template) {
        Perseguidor perseguidorTemplate = template.GetComponent<Perseguidor>();
        campoDeVisao = perseguidorTemplate.campoDeVisao;
        zonaDeAtaque = perseguidorTemplate.zonaDeAtaque;
        waypoints = perseguidorTemplate.waypoints;
    }

    #region Patrol & Chase
    public void Perseguir()
    {
        if (!podePerseguir || navAgent == null || !navAgent.isOnNavMesh) return;

        if (_playerNoCampoDeVisao && target != null)
        {
            navAgent.SetDestination(target.position);
            navAgent.updateRotation = false;
            try{
            animator.Persegue(true);
            }catch{}
                
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

            Vector3 dir = transform.position - waypoints[IndexPosicaoAtual].position;
            animator.Olhar(dir);  
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

        while (t < duracaoDoDash)
        {
            transform.position += dashDirection * velocidadeDeDash * Time.deltaTime;

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Escudo") && !caido) {
            AudioManager.PlaySounds(TiposDeSons.ENEMYHITSHIELD);
            Vector3 direcaoImpacto = (transform.position - other.transform.position).normalized;
            CaidoPorEscudo(direcaoImpacto);
        }
        
        if(other.CompareTag("Player") && !caido) {
            Debug.Log("Entrou no hit player");
            other.GetComponent<Player>()?.MudarVida(-1, AnimadorPlayer.fonteDeDano.PORRADA);
        }
    }

    public void CaidoPorEscudo(Vector3 direcaoImpacto)
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

        StartCoroutine(AplicarKnockback(direcaoImpacto));
    }

    private bool agarrado = false;

    public void SendoAgarrado() {
            agarrado = true;
            navAgent.enabled = false;
            Perseguidor perseguidor = GetComponent<Perseguidor>();
            perseguidor.enabled = false;
            collider.isTrigger = false;
    }

    public void Recuperar() {
        if (!carregavel.sendoCarregado) {
            caido = false;
            podePerseguir = true;

            if (navAgent != null && navAgent.isOnNavMesh) {
                navAgent.isStopped = false;
                navAgent.updateRotation = true;
            }

            animator.Desvirar();
            interagivel.enabled = false;

            currentState = (target != null) ? State.Chase : State.Patrol;
        }
    }
    
    private IEnumerator AplicarKnockback(Vector3 direcao)
    {
        direcao.y = 0f; 
        direcao.Normalize();

        float t = 0f;

        while (t < knockbackDuration)
        {
            transform.position += direcao * knockbackForce * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
    } 
    #endregion

    #region Localização de Target
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
    #endregion

    public void ResetarParaEstadoInicial()
    {
        //Para todas as corrotinas
        StopAllCoroutines();

        //Reset de variáveis de estado
        caido = false;
        currentState = State.Patrol;
        podePerseguir = true;
        temAlvoFixo = false;
        agarrado = false; 
        tempoCaidoRestante = 0f;
        tempoDeAtaqueRestante = 0f;
        tempoRestanteDeFoco = 0f;

        //Reset de componentes
        if (navAgent != null)
        {
            navAgent.enabled = true;
            if (navAgent.isOnNavMesh)
            {
                navAgent.isStopped = false;
                navAgent.updateRotation = true;
            }
        }

        if (collider != null)
        {
            collider.isTrigger = true;
        }

        if (interagivel != null)
        {
            interagivel.enabled = false;
        }

        //Reset animacao
        if (animator != null)
        {
            animator.Desvirar();
            animator.Persegue(false);
            animator.Ataca(false);
        }

        // Reset de patrulha
        IndexPosicaoAtual = 0;
        if (waypoints.Length > 0 && navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(waypoints[IndexPosicaoAtual].position);
        }

        Debug.Log("Perseguidor resetado para estado inicial!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, campoDeVisao);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, zonaDeAtaque);
    }
}