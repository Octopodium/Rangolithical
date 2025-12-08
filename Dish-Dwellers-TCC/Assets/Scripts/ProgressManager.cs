using System;
using System.IO;
using UnityEngine;

public class ProgressManager : MonoBehaviour {

    private string path = "/SaveFiles";
    [SerializeField] private GameObject iconeSalvando;
    public static ProgressManager Instance;
    public Progress loadedProgress;

    public System.Action OnProgressChange;


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
        if(!File.Exists(path)){
            //File.Create(path);
            loadedProgress = new Progress{
                ultimoNivelAlcancado = 1,
                ultimaSalaAlcancada = 1,
            };
            Debug.Log("Salvando progresso pela primeira vez.");
            //SalvarProgresso(progress);
            OnProgressChange?.Invoke();
        }
        else {
            Debug.Log("Ja existe um saveFile.");
            loadedProgress = CarregarProgresso();
            Debug.Log($"Sala : {loadedProgress.ultimaSalaAlcancada} \nFase : {loadedProgress.ultimoNivelAlcancado}");
            OnProgressChange?.Invoke();
        }
    }

    public async void SalvarProgresso(Progress progresso) {
        Debug.Log("Iniciando salvamento...");
        iconeSalvando.SetActive(true);
        string json = JsonUtility.ToJson(progresso);
        await File.WriteAllTextAsync(path, json);
        iconeSalvando.SetActive(false);
        Debug.Log($"Progresso salvo : \n nivel : {progresso.ultimoNivelAlcancado}\n sala : {progresso.ultimaSalaAlcancada}");
        OnProgressChange?.Invoke();
    }

    public Progress CarregarProgresso() {
        if(!File.Exists(path)) return null;
        
        string json = File.ReadAllText(path);
        Progress progress = JsonUtility.FromJson<Progress>(json);
        return progress;
    }

    public bool SalaJaVisitada(SalaInfo sala) {
        if (sala.numeroDaRegiao < loadedProgress.ultimoNivelAlcancado) return true;
        return sala.numeroDaRegiao == loadedProgress.ultimoNivelAlcancado && sala.numeroDaSala <= loadedProgress.ultimaSalaAlcancada;
    }


    public void LiberarTodas() {
        loadedProgress = new Progress{
            ultimoNivelAlcancado = 5,
            ultimaSalaAlcancada = 5,
        };

        SalvarProgresso(loadedProgress);
    }

    public void Zerar() {
        loadedProgress = new Progress{
            ultimoNivelAlcancado = 1,
            ultimaSalaAlcancada = 1,
        };

        SalvarProgresso(loadedProgress);
    }

}

[Serializable]
public class Progress {

    public int ultimoNivelAlcancado;
    public int ultimaSalaAlcancada;

}