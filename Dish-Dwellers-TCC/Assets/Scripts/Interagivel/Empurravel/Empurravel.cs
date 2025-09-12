using UnityEngine;



[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(Interagivel))]
public class Empurravel : MonoBehaviour, InteracaoCondicional {
    [System.Serializable]
    public class DirecaoEmpurrar {
        public bool cima = true;
        public bool baixo = true;
        public bool esquerda = true;
        public bool direita = true;
    }



    public float distBordaInteracao = 0.5f;
    public float paddingTrigger = 0.25f;
    public DirecaoEmpurrar direcoes;

    Rigidbody rb;
    BoxCollider col;
    Interagivel interagivel;


    bool sendoEmpurrado = false;
    Player jogadorEmpurrando = null;
    Vector3 eixo, ultimaPosicaoPlayer, eixoInvertido;

    Vector3 topoOffset;

    bool algoNoCaminho = false;



    void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        interagivel = GetComponent<Interagivel>();

        topoOffset = col.center;
        topoOffset.y += col.size.y /2f;

        CriarTriggersDeInteracao();
    }

    public void CriarTriggersDeInteracao() {
        if (direcoes.direita) CriarTriggerDeInteracao(1, 0);
        if (direcoes.esquerda) CriarTriggerDeInteracao(-1, 0);
        if (direcoes.cima) CriarTriggerDeInteracao(0, 1);
        if (direcoes.baixo) CriarTriggerDeInteracao(0, -1);
    }

    GameObject CriarTriggerDeInteracao(int xDir, int yDir) {
        GameObject trigger = new GameObject("TriggerDeInteracao_" + gameObject.name);
        trigger.layer = LayerMask.NameToLayer("Interagivel");

        trigger.transform.SetParent(transform, false);


        Vector3 direcao = transform.forward * xDir + transform.right * yDir;

        trigger.transform.localPosition = col.center + direcao;
        trigger.transform.localRotation = Quaternion.identity;
        trigger.transform.localScale = Vector3.one;


        BoxCollider boxCol = trigger.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;

        Vector3 colSize = Vector3.one;
        colSize.z = Mathf.Abs(xDir) == 1 ? distBordaInteracao : (col.size.x - paddingTrigger*2f);
        colSize.y = col.size.y - paddingTrigger * 2f;
        colSize.x = Mathf.Abs(yDir) == 1 ? distBordaInteracao : (col.size.z - paddingTrigger*2f);
        boxCol.size = colSize;

        boxCol.center = new Vector3(yDir, 0, xDir) * distBordaInteracao / 2f;


        PontoInteragivel ponto = trigger.AddComponent<PontoInteragivel>();
        ponto.SetInteragivelParaRedirecionar(interagivel);
        ponto.offsetIndicador = Vector3.zero;

        OnTrigger onTrigger = trigger.AddComponent<OnTrigger>();
        onTrigger.onTriggerStayAction += col => OnTriggerAoRedor(col, yDir, xDir);
        onTrigger.onTriggerExitAction += OnSaiuDoTrigger;


        return trigger;
    }

    public bool PodeInteragir(Player jogador) {
        return !jogador.carregador.estaCarregando && jogador.transform.position.y + paddingTrigger < (transform.position.y + topoOffset.y);
    }

    public MotivoNaoInteracao NaoPodeInteragirPois(Player jogador) {
        if (jogador.carregador.aguentaCarregar < Peso.Pesado) return MotivoNaoInteracao.Fraco;
        return MotivoNaoInteracao.Nenhum;
    }


    public void Interagir(Player jogador) {
        if (sendoEmpurrado) {
            SoltarEmpurro();
            return;
        }

        Vector3 posRelativa = (jogador.transform.position - transform.position).normalized;
        Vector3 direcao = Mathf.Abs(posRelativa.x) > Mathf.Abs(posRelativa.z) ? new Vector3(Mathf.Sign(posRelativa.x), 0, 0) : new Vector3(0, 0, Mathf.Sign(posRelativa.z));

        // Posiciona o jogador bem no meio da caixa
        Vector3 novaPosicaoPlayer = transform.right * direcao.x + transform.forward * direcao.z;
        novaPosicaoPlayer += novaPosicaoPlayer * distBordaInteracao;
        novaPosicaoPlayer += transform.position;
        novaPosicaoPlayer.y = jogador.transform.position.y;

        ultimaPosicaoPlayer = novaPosicaoPlayer;
        jogador.Teletransportar(novaPosicaoPlayer);

        jogadorEmpurrando = jogador;
        sendoEmpurrado = true;
        eixo = direcao;
        eixoInvertido = eixo * -1;

        jogadorEmpurrando.empurrando = true;
    }

    void SoltarEmpurro() {
        jogadorEmpurrando.empurrando = false;
        jogadorEmpurrando = null;
        sendoEmpurrado = false;
        eixo = Vector3.zero;
        eixoInvertido = Vector3.zero;
    }


    void OnTriggerAoRedor(Collider col, int x, int z) {
        if (!sendoEmpurrado) return;
        if (col.isTrigger) return;

        if (eixoInvertido.x == x && eixoInvertido.z == z)
            algoNoCaminho = true;
    }

    void OnSaiuDoTrigger(Collider col) {
        if (!sendoEmpurrado) return;
        if (col.gameObject == jogadorEmpurrando.gameObject)
            SoltarEmpurro();
    }

    void FixedUpdate() {
        if (!sendoEmpurrado) return;

        Vector3 variacaoPos = jogadorEmpurrando.transform.position - ultimaPosicaoPlayer;
        ultimaPosicaoPlayer = jogadorEmpurrando.transform.position;


        if (algoNoCaminho) {
            algoNoCaminho = false;
            return;
        }

        if (Vector3.Dot(eixoInvertido, variacaoPos.normalized) < 0f) {
            SoltarEmpurro();
            return;
        }

        
        Vector3 movimento = transform.position;
        movimento.x += Mathf.Abs(eixo.x) * variacaoPos.x;
        movimento.z += Mathf.Abs(eixo.z) * variacaoPos.z;

        transform.position = movimento;
    }

    void OnDrawGizmosSelected() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();

        Vector3 size = col.size;
        size.x += distBordaInteracao * 2;
        size.z += distBordaInteracao * 2;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + col.center, size);
    }
}
