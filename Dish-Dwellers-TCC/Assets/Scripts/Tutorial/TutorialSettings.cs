using UnityEngine;
using System.Collections;

public class TutorialSettings : MonoBehaviour
{
    public GameObject tutorialTroca;

    [Space(15), Header("Configurações fade in :")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private bool transicaoNoInicio = false;
    [SerializeField] private float transitionSpeed = 1.0f;
    [SerializeField] private float transitionDellay = 0.0f;
    [SerializeField, Range(1.0f, 50.0f)] private float startingRadius = 10.0f;
    [SerializeField] private Vector3 startingPosition = new Vector3();
    private MaterialPropertyBlock materialPropertyBlock;
    private readonly int radiusID = Shader.PropertyToID("_Radius");
    private readonly int startingPositionID = Shader.PropertyToID("_PointOfContact");


    private void Awake() {
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    public void Start(){
        if(GameManager.instance.modoDeJogo == ModoDeJogo.SINGLEPLAYER && tutorialTroca != null)
            tutorialTroca.SetActive(true);
        foreach(Renderer render in renderers) {
            render.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(radiusID, startingRadius);
            materialPropertyBlock.SetVector(startingPositionID, startingPosition);
            render.SetPropertyBlock(materialPropertyBlock);
        }
        if(transicaoNoInicio)
            FadeIn();
    }

    public void FadeIn(){
        StartCoroutine(FadeInTutorial());
    }

    IEnumerator FadeInTutorial() {
        yield return new WaitForSeconds(transitionDellay);
        while( startingRadius > 0) {
            startingRadius -= Time.fixedDeltaTime * transitionSpeed;
            foreach(Renderer render in renderers) {
                render.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat(radiusID, startingRadius);
                render.SetPropertyBlock(materialPropertyBlock);
            }
            yield return new WaitForFixedUpdate();
        }
        startingRadius = 0;

        foreach(Renderer render in renderers){
            render.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(radiusID, startingRadius);
            render.SetPropertyBlock(materialPropertyBlock);
        }
    }

}
