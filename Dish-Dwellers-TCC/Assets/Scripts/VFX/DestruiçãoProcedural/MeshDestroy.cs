using UnityEngine;
using System.Collections.Generic;

public class MeshDestroy : MonoBehaviour{
    [SerializeField] private int cutCascades = 1;
    [SerializeField] private float explosionForce = 0.0f;
    [Tooltip("Determines wether the cutting plane plane position will be centered on the original mesh`s bounds, or on a random position inside of it.")]
    [SerializeField] private bool centeredCut = false;
    [Tooltip("Determines the scale of the generated mesh parts")]
    [SerializeField][Range(0, 1)]private float meshPartsScale = 1.0f;

    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();
    private GameObject[] generatedMeshParts;


    private void OnEnable(){
        if(generatedMeshParts != null){
            for(int i = 0; i < generatedMeshParts.Length; i++){
                Destroy(generatedMeshParts[i]);
                generatedMeshParts[i] = null;
            }
        }
    }

    private void Start(){
        generatedMeshParts = new GameObject[(int)Mathf.Pow(2, cutCascades)];
    }

    public void DestroyMesh() {
        Mesh originalMesh = GetComponent<MeshFilter>().mesh;
        originalMesh.RecalculateBounds();
        List<MeshPart> meshParts = new List<MeshPart>();
        List<MeshPart> subMeshParts = new List<MeshPart>();

        MeshPart mainMeshPart = new MeshPart(originalMesh);
        
        meshParts.Add(mainMeshPart);
        for(int i = 0; i < cutCascades; i++) {
            for(int j = 0; j < meshParts.Count; j++) {
                Bounds bounds = meshParts[j].bounds;
                bounds.Expand(0.5f);
                Plane plane;
                if(!centeredCut){
                    plane = new Plane(Random.onUnitSphere, new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z)
                    ));
                }
                else{
                    plane = new Plane(Random.onUnitSphere, bounds.center);
                }
                subMeshParts.Add(GenerateMeshPart(meshParts[j], plane, true));
                subMeshParts.Add(GenerateMeshPart(meshParts[j], plane, false));
            }
            meshParts = new List<MeshPart>(subMeshParts);
            subMeshParts.Clear();
        }
        for( int i = 0; i < meshParts.Count; i++) {
            meshParts[i].MakeGameObject(this, meshPartsScale);
            meshParts[i].newObject.GetComponent<Rigidbody>().AddForceAtPosition(meshParts[i].bounds.center * explosionForce, transform.position);
            generatedMeshParts[i] = meshParts[i].newObject;
        }
        gameObject.SetActive(false);
        // Destroy(gameObject);
    }

    private MeshPart GenerateMeshPart(MeshPart originalMeshPart, Plane plane, bool left) {
        MeshPart newMeshPart = new MeshPart();
        Ray ray1 = new Ray();
        Ray ray2 = new Ray();
        Debug.Log($"originalMeshPart number of tris : {originalMeshPart.tris.Length}");
        for(int i = 0; i < originalMeshPart.tris.Length; i++) {
            Debug.Log("batch" + i);
            int[] tri = originalMeshPart.tris[i];
            edgeSet = false;
            for(int j = 0; j < tri.Length; j += 3) {
                Debug.Log("tri id " + j);
                bool sideA = plane.GetSide(originalMeshPart.vertices[tri[j]]) == left;
                bool sideB = plane.GetSide(originalMeshPart.vertices[tri[j + 1]]) == left;
                bool sideC = plane.GetSide(originalMeshPart.vertices[tri[j + 2]]) == left;
                int vertexOnTheLeft = (sideA ? 1 : 0) + (sideB ? 1 : 0) + (sideC ? 1 : 0);
                if(vertexOnTheLeft == 0) continue;
                if(vertexOnTheLeft == 3) {
                    newMeshPart.AddTriangle(i,
                        new Vector3[]{
                            originalMeshPart.vertices[tri[j]],
                            originalMeshPart.vertices[tri[j + 1]],
                            originalMeshPart.vertices[tri[j + 2]]
                        },
                        new Vector3[]{
                            originalMeshPart.normals[tri[j]],
                            originalMeshPart.normals[tri[j + 1]],
                            originalMeshPart.normals[tri[j + 2]]
                        },
                        new Vector2[]{
                            originalMeshPart.UVs[tri[j]],
                            originalMeshPart.UVs[tri[j + 1]],
                            originalMeshPart.UVs[tri[j + 2]],
                        }
                    );
                    continue;
                }
                int index = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;
                ray1.origin = originalMeshPart.vertices[tri[j + index]];
                ray2.origin = originalMeshPart.vertices[tri[j + index]];
                ray1.direction = originalMeshPart.vertices[tri[j + ((index + 1) % 3)]] - originalMeshPart.vertices[tri[j + index]];
                ray2.direction = originalMeshPart.vertices[tri[j + ((index + 2) % 3)]] - originalMeshPart.vertices[tri[j + index]];
                plane.Raycast(ray1, out float cutpoint1);
                plane.Raycast(ray2, out float cutpoint2);

                float lerp1 = cutpoint1 / ray1.direction.magnitude;
                float lerp2 = cutpoint2 / ray2.direction.magnitude;
                Vector3 vertex1 = ray1.origin + ray1.direction.normalized * cutpoint1;
                Vector3 vertex2 = ray2.origin + ray2.direction.normalized * cutpoint2;
                int anchor = tri[j + index];
                int vertexb = tri[j + ((index + 1) % 3)];
                int vertexc = tri[j + ((index + 2) % 3)];

                AddEdge(i,
                    newMeshPart,
                    left ? plane.normal * -1 : plane.normal,
                    vertex1,
                    vertex2,
                    Vector2.Lerp(originalMeshPart.UVs[anchor], originalMeshPart.UVs[vertexb], lerp1),
                    Vector2.Lerp(originalMeshPart.UVs[anchor], originalMeshPart.UVs[vertexc], lerp2)
                );
                if(vertexOnTheLeft == 1) {
                    newMeshPart.AddTriangle(i,
                        new Vector3[]{
                            originalMeshPart.vertices[anchor],
                            vertex1,
                            vertex2
                        },
                        new Vector3[]{
                            originalMeshPart.normals[anchor],
                            Vector3.Lerp(originalMeshPart.normals[anchor], originalMeshPart.normals[vertexb], lerp1),
                            Vector3.Lerp(originalMeshPart.normals[anchor], originalMeshPart.normals[vertexc], lerp2),
                        },
                        new Vector2[]{
                            originalMeshPart.UVs[anchor],
                            Vector2.Lerp(originalMeshPart.UVs[anchor], originalMeshPart.UVs[vertexb], lerp1),
                            Vector2.Lerp(originalMeshPart.UVs[anchor], originalMeshPart.UVs[vertexc], lerp2)
                        }
                    );
                    continue;
                }
                if(vertexOnTheLeft == 2) {
                    newMeshPart.AddTriangle(i,
                        new Vector3[]{
                            ray1.origin + ray1.direction.normalized * cutpoint1,
                            originalMeshPart.vertices[tri[j + ((index + 1) % 3)]],
                            originalMeshPart.vertices[tri[j + ((index + 2) % 3)]]
                        },
                        new Vector3[]{
                            Vector3.Lerp(originalMeshPart.normals[tri[j + index]], originalMeshPart.normals[tri[j + ((index + 1) % 3)]], lerp1),
                            originalMeshPart.normals[tri[j + ((index + 1) % 3)]],
                            originalMeshPart.normals[tri[j + ((index + 2) % 3)]]
                        },
                        new Vector2[]{
                            Vector2.Lerp(originalMeshPart.UVs[tri[j + index]], originalMeshPart.UVs[tri[j + ((index + 1) % 3)]], lerp1),
                            originalMeshPart.UVs[tri[j + ((index + 1) % 3)]],
                            originalMeshPart.UVs[tri[j + ((index + 2) % 3)]]
                        }
                    );  
                    newMeshPart.AddTriangle(i,
                        new Vector3[]{
                            ray1.origin + ray1.direction.normalized * cutpoint1,
                            originalMeshPart.vertices[tri[j + ((index + 2) % 3)]],
                            ray2.origin + ray2.direction.normalized * cutpoint2
                        },
                        new Vector3[]{
                            Vector3.Lerp(originalMeshPart.normals[tri[j + index]], originalMeshPart.normals[tri[j + ((index + 1) % 3)]], lerp1),
                            originalMeshPart.normals[tri[j + ((index + 2) % 3)]],
                            Vector3.Lerp(originalMeshPart.normals[tri[j + index]], originalMeshPart.normals[tri[j + ((index + 2) % 3)]], lerp2)
                        },
                        new Vector2[]{
                            Vector2.Lerp(originalMeshPart.UVs[tri[j + index]], originalMeshPart.UVs[tri[j + ((index + 1) % 3)]], lerp1),
                            originalMeshPart.UVs[tri[j + ((index + 2) % 3)]],
                            Vector2.Lerp(originalMeshPart.UVs[tri[j + index]], originalMeshPart.UVs[tri[j + ((index + 2) % 3)]], lerp2),
                        }
                    );
                    continue;
                } 
            }
        }
        newMeshPart.FillArrays();
        return newMeshPart;
    }

    private void AddEdge (int subMeshIndex, MeshPart meshPart, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2) {
        if(!edgeSet) {
            edgeSet = true;
            edgeVertex = vertex1;
            edgeUV = uv1;
        }
        else {
            edgePlane.Set3Points(edgeVertex, vertex1, vertex2);
            meshPart.AddTriangle(subMeshIndex,
                new Vector3[]{
                    edgeVertex,
                    edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2,
                    edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1,
                },
                new Vector3[]{
                    normal, 
                    normal,
                    normal
                },
                new Vector2[]{
                    edgeUV,
                    uv1,
                    uv2
                }
            );
        }
    }

}

public class MeshPart {

    private List<Vector3> _vertices = new List<Vector3>(); 
    private List<List<int>> _tris = new List<List<int>>();
    private List<Vector3> _normals = new List<Vector3>();
    private List<Vector2> _UVs = new List<Vector2>();
    public Vector3[] vertices;
    public int[][] tris;
    public Vector3[] normals;
    public Vector2[] UVs;
    public GameObject newObject;
    public Bounds bounds = new Bounds();


    public MeshPart() {}

    public MeshPart(Mesh originalMesh) {
        vertices = originalMesh.vertices;
        tris = new int[originalMesh.subMeshCount][];
        normals = originalMesh.normals;
        UVs = originalMesh.uv;
        bounds = originalMesh.bounds;
        for(int i = 0; i < originalMesh.subMeshCount; i++) tris[i] = originalMesh.GetTriangles(i);
    }

    public void AddTriangle(int submesh, Vector3[] vertices, Vector3[] normals, Vector2[] UVs){
        if(_tris.Count - 1 < submesh ) _tris.Add(new List<int>());
        for (int i = 0; i < 3; i++) {
            _tris[submesh].Add(_vertices.Count);
            _vertices.Add(vertices[i]);
            _normals.Add(normals[i]);
            _UVs.Add(UVs[i]);

            bounds.min = Vector3.Min(bounds.min, vertices[i]);
            bounds.max = Vector3.Max(bounds.max, vertices[i]);
        }
    }

    public void FillArrays() {
        vertices = _vertices.ToArray();
        normals = _normals.ToArray();
        UVs = _UVs.ToArray();
        tris = new int[_tris.Count][];
        for(int i = 0; i < _tris.Count; i++){
            tris[i] = _tris[i].ToArray();
        }
    }

    public void MakeGameObject(MeshDestroy originalObject, float partScale) {
        newObject = new GameObject(originalObject.name);
        originalObject.transform.GetPositionAndRotation(
            out Vector3 originalPosition,
            out Quaternion originalRotation
        );
        newObject.transform.SetPositionAndRotation(
            originalPosition,
            originalRotation
        );
        newObject.transform.localScale = originalObject.transform.localScale * partScale;

        Mesh mesh = new Mesh();
        mesh.name = originalObject.GetComponent<MeshFilter>().mesh.name;
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = UVs;
        for(int i = 0; i < tris.Length; i++) mesh.SetTriangles(tris[i], i, true);
        bounds = mesh.bounds;

        MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
        meshRenderer.materials = originalObject.GetComponent<MeshRenderer>().materials;
        MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshCollider meshCollider = newObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        Rigidbody rigidbody = newObject.AddComponent<Rigidbody>();
        
    }

}
