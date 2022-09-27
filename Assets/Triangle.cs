using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum EFormType
{
    Plane,
    Cube,
    Cylindre,
    Sphere
}

public class Triangle : MonoBehaviour
{
    public EFormType FormType;
    
    public Material mat;

    [Header("Rectangle")]
    public int sizeRectangleX;
    public int sizeRectangleY;

    [Header("Cylindre")]
    public int CylindreRayon;
    public int Hauteur;
    public int NbMeridien;

    [Header("Sphere")]
    public int SphereRayon;

    // Use this for initialization
    void Start()
    {
        // Creation d'un composant MeshFilter qui peut ensuite être visualisé
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh msh = null;
        switch (FormType) {
            case EFormType.Plane:
                msh = createPlane();
                break;
            case EFormType.Cube:
                msh = createCube();
                break;
            case EFormType.Cylindre:
                msh = createCylindre();
                break;
            case EFormType.Sphere:
                msh = createSphere();
                break;
        }

        // Remplissage du Mesh et ajout du matériel
        gameObject.GetComponent<MeshFilter>().mesh = msh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    void OnDrawGizmos()
    {
        Mesh mesh = createSphere();
        
        foreach (Vector3 coord in mesh.vertices)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(coord, 0.1f);
        }
    }

    Mesh createSphere()
    {
        Mesh msh = new Mesh();

        int nbParraleles = 8;
        int nbMeridiens = 8;
        int rayon = 1;

        Vector3[] vertices = new Vector3[nbParraleles * nbMeridiens];
        int[] triangles = new int[nbParraleles * nbMeridiens * 6];
        
        
        for (int j = 1; j < nbParraleles; ++j)
        {
            for (int i = 0; i < nbMeridiens; ++i)
            {
                double phi = Math.PI * j / nbParraleles;
                double theta = 2 * Math.PI * i / nbMeridiens;

                vertices[i * nbMeridiens + j] = new Vector3(
                    (float)(rayon * Math.Cos(theta) * Math.Sin(phi)), 
                    (float)(rayon * Math.Sin(theta) * Math.Sin(phi)), 
                    (float)(rayon * Math.Cos(phi))
                );

            }
        }


        //vertices[nbMeridiens * nbParraleles-1] = new Vector3(0, 0, rayon);
        //vertices[0] = new Vector3(0, 0, -rayon);
        vertices[0] = new Vector3(1, 1, 1);
        vertices[nbMeridiens * nbParraleles - 1] = new Vector3(1, 1, 1);

        msh.vertices = vertices;
        msh.triangles = triangles;

        return msh;
    }

    Mesh createCylindre()
    {
        Mesh msh = new Mesh();

        int nbVertices = 8;

        Vector3[] vertices = new Vector3[nbVertices*2 + 2];
        int[] triangles = new int[nbVertices * 6 + nbVertices * 2 * 3];

        for(int i = 0; i < nbVertices; ++i)
        {
            double angle = 2 * Math.PI * i / nbVertices;
            vertices[i] = new Vector3((float)(CylindreRayon * Math.Cos(angle)), 
                                        (float)(CylindreRayon * Math.Sin(angle)), 0.0f);
        }

        for(int i = nbVertices; i < 2*nbVertices; ++i)
        {
            double angle = 2 * Math.PI * i / nbVertices;
            vertices[i] = new Vector3((float)(CylindreRayon * Math.Cos(angle)), (float)(CylindreRayon * Math.Sin(angle)), (float)Hauteur);
        }

        vertices[nbVertices * 2] = new Vector3(0, 0, 0);
        vertices[nbVertices * 2 + 1] = new Vector3(0, 0, Hauteur);

        //Face du cylindre
        for (int i = 0; i < nbVertices * 6; i += 6)
        {
            int index = (int)(i / 6);

            if (index == nbVertices - 1)
            {
                triangles[i] = index;
                triangles[i + 1] = nbVertices;
                triangles[i + 2] = index + nbVertices;

                triangles[i + 3] = index;
                triangles[i + 4] = 0;
                triangles[i + 5] = nbVertices;
            }
            else
            {
                triangles[i] = index;
                triangles[i + 1] = index + nbVertices + 1;
                triangles[i + 2] = index + nbVertices;

                triangles[i + 3] = index;
                triangles[i + 4] = index + 1;
                triangles[i + 5] = index + nbVertices + 1;
            }
            
        }

        //Chapeaux
        for (int i = nbVertices * 6; i < nbVertices * 6 + nbVertices * 3; i += 3)
        {
            int index = i/3 - nbVertices * 2;
            //Debug.Log(index);

            //Haut
            if (index == nbVertices-1)
            {
                triangles[i] = index;
                triangles[i + 1] = nbVertices*2;
                triangles[i + 2] = 0;
            }
            else
            {
                triangles[i] = index;
                triangles[i + 1] = nbVertices * 2;
                triangles[i + 2] = index + 1;
            }

            //Bas
            index += nbVertices;
            //Debug.Log(index);
            if (index == 2 * nbVertices - 1)
            {
                triangles[i + nbVertices * 3] = nbVertices;
                triangles[i + nbVertices * 3 + 1] = nbVertices * 2 + 1;
                triangles[i + nbVertices * 3 + 2] = index;
            }
            else
            {
                triangles[i + nbVertices*3] = index + 1;
                triangles[i + nbVertices*3 + 1] = nbVertices * 2 + 1;
                triangles[i + nbVertices*3 + 2] = index;
            }
        }

        msh.vertices = vertices;
        msh.triangles = triangles;

        return msh;
    }

    Mesh createPlane()
    {
        Mesh msh = new Mesh();

        int sizeOfVertices = (sizeRectangleX + 1) * (sizeRectangleY + 1);

        // Création des structures de données qui accueilleront sommets et  triangles
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