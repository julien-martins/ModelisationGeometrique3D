using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Simplification : MonoBehaviour
{
    class Cluster{
        public Vector3 min, max;
        public List<Vector3> vertices;
        public List<int> indices;
        public Vector3 vertexMean;
        public int vertexMeanIndex;
        public Vector3 center;
        public Vector3 size;
    }

    public MeshGenerator meshGenerator;

    [Range(2, 20)]
    public int Subdivision = 3;
    public float cubeSize = 3.0f;

    public Material mat;

    Vector3[] Bounds;

    Mesh oldMesh;
    Mesh newMesh;

    List<Cluster> clusters_;

    Dictionary<int, int> ReplaceIndex;

    public bool Debugging;

    // Start is called before the first frame update
    void Start()
    {
        clusters_ = new();
        ReplaceIndex = new();
        
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Debug.Log("GETTING MESH");
        oldMesh = meshGenerator.GetMesh();
        Debug.Log("SIMPLIFY MESH ...");
        newMesh = Simplify(oldMesh);

        Debug.Log("Drawing Mesh ...");
        // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshFilter>().mesh = newMesh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnDrawGizmos() {
        if(Debugging == false) return;
        if(clusters_ == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + oldMesh.bounds.min, 0.2f);
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position + oldMesh.bounds.max, 0.2f);

        if(clusters_[1].vertices != null){
            
            foreach(Vector3 vertex in clusters_[1].vertices){
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(vertex, 0.02f);
            }
            
        }

        int debug = 0;
        foreach(Cluster cluster in clusters_){
            //Draw Wire Cube
            if(debug == 0) Gizmos.color = Color.blue;
            else Gizmos.color = Color.white;
            Gizmos.DrawWireCube(cluster.center, cluster.size);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(cluster.min, 0.1f);
            Gizmos.DrawSphere(cluster.max, 0.1f);

            //Draw Vertex in each cluster
            if(cluster.vertices != null){
                
                
                foreach(Vector3 vertex in cluster.vertices){
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(vertex, 0.01f);
                }
                                
            }

            //Draw the vertex mean
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + cluster.vertexMean, 0.2f);
            
            debug++;
        }

    }

    Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    Mesh Simplify(Mesh mesh){    
        clusters_.Clear();


        Vector3 offset;
        if(Subdivision > 1)
            offset = mesh.bounds.size / Subdivision;
        else
            offset = mesh.bounds.size;

        
        for(float i = mesh.bounds.min.x; i <= mesh.bounds.max.x; i += offset.x){
            for(float j = mesh.bounds.min.y; j <= mesh.bounds.max.y; j += offset.y){
                for(float k = mesh.bounds.min.z; k <= mesh.bounds.max.z; k += offset.z){
                    Cluster cluster = new();
                    cluster.center = transform.position + new Vector3(i, j, k);
                    cluster.size = offset;
                    cluster.min = cluster.center - offset/2;
                    cluster.max = cluster.center + offset/2;
                    clusters_.Add(cluster);
                }
            }
        }

        //Put each vertex on his cluster
        Debug.Log("Nombre de vertex: " + mesh.vertices.Length);
        List<Vector3> result = new();
        for(int i = 0; i < mesh.vertices.Length; ++i){
            
            //Find the cluster indice
            //(xmin  - x) / pas = i

            int clusterIndiceX = (int)(Mathf.Floor(Mathf.Abs(clusters_[0].min.x - (transform.position.x + mesh.vertices[i].x) )) / offset.x);
            int clusterIndiceY = (int)(Mathf.Floor(Mathf.Abs(clusters_[0].min.y - (transform.position.y + mesh.vertices[i].y) )) / offset.y);
            int clusterIndiceZ = (int)(Mathf.Floor(Mathf.Abs(clusters_[0].min.z - (transform.position.z + mesh.vertices[i].z) )) / offset.z);

            int indice = clusterIndiceX + Subdivision * (clusterIndiceY + Subdivision * clusterIndiceZ);

            if(clusters_[indice].vertices == null) clusters_[indice].vertices = new List<Vector3>();
            clusters_[indice].vertices.Add(transform.position + mesh.vertices[i]);

            if(clusters_[indice].indices == null) clusters_[indice].indices = new();
            clusters_[indice].indices.Add(i);

        }
        Debug.Log("Fin asssignation des vertex");
        
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();

        //Make the vertex mean of all clusters
        foreach(Cluster cluster in clusters_){
            if(cluster.vertices == null) continue;

            for(int i = 0; i < cluster.vertices.Count; ++i){
                cluster.vertexMean += cluster.vertices[i];
                //Replace this vertex on the triangles
                ReplaceIndex.Add(cluster.indices[i], cluster.vertices.Count);
            }

            cluster.vertexMean /= cluster.vertices.Count;

            //newVertices.Add(transform.position + cluster.vertexMean);
        }

        Debug.Log("Vertex mean: " + clusters_.Count);

        foreach(KeyValuePair<int, int> pair in ReplaceIndex){
            Debug.Log(pair.Key + " | " + pair.Value);
        }
        
        /*
        //Replace all triangles
        for(int i = 0; i < mesh.triangles.Length; ++i){
            newTriangles.Add(ReplaceIndex[mesh.triangles[i]]);
        }

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        */
        
        return mesh;
    }

    List<Vector3> AllVerticesInCube(Mesh mesh, Cluster cluster){
        List<Vector3> result = new();

        for(int i = 0; i < mesh.vertices.Length; ++i){
            if(mesh.vertices[i].x >= cluster.min.x && mesh.vertices[i].y >= cluster.min.y && mesh.vertices[i].z >= cluster.min.z &&
                mesh.vertices[i].x <= cluster.max.x && mesh.vertices[i].y <= cluster.max.y && mesh.vertices[i].z <= cluster.max.z){
                result.Add(mesh.vertices[i]);
            }
        }

        return result;
    }

    void Update(){
        //newMesh = Simplify(oldMesh);
    }

}
