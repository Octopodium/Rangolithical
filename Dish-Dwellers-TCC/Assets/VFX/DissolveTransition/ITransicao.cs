using UnityEngine;
using System.Collections;

public abstract class ITransicao : MonoBehaviour {
    
    abstract public float GetDuracao();
    abstract public IEnumerator PlayTransicao();
    
    protected virtual Texture2D CaptureCameraTexture() {
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

}
