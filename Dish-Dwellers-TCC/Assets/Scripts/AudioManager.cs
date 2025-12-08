using UnityEngine;
using UnityEngine.UI;

public enum TiposDeSons {SHIELDHIT, VINESBURNING, PRESSUREPLATE, KNOCKBACK, ENEMYHITSHIELD}

public class AudioManager : MonoBehaviour
{
    private static AudioManager audioManager;
    public AudioClip[] listaDeSons;
    private AudioSource audioSource;
    public AudioSource musicaAtual;

    private float defaultMasterVolume = 0.5f;
    public float currentMasterVolume;

    private string masterVolumeKey = "Master Volume";

    void Awake() {
        if (audioManager == null) {
            audioManager = this;
            //DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        
        LoadVolumeSafe();
    }

    void Start(){
        ConfigurarSliders();
    }

    void ConfigurarSliders(){
        SliderController[] sliders = FindObjectsOfType<SliderController>(true);
        
        foreach(SliderController slider in sliders){
            if(slider != null && slider.sliderObject != null){
                slider.sliderObject.value = currentMasterVolume;
                
                slider.sliderObject.onValueChanged.RemoveAllListeners();
                slider.sliderObject.onValueChanged.AddListener(MudaVolume);
            }
        }
    }

    /// <summary>
    /// Método global para os SFX.
    /// </summary>
    public static void PlaySounds(TiposDeSons sons, float volume = 1f) {
        if(audioManager != null && audioManager.audioSource != null){
            try{
                audioManager.audioSource.PlayOneShot(
                    audioManager.listaDeSons[(int)sons], 
                    volume * audioManager.currentMasterVolume
                );
            }
            catch{
                Debug.Log($"<color=red>Deu erro no audio</color>");
            }
        }
    }

    public void MudaVolume(float value) {
        currentMasterVolume = value;
        audioSource.volume = value;
        
        if(musicaAtual != null){
            musicaAtual.volume = value;
        }
        
        SaveVolume(masterVolumeKey, value);
    }

    private void SaveVolume(string key, float value){
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
    
    private void LoadVolumeSafe(){
        currentMasterVolume = PlayerPrefs.GetFloat(masterVolumeKey, defaultMasterVolume);
        
        if(audioSource != null){
            audioSource.volume = currentMasterVolume;
        }
        
        if(musicaAtual != null){
            musicaAtual.volume = currentMasterVolume;
        }
    }
}