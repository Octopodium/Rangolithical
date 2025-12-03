using UnityEngine;

public partial struct ValorGenerico {
    public string CodificadorCustomVector3(Vector3 v) {
        return v.ToString();
    }

    public ValorGenerico DecodificadorCustomVector3(string id) {

        string[] vals = id.Substring(1, id.Length - 2).Split(',');
        Vector3 v = new Vector3(
            float.Parse(vals[0]),
            float.Parse(vals[1]),
            float.Parse(vals[2])
        );

        return new ValorGenerico(typeof(Vector3), v);
    }

    public string CodificadorCustomQuaternion(Quaternion v) {
        return v.ToString();
    }

    public ValorGenerico DecodificadorCustomQuaternion(string id) {

        string[] vals = id.Substring(1, id.Length - 2).Split(',');
        Quaternion v = new Quaternion(
            float.Parse(vals[0]),
            float.Parse(vals[1]),
            float.Parse(vals[2]),
            float.Parse(vals[3])
        );
        
        return new ValorGenerico(typeof(Quaternion), v);
    }
}