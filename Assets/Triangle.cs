using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public Material mat;

    public int nbOfTriangles;

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();          // Creation d'un composant MeshFilter qui peut ensuite �tre visualis�
        gameObject.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[nbOfTriangles*3];            // Cr�ation des structures de donn�es qui accueilleront sommets et  triangles
        int[] triangles = new int[nbOfTriangles*3];

        Vector3 indexSave = new Vector3(0, 0, 0);

        for (int i = 0; i < nbOfTriangles*3; i += 3)
        {
            vertices[i] = indexSave;            // Remplissage de la structure sommet 
            vertices[i + 1] = new Vector3(i*1, 0, 0);
            vertices[i + 2] = new Vector3(i + 0, 1, 0);

            indexSave = vertices[i + 1];

            triangles[i] = i;                               // Remplissage de la structure triangle. Les sommets sont repr�sent�s par leurs indices
            triangles[i + 1] = i + 1;                       // les triangles sont repr�sent�s par trois indices (et sont mis bout � bout)
            triangles[i + 2] = i + 2;
        }

        Mesh msh = new Mesh();                          // Cr�ation et remplissage du Mesh

        msh.vertices = vertices;
        msh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }
}
/*

*/