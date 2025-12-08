using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider sliderObject;
    private AudioManager audioManager;

    public void Awake(){
        audioManager = FindObjectOfType<AudioManager>();
        
        if(sliderObject != null && audioManager != null)
        {
            sliderObject.onValueChanged.RemoveAllListeners();
            sliderObject.onValueChanged.AddListener(audioManager.MudaVolume);
            
            if(audioManager != null)
            {
                sliderObject.value = audioManager.currentMasterVolume;
            }
        }
    }

    public void AtualizarSlider(float value){
        if(sliderObject != null)
        {
            sliderObject.value = value;
        }
    }
}