using UnityEngine;
using UnityEngine.UI;

public enum TiposDeSons {
SHIELDHIT, VINESBURNING, PRESSUREPLATE, KNOCKBACK
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager audioManager;
    public AudioClip[] listaDeSons;
    private AudioSource audioSource;
    public AudioSource musicaAtual;

    private float defaultMasterVolume = 0.5f;
    public float currentMasterVolume;

    private string masterVolumeKey = "Master Volume";
    public SliderController masterVolumeSlider;

    void Awake() {
        if (audioManager == null) {
            audioManager = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = defaultMasterVolume;
        masterVolumeSlider.sliderObject.onValueChanged.AddListener(MudaVolume); 
        LoadVolume();
    }

    /// <summary>
    /// Método global para os SFX.
    /// Basta chamar onde você necessita de um efeito sonoro e escolher seu tipo, que ele ira usar
    /// diretamente o clipe daquele mesmo tipo contido na lista de sons.
    /// </summary>
    /// <param name="sons"></param>
    /// <param name="volume"></param>
    public static void PlaySounds(TiposDeSons sons, float volume = 1f) {
        audioManager.audioSource.PlayOneShot(audioManager.listaDeSons[(int)sons], volume);
    }

    public void MudaVolume(float value) {
        audioSource.volume = value;
        SaveVolume(masterVolumeKey, value);
    }

    public void SaveVolume(string key, float value){
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
    public void LoadVolume(){
        currentMasterVolume = PlayerPrefs.GetFloat(masterVolumeKey);
        MudaVolume(currentMasterVolume);
        masterVolumeSlider.MudarValueSlider(currentMasterVolume);
    }
}
