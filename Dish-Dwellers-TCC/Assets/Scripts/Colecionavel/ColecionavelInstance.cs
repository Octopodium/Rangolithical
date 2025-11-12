using UnityEngine;

public class ColecionavelInstance : MonoBehaviour {
    public ColecionavelData colecionavel;
    public SpriteRenderer sRenderer;

    void Start() {
        RefreshSprite();

        if (ColecionavelController.instance == null) Debug.LogWarning("Colecionaveis só funcionam se o jogo for iniciado apartir do menu!");
        else RefreshVisual();
    }

    public void RefreshSprite() {
        sRenderer.sprite = colecionavel?.sprite;
    }

    public void RefreshVisual() {
        if (ColecionavelController.instance.Tem(colecionavel)) sRenderer.color = Color.white;
        else sRenderer.color = new Color(1,1,1,0.5f);
    }

    public void Coletar() {
        if (ColecionavelController.instance == null) return;

        if (!ColecionavelController.instance.Tem(colecionavel))
            ColecionavelController.instance.Coletar(colecionavel);
        
        RefreshVisual();
    }


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Coletar();
            Destroy(gameObject);
        }
    }
}
