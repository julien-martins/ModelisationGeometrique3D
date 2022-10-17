using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simplification : MonoBehaviour
{
    public MeshGenerator meshGenerator;

    public float Subdivision = 3;
    public float cubeSize = 3.0f;

    public Material mat;

    Vector3[] Bounds;

    Mesh newMesh;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = meshGenerator.GetMesh();

        newMesh = Simplify(mesh);

        // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshFilter>().mesh = newMesh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position + newMesh.bounds.min, 0.2f);
        Gizmos.DrawSphere(transform.position + newMesh.bounds.max, 0.2f);

        Vector3 offset = Abs(Abs(newMesh.bounds.max) - Abs(newMesh.bounds.min)) / Subdivision;
        Debug.Log(offset);
        Debug.Log(newMesh.bounds.min);
        Debug.Log(newMesh.bounds.max);

        
        for(float i = newMesh.bounds.min.x; i <= newMesh.bounds.max.x; i += offset.x/2){
            for(float j = newMesh.bounds.min.y; j <= newMesh.bounds.max.y; j += offset.y/2){
                for(float k = newMesh.bounds.min.z; k <= newMesh.bounds.max.z; k += offset.z/2){
                    //Debug.Log(i);
                    Gizmos.DrawWireCube(transform.position + new Vector3(i, j, k), offset/2);
                }
            }
        }
        
    }

    Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    Mesh Simplify(Mesh mesh){
        
        return mesh;
    }

    void Update(){
    }

}
