using System;
using System.IO;
using UnityEngine;

public class ProgressManager : MonoBehaviour {

    private string path = "/SaveFiles";
    [SerializeField] private GameObject iconeSalvando;
    public static ProgressManager Instance;


    private void Awake() {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

        path = Application.persistentDataPath + path;
    }

    private void Start(){
        Debug.Log(path);
    }

    public async void SalvarProgresso(Progress progresso) {
        iconeSalvando.SetActive(true);
        string json = JsonUtility.ToJson(progresso);
        await File.WriteAllTextAsync(path, json);
        iconeSalvando.SetActive(false);
        Debug.Log($"Progresso salvo : \n nivel : {progresso.ultimoNivelCompletado}\n sala : {progresso.ultimaSalaCompletada}");
    }

    public Progress CarregarProgresso() {
        string json = File.ReadAllText(path);
        Progress progress = JsonUtility.FromJson<Progress>(json);
        return progress;
    }

}

[Serializable]
public class Progress {

    public int ultimoNivelCompletado;
    public int ultimaSalaCompletada;

}