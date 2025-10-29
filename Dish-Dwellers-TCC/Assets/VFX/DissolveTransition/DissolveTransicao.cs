using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DissolveTransicao :  ITransicao {
    
    [SerializeField] private float dellay = 0.0f;
    [SerializeField] private float duracaoDaTransicao = 2.0f;
    private Image image;
    private static int edgeID = Shader.PropertyToID("_Edge");
    private static int mainTextureID = Shader.PropertyToID("_MainTexture");


    private void Awake() {
        image = GetComponent<Image>();
    }

    private void OnEnable() {
        StartCoroutine(PlayTransicao());
    }

    public override float GetDuracao() => dellay + duracaoDaTransicao;

    private Texture2D CaptureCameraTexture() {
        Camera cameraAlvo = Camera.main;
        RenderTexture texturaTemporaria = new RenderTexture(cameraAlvo.pixelWidth, cameraAlvo.pixelHeight, 24);
        cameraAlvo.targetTexture = texturaTemporaria;
        cameraAlvo.Render();
        RenderTexture.active = texturaTemporaria;
        Texture2D printCamera = new Texture2D(cameraAlvo.pixelWidth, cameraAlvo.pixelHeight, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(texturaTemporaria, printCamera);

        // CleanUp:
        cameraAlvo.targetTexture = null;
        RenderTexture.active = null;
        texturaTemporaria.Release();
        Destroy(texturaTemporaria);

        return printCamera;
    }

    public override IEnumerator PlayTransicao() {
        float timer = 0.0f;
        image.materialForRendering.SetFloat(edgeID, 0);
        image.materialForRendering.SetTexture(mainTextureID, CaptureCameraTexture());
        yield return new WaitForSecondsRealtime(dellay);
        while(timer < duracaoDaTransicao) {
            timer += Time.unscaledDeltaTime;
            // materialPropertyBlock.SetFloat(edgeID, timer / duracaoDaTransicao);
            image.materialForRendering.SetFloat(edgeID, timer / duracaoDaTransicao);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
