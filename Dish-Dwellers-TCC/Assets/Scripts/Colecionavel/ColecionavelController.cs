using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Linq;

[Serializable]
public struct ColecionavelInfo {
    public string id;
    public bool pego;
    public bool utilizado;

    public ColecionavelInfo(string id = "", bool pego = false, bool utilizado = false) {
        this.id = id;
        this.pego = pego;
        this.utilizado = utilizado;
    }
}

[Serializable]
public struct ReceitaInfo {
    public string id;
    public bool feita;
    public string[] ids_utilizados;

    public ReceitaInfo(string id = "", bool feita = false, string[] ids_utilizados = null) {
        this.id = id;
        this.feita = feita;
        this.ids_utilizados = ids_utilizados;
    }
}

[Serializable]
public struct ColetavelSave {
    public Dictionary<string, ColecionavelInfo> coletaveis;
    public Dictionary<string, ReceitaInfo> receitas;

    public ColetavelSave(Dictionary<string, ColecionavelInfo> coletaveis = default, Dictionary<string, ReceitaInfo> receitas = default) {
        this.coletaveis = coletaveis;
        this.receitas = receitas;
    }

    public ColetavelSaveInFile SaveInFile() {
        return new ColetavelSaveInFile(coletaveis.Values.ToArray(), receitas.Values.ToArray());
    }
}


[Serializable]
public struct ColetavelSaveInFile {
    public ColecionavelInfo[] coletaveis;
    public ReceitaInfo[] receitas;

    public ColetavelSaveInFile(ColecionavelInfo[] coletaveis = default, ReceitaInfo[] receitas = default) {
        this.coletaveis = coletaveis;
        this.receitas = receitas;
    }

    public ColetavelSave GetSave() {
        Dictionary<string, ColecionavelInfo> cs = new Dictionary<string, ColecionavelInfo>();
        if (coletaveis != null) {
            foreach (var col in coletaveis) {
                cs[col.id] = col;
            }
        }
        
        Dictionary<string, ReceitaInfo> rs = new Dictionary<string, ReceitaInfo>();
        if (receitas != null) {
            foreach (var rec in receitas) {
                rs[rec.id] = rec;
            }
        }
        

        return new ColetavelSave(cs,rs);
    }
}



public class ColecionavelController : MonoBehaviour {
    public string save_path = "/colecionaveis.txt";

    public string resource_coletaveis_path = "Colecionaveis";
    public string resource_receitas_path = "Receitas";

    public static ColecionavelController instance;


    public ColetavelSave save;


    Dictionary<string, ColecionavelData> colecionaveis;
    Dictionary<string, ReceitaData> receitas;


    public Action<ColetavelSave> onCarregado, onSalvo;
    public Action onCarregando, onSalvando;


    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        save_path = Application.persistentDataPath + save_path;

        colecionaveis = new Dictionary<string, ColecionavelData>();
        ColecionavelData[] cols = Resources.LoadAll<ColecionavelData>(resource_coletaveis_path);
        foreach (ColecionavelData col in cols) {
            colecionaveis[col.id] = col;
        }

        receitas = new Dictionary<string, ReceitaData>();
        ReceitaData[] recs = Resources.LoadAll<ReceitaData>(resource_coletaveis_path);
        foreach (ReceitaData rec in recs) {
            receitas[rec.id] = rec;
        }

        save = new ColetavelSave();
        save.coletaveis = new Dictionary<string, ColecionavelInfo>();
        save.receitas = new Dictionary<string, ReceitaInfo>();
    }

    void Start() {
        onCarregado += c => { Debug.Log("Colecionaveis carregados: " + string.Join(", ", c.coletaveis.Keys)); Debug.Log("Receitas carregadas: " + string.Join(", ", c.receitas.Keys));};
        onSalvo += c => { Debug.Log("Colecionaveis salvos: " + string.Join(", ", c.coletaveis.Keys)); Debug.Log("Receitas salvas: " + string.Join(", ", c.receitas.Keys)); };

        Carregar();
    }


    #region Coletavel

    // Métodos

    public void Coletar(ColecionavelData colecionado, bool salvar = true) => Coletar(colecionado.id, salvar);
    public void Coletar(string id, bool salvar = true) {
        if (save.coletaveis.ContainsKey(id)) {
            ColecionavelInfo v = save.coletaveis[id];
            v.id = id;
            v.pego = true;
            v.utilizado = false;
            save.coletaveis[id] = v;
        }
        else save.coletaveis.Add(id,new ColecionavelInfo(id, true, false));

        if (salvar) Salvar();
    }



    public bool PodeUtilizar(ColecionavelData colecionado) => PodeUtilizar(colecionado.id);
    public bool PodeUtilizar(string id) {
        if (!Tem(id)) return false;
        return !save.coletaveis[id].utilizado;
    }



    public bool Utilizar(ColecionavelData colecionado, bool salvar = true) => Utilizar(colecionado.id, salvar);
    public bool Utilizar(string id, bool salvar = true) {
        if (!PodeUtilizar(id)) return false;

        ColecionavelInfo v = save.coletaveis[id];
        v.id = id;
        v.pego = true;
        v.utilizado = true;
        save.coletaveis[id] = v;

        if (salvar) Salvar();

        return true;
    }



    public bool Tem(ColecionavelData colecionado) => Tem(colecionado.id);
    public bool Tem(string id) {
        return save.coletaveis.ContainsKey(id) && save.coletaveis[id].pego;
    }




    public bool TemDisponivel(IngredienteData ingrediente) {
        foreach (ColecionavelData coletado in GetColetados()) {
            if (coletado.ingrediente == ingrediente) return true;
        }

        return false;
    }

    public ColecionavelData PegarUtilizavel(IngredienteData ingrediente) {
        foreach (string id in save.coletaveis.Keys) {
            ColecionavelData coletado = colecionaveis[id];
            if (coletado.ingrediente == ingrediente && !save.coletaveis[id].utilizado) return coletado;
        }
        return null;
    }

    // Iterações

    public string[] GetColetadosIds() {
        return save.coletaveis.Keys.ToArray();
    }

    public IEnumerable<ColecionavelData> GetColetados() {
        foreach (string id in save.coletaveis.Keys) {
            ColecionavelData coletado = colecionaveis[id];
            if (save.coletaveis[id].pego)
                yield return coletado;
        }
    }

    public IEnumerable<ColecionavelData> GetUtilizados() {
        foreach (string id in save.coletaveis.Keys) {
            ColecionavelData coletado = colecionaveis[id];
            if (save.coletaveis[id].utilizado)
                yield return coletado;
        }
    }

    #endregion



    #region Receita

    // Métodos

    public bool PodeFazer(ReceitaData receita) {
        foreach (IngredienteData ingrediente in receita.requisitos) {
            if (TemDisponivel(ingrediente))
                return false;
        }

        return true;
    }
    public bool PodeFazer(string receita_id) => PodeFazer(receitas[receita_id]);



    public bool Fazer(ReceitaData receita, bool salvar = true) {
        List<ColecionavelData> colecionaveis = new List<ColecionavelData>();
        foreach (IngredienteData ingrediente in receita.requisitos) {
            ColecionavelData colecionavel = PegarUtilizavel(ingrediente);
            if (colecionavel == null || !PodeUtilizar(colecionavel)) return false;
            colecionaveis.Add(colecionavel);
        }

        List<string> ids_utilizados = new List<string>();

        foreach (ColecionavelData colecionavel in colecionaveis) {
            Utilizar(colecionavel, false);
            ids_utilizados.Add(colecionavel.id);
        }

        if (save.receitas.ContainsKey(receita.id)) {
            ReceitaInfo v = save.receitas[receita.id];
            v.id = receita.id;
            v.feita = true;
            v.ids_utilizados = ids_utilizados.ToArray();
            save.receitas[receita.id] = v;
        }
        else save.receitas.Add(receita.id, new ReceitaInfo(receita.id, true, ids_utilizados.ToArray()));

        if (salvar) Salvar();

        return true;
    }
    public bool Fazer(string receita_id, bool salvar = true) => Fazer(receitas[receita_id], salvar);



    public bool EstaFeita(ReceitaData receita) => EstaFeita(receita.id);
    public bool EstaFeita(string receita_id) {
        return save.receitas.ContainsKey(receita_id) && save.receitas[receita_id].feita;
    }

    // Iterações

    public IEnumerable<ReceitaData> GetReceitas() {
        foreach (string id in save.receitas.Keys) {
            ReceitaData receita = receitas[id];
            yield return receita;
        }
    }

    public IEnumerable<ReceitaData> GetReceitasFeitas() {
        foreach (string id in save.receitas.Keys) {
            if (save.receitas[id].feita) {
                ReceitaData receita = receitas[id];
                yield return receita;
            }
        }
    }

    #endregion


    // Salvar / Carregar

    public async void Salvar(Action<ColetavelSave> callback = null) {
        onSalvando?.Invoke();

        string json = JsonUtility.ToJson(save.SaveInFile());
        await File.WriteAllTextAsync(save_path, json);

        callback?.Invoke(save);
        onSalvo?.Invoke(save);
    }

    public async void Carregar(Action<ColetavelSave> callback = null) {
        onCarregando?.Invoke();

        if (!File.Exists(save_path)) {
            save = new ColetavelSave();
            save.coletaveis = new Dictionary<string, ColecionavelInfo>();
            save.receitas = new Dictionary<string, ReceitaInfo>();
        } else {
            string json = await File.ReadAllTextAsync(save_path);
            save = JsonUtility.FromJson<ColetavelSaveInFile>(json).GetSave();
        }

        callback?.Invoke(save);
        onCarregado?.Invoke(save);
    }
}
