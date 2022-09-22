using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public Material mat;

    public int sizeRectangleX;
    public int sizeRectangleY;

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();          // Creation d'un composant MeshFilter qui peut ensuite être visualisé
        gameObject.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[sizeRectangleX * sizeRectangleY];            // Création des structures de données qui accueilleront sommets et  triangles
        int[] triangles = new int[sizeRectangleX * sizeRectangleY * 6];

        for(int i = 0; i < sizeRectangleX; ++i)
        {
            for (int j = 0; j < sizeRectangleY; ++j)
            {
                vertices[i * sizeRectangleX + j] = new Vector3(i, j, 0);
            }
        }

        for (int i = 0; i < sizeRectangleX - 1; ++i)
        {
            for (int j = 0; j < sizeRectangleY - 1; ++j)
            {
                triangles[i * sizeRectangleY + j] = i * sizeRectangleY + (j + 1);
                triangles[i * sizeRectangleY + j + 1] = (i + 1) * sizeRectangleY + j;
                triangles[i * sizeRectangleY + j + 2] = i * sizeRectangleY + j;

                triangles[i * sizeRectangleY + j + 3] = i * sizeRectangleY + (j + 1);
                triangles[i * sizeRectangleY + j + 4] = (i + 1) * sizeRectangleY + (j + 1);
                triangles[i * sizeRectangleY + j + 5] = (i + 1) * sizeRectangleY + j;
            }
        }


        /*
        //Cube
        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[8*6];

        //face avant
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(1, 1, 0);
        vertices[3] = new Vector3(0, 1, 0);
        vertices[4] = new Vector3(0, 0, -1);
        vertices[5] = new Vector3(1, 0, -1);
        vertices[6] = new Vector3(1, 1, -1);
        vertices[7] = new Vector3(0, 1, -1);

        //Face avant
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        //Face Gauche
        triangles[6] = 7;
        triangles[7] = 4;
        triangles[8] = 0;

        triangles[9] = 7;
        triangles[10] = 0;
        triangles[11] = 3;

        //Face Droite
        triangles[12] = 1;
        triangles[13] = 6;
        triangles[14] = 2;

        triangles[15] = 1;
        triangles[16] = 5;
        triangles[17] = 6;

        //Face bas
        triangles[18] = 0;
        triangles[19] = 4;
        triangles[20] = 1;

        triangles[21] = 1;
        triangles[22] = 4;
        triangles[23] = 5;

        //Face haut
        triangles[24] = 6;
        triangles[25] = 7;
        triangles[26] = 2;

        triangles[27] = 7;
        triangles[28] = 3;
        triangles[29] = 2;

        //Face back
        triangles[30] = 6;
        triangles[31] = 5;
        triangles[32] = 4;

        triangles[33] = 6;
        triangles[34] = 4;
        triangles[35] = 7;
        */

        Mesh msh = new Mesh();                          // Création et remplissage du Mesh

        msh.vertices = vertices;
        msh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = msh;           // Remplissage du Mesh et ajout du matériel
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }
}
/*

*/