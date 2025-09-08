using UnityEngine;

public class MeshFragmentFade : MonoBehaviour{ 

    private int opacityHash = Shader.PropertyToID("Opacity");
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private float lifetime = 2.0f;
    private float age = 0.0f;
    private float fadeDuration = 2.0f;
    private float fadeProgression = 0.0f;
    private Vector3 initialScale;

    private void Start(){
        meshRenderer = GetComponent<MeshRenderer>();
        initialScale = transform.localScale;
    }   

    private void FixedUpdate(){
        if(age < lifetime)
            age += Time.fixedDeltaTime;
        else if (fadeProgression < fadeDuration){
            float progression = fadeProgression / fadeDuration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, progression);
            materialPropertyBlock.SetFloat(opacityHash, (1f - progression) * 2f); // Precisa ser um valor entre 0 e 2 para funcionar com o Dithering node  
            fadeProgression += Time.fixedDeltaTime;
        }
        else
            Destroy(gameObject);
    }

}
