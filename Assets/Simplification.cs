using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Simplification : MonoBehaviour
{
    class Cluster{
        public Vector3 min, max;
        public List<Vector3> vertices;
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

    // Start is called before the first frame update
    void Start()
    {
        clusters_ = new();
        
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Debug.Log("GETTING MESH");
        oldMesh = meshGenerator.GetMesh();
        Debug.Log("SIMPLIFY MESH ...");
        Debug.Log(oldMesh);
        newMesh = Simplify(oldMesh);

        Debug.Log("Drawing Mesh ...");
        // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshFilter>().mesh = newMesh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnDrawGizmos() {
        if(clusters_ == null) return;

        foreach(Cluster cluster in clusters_){
            //Draw Wire Cube
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + cluster.center, cluster.size/2);

            //Draw min and max cluster
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + cluster.min, 0.02f);
            Gizmos.DrawSphere(transform.position + cluster.max, 0.02f);

            //Draw the vertex mean
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + cluster.vertexMean, 0.2f);
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
                    cluster.center = new Vector3(i, j, k);
                    cluster.size = mesh.bounds.size;
                    cluster.min = new Vector3(i, j, k) - offset/2;
                    cluster.max = new Vector3(i, j, k) + offset/2;
                    clusters_.Add(cluster);
                }
            }
        }

        //Put each vertex on his cluster
        List<Vector3> result = new();
        for(int i = 0; i < mesh.vertices.Length; ++i){
            
            foreach(Cluster cluster in clusters_){
                if(mesh.vertices[i].x >= cluster.min.x && mesh.vertices[i].y >= cluster.min.y && mesh.vertices[i].z >= cluster.min.z &&
                    mesh.vertices[i].x <= cluster.max.x && mesh.vertices[i].y <= cluster.max.y && mesh.vertices[i].z <= cluster.max.z){
                    if(cluster.vertices == null) cluster.vertices = new List<Vector3>();
                    cluster.vertices.Add(mesh.vertices[i]);
                }
            }
            
        }
        
        
        //Make the vertex mean of all clusters
        foreach(Cluster cluster in clusters_){
            if(cluster.vertices == null) continue;

            //cluster.vertexMean = cluster.vertices[0];
        }

        List<Vector3> vertices = new();
        List<int> triangles = new();
        //Initialize all vertices

        //Initialize all triangles
        

        mesh.vertices = vertices.ToArray();
        
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
