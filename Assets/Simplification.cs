using System;
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

    MeshGenerator meshGenerator;

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

    [Range(0, 100)] public int indiceTest = 1;
    
    [Range(0, 100)] public int vertexTest = 0;

    // Start is called before the first frame update
    void Start()
    {
        meshGenerator = GetComponent<MeshGenerator>();

        clusters_ = new();
        ReplaceIndex = new();
        
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Debug.Log("GETTING MESH");
        oldMesh = meshGenerator.GetMesh();
        Debug.Log(oldMesh);
        Debug.Log("SIMPLIFY MESH ...");
        newMesh = Simplify(oldMesh);

        Debug.Log("Drawing Mesh ...");
        // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshFilter>().mesh = newMesh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
    
        meshGenerator.SaveMeshOFF("bunny_simple.off", newMesh);
    }

    void OnDrawGizmos() {
        if(Debugging == false) return;
        if(clusters_ == null) return;

        Gizmos.DrawSphere(Vector3.zero, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + oldMesh.bounds.min, 0.2f);
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(transform.position + oldMesh.bounds.max, 0.2f);
        
        Vector3 offset = oldMesh.bounds.size / Subdivision;

        if(clusters_[indiceTest].vertices != null){
            
            foreach(Vector3 vertex in clusters_[indiceTest].vertices){
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(vertex, 0.01f);
            }
            
        }

        int debug = 0;
        foreach(Cluster cluster in clusters_){
            //Draw Wire Cube
            if(debug == 0) Gizmos.color = Color.blue;
            else Gizmos.color = Color.white;
            Gizmos.DrawWireCube(cluster.center, cluster.size);

            
            //Draw Vertex in each cluster
            /*
            if(cluster.vertices != null){
                
                
                foreach(Vector3 vertex in cluster.vertices){
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(vertex, 0.01f);
                }
                                
            }
            */

            //Draw the vertex mean
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(cluster.vertexMean, 0.02f);
            
            debug++;
        }

    }

    Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    Mesh Simplify(Mesh mesh){    
        clusters_.Clear();

        Vector3[] vertices = mesh.vertices;

        Vector3 offset;
        if(Subdivision > 1)
            offset = mesh.bounds.size / Subdivision;
        else
            offset = mesh.bounds.size;

        
        for(float k = mesh.bounds.min.z; k <= mesh.bounds.max.z; k += offset.z){
            for(float j = mesh.bounds.min.y; j <= mesh.bounds.max.y; j += offset.y){
                for(float i = mesh.bounds.min.x; i <= mesh.bounds.max.x; i += offset.x){
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
        Debug.Log("Nombre de vertex: " + vertices.Length);
        List<Vector3> result = new();
        //for(int i = 0; i < mesh.vertices.Length; ++i){
        for(int i = 0; i < vertices.Length; ++i){
            
            foreach(Cluster cluster in clusters_){
                if(VertexInCluster(vertices[i], cluster)){
                    if(cluster.vertices == null) cluster.vertices = new();
                    cluster.vertices.Add(transform.position + vertices[i]);

                    if(cluster.indices == null) cluster.indices = new();
                    cluster.indices.Add(i);
                }
            }
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
                

                ReplaceIndex.Add(cluster.indices[i], newVertices.Count);

            }

            cluster.vertexMean /= cluster.vertices.Count;

            newVertices.Add(cluster.vertexMean);
        }

        /*

        Debug.Log("Vertex mean: " + clusters_.Count);

        foreach(KeyValuePair<int, int> pair in ReplaceIndex){
            Debug.Log(pair.Key + " | " + pair.Value);
        }

        */
        
        
        //Replace all triangles
        
        Debug.Log("Replacing triangle ...");
        

        
        for(int i = 0; i < mesh.triangles.Length; i += 3) {

            if(!ReplaceIndex.ContainsKey(mesh.triangles[i])) continue;
            if(!ReplaceIndex.ContainsKey(mesh.triangles[i+1])) continue;
            if(!ReplaceIndex.ContainsKey(mesh.triangles[i+2])) continue;

            int newIndex1 = ReplaceIndex[mesh.triangles[i]];
            int newIndex2 = ReplaceIndex[mesh.triangles[i + 1]];
            int newIndex3 = ReplaceIndex[mesh.triangles[i + 2]];

            if(newIndex1 != newIndex2 && newIndex2 != newIndex3) {
                newTriangles.Add(newIndex1);
                newTriangles.Add(newIndex2);
                newTriangles.Add(newIndex3);
            }
        }
        


        Debug.Log("Nb of new vertices: ");
        Debug.Log(newVertices.Count);

        Mesh newMesh = new();

        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles =  newTriangles.ToArray();

        return newMesh;
    }

    bool VertexInCluster(Vector3 vertex, Cluster cluster){
        if(vertex.x >= cluster.min.x && vertex.y >= cluster.min.y && vertex.z >= cluster.min.z &&
            vertex.x <= cluster.max.x && vertex.y <= cluster.max.y && vertex.z <= cluster.max.z){
            return true;
        }

        return false;
    }

    void Update(){
        //newMesh = Simplify(oldMesh);
    }

}
