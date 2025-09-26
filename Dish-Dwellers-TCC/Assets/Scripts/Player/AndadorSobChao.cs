using UnityEngine;

public class AndadorSobChao : MonoBehaviour {
    public enum CheckDeChao { FixedUpdate, Update, Script }
    public enum TipoDeCheck { Ray, Box }

    public CheckDeChao modoDeCheck = CheckDeChao.FixedUpdate;
    public TipoDeCheck tipoDeCheck = TipoDeCheck.Ray;
    [HideInInspector] public bool noChao = false;

    public float distanciaCheckChao;

    public Collider chaoAtual;
    Vector3 offsetCheckChao, offsetBase = Vector3.zero;
    GrudaEmChoes grudarNoChao;

    public Vector3 boxRect;

    void Awake() {
        SetDistanciaChao(distanciaCheckChao);
        grudarNoChao = GetComponent<GrudaEmChoes>();
    }


    void Update() {
        if (modoDeCheck != CheckDeChao.Update) return;
        ChecarChao();
    }

    void FixedUpdate() {
        if (modoDeCheck != CheckDeChao.FixedUpdate) return;
        ChecarChao();
    }

    public void SetDistanciaChao(float dist) {
        distanciaCheckChao = dist;
        offsetCheckChao = new Vector3(0, dist, 0);
    }

    public void SetOffsetBase(Vector3 offset) {
        offsetBase = offset;
    }

    public void SetBoxRect(Vector3 rect) {
        boxRect = rect;
    }

    Vector3 GetPosBase() {
        return transform.position + offsetBase + offsetCheckChao;
    }

    public bool ChecarChao() {
        return tipoDeCheck == TipoDeCheck.Ray ? ChecarChaoRay() : ChecarChaoBox();
    }

    public bool ChecarChaoRay() {
        bool isChao = Physics.Raycast(GetPosBase(), Vector3.down, out RaycastHit hit, 2 * distanciaCheckChao, GameManager.instance.layerChao);
        return HandleChecarChao(isChao ? hit.collider : null);
    }

    Collider[] results = new Collider[2];
    Collider PegarColliderIgnorandoASi(int quant) {
        if (quant == 0) return null;
        bool col1isMe = results[0].gameObject == gameObject;
        if (quant == 1) return col1isMe ? null : results[0];
        return col1isMe ? results[1] : results[0];
    }

    public bool ChecarChaoBox() {
        boxRect.y = distanciaCheckChao;
        int quant = Physics.OverlapBoxNonAlloc(GetPosBase(), boxRect, results, Quaternion.identity, GameManager.instance.layerChao);
        return HandleChecarChao(PegarColliderIgnorandoASi(quant));
    }

    bool HandleChecarChao(Collider col) {
        if (grudarNoChao != null) {
            if (col != null) {
                if (col != chaoAtual) {
                    if (col?.tag == "Grudavel") GrudarNoChao(col.transform);
                    else DesgrudarDoChao();
                    chaoAtual = col;
                }
            }
            else {
                DesgrudarDoChao();
                chaoAtual = null;
            }
        }

        noChao = col != null;
        return noChao;
    }

    public void GrudarNoChao(Transform chao) {
        if (chao == null) {
            DesgrudarDoChao();
            return;
        }

        grudarNoChao.Grudar(chao.transform);
    }

    public void DesgrudarDoChao() {
        if (chaoAtual != null && chaoAtual.tag == "Grudavel") {
            grudarNoChao.Desgrudar(chaoAtual.transform);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;

        if (tipoDeCheck == TipoDeCheck.Ray)
            Gizmos.DrawLine(transform.position + offsetBase + offsetCheckChao, transform.position + offsetBase + offsetCheckChao + Vector3.down * distanciaCheckChao * 2);
        else {
            Vector3 pos = GetPosBase();

            Gizmos.DrawWireCube(pos, boxRect * 2f);
        }
    }
}
