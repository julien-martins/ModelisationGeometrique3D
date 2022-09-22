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
        // Creation d'un composant MeshFilter qui peut ensuite �tre visualis�
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh msh = createPlane();

        // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshFilter>().mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    Mesh createPlane()
    {
        Mesh msh = new Mesh();

        int sizeOfVertices = (sizeRectangleX + 1) * (sizeRectangleY + 1);

        // Cr�ation des structures de donn�es qui accueilleront sommets et  triangles
        Vector3[] vertices = new Vector3[sizeOfVertices];
        int[] triangles = new int[sizeRectangleX * sizeRectangleY * 6];

        //Creer touts les vertices du plan
        for (int i = 0; i < sizeOfVertices; ++i)
        {
            int x = i % (sizeRectangleX + 1);
            int y = i / (sizeRectangleX + 1);

            vertices[i] = new Vector3(x, y, 0);
        }

        //Creer tout les triangles du plan en associant les vertices
        for (int j = 0; j < sizeRectangleX * sizeRectangleY * 6; j += 6)
        {
            int offsetY = (int)(j / 6) / sizeRectangleX;

            triangles[j] = (int)(j / 6) + offsetY;
            triangles[j + 1] = (int)(j / 6) + 1 + offsetY;
            triangles[j + 2] = sizeRectangleX + 1 + (int)(j / 6) + offsetY;

            triangles[j + 3] = (int)(j / 6) + 1 + offsetY;
            triangles[j + 4] = sizeRectangleX + 2 + (int)(j / 6) + offsetY;
            triangles[j + 5] = sizeRectangleX + 1 + (int)(j / 6) + offsetY;
        }

        //Creer le mesh
        msh.vertices = vertices;
        msh.triangles = triangles;

        return msh;
    }
    Mesh createCube()
    {
        Mesh msh = new Mesh();


        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[8 * 6];

        //Creation de tout les vertices du cubes
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

        msh.vertices = vertices;
        msh.triangles = triangles;

        return msh;
    }

}