using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EFormType
{
    Plane,
    Cube,
    Cylindre,
    Sphere,
    VoxelSphere,
    Custom
}

public class MeshGenerator : MonoBehaviour
{
    public EFormType FormType;
    
    public Material mat;

    [Header("Debugger")] 
    public bool DrawVertices = false;
    public float SphereRadius = 0.01f;

    [Header("Rectangle")]
    public int sizeRectangleX;
    public int sizeRectangleY;

    [Header("Cylindre")]
    public int CylindreRayon;
    public int Hauteur;
    public int NbMeridien;

    [Header("Sphere")]
    public Vector3 center;
    public int SphereRayon;

    public int SphereMeridien;
    public int SphereParraleles;

    [Header("Custom")]
    public string fileName;

    public Mesh Mesh {get; private set;}

    // Use this for initialization
    void Start()
    {
        /*
        // Creation d'un composant MeshFilter qui peut ensuite �tre visualis�
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh = GetMesh();

        // Remplissage du Mesh et ajout du mat�riel
        gameObject.GetComponent<MeshFilter>().mesh = Mesh;
        gameObject.GetComponent<MeshRenderer>().material = mat;
        */
    }

    void OnDrawGizmos()
    {
        /*
        if (!DrawVertices) return;

        Mesh mesh = GetMesh();

        foreach (Vector3 coord in mesh.vertices)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(coord, SphereRadius);
        }
        */
    }

    public Mesh GetMesh()
    {
        Mesh msh = null;
        switch (FormType)
        {
            case EFormType.Plane:
                msh = CreatePlane();
                break;
            case EFormType.Cube:
                msh = CreateCube(Vector3.zero);
                break;
            case EFormType.Cylindre:
                msh = CreateCylindre();
                break;
            case EFormType.Sphere:
                msh = CreateSphere();
                break;
            case EFormType.Custom:
                msh = LoadMeshOFF(fileName);
                break;
        }

        return msh;
    }

    Mesh LoadMeshOFF(string fileName)
    {
        String[] lines = readFiles(fileName + ".off");
        if (lines[0] != "OFF") return null;

        Mesh mesh = new();
        int sizeVertices = int.Parse(lines[1].Split(" ")[0]);
        int sizeTriangles = int.Parse(lines[1].Split(" ")[1]);

        Vector3[] vertices = new Vector3[sizeVertices];
        int[] triangles = new int[sizeTriangles * 3];

        int headerOffset = 2;

        //Vertices
        for(int i = headerOffset; i < sizeVertices + headerOffset; ++i)
        {
            String[] coordStr = lines[i].Replace(".", ",").Split(" ");
            double x = double.Parse(coordStr[0]);
            double y = double.Parse(coordStr[1]);
            double z = double.Parse(coordStr[2]);

            vertices[i- headerOffset] = new Vector3((float)x, (float)y, (float)z);
        }

        //Triangles
        for(int i = sizeVertices + headerOffset; i < sizeVertices + sizeTriangles - headerOffset; ++i)
        {
            String[] coordStr = lines[i].Split(" ");
            int s1 = int.Parse(coordStr[1]);
            int s2 = int.Parse(coordStr[2]);
            int s3 = int.Parse(coordStr[3]);

            int index = (i - sizeVertices - headerOffset) * 3;

            triangles[index] = s1;
            triangles[index + 1] = s2;
            triangles[index + 2] = s3;
        }

        NormalizeModel(vertices);

        ReplaceModel(vertices);

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

    void SaveMeshOFF()
    {
        

    }

    Vector3 MaxCoord(Vector3[] vertices)
    {
        Vector3 max = vertices[0];
        for(int i = 1; i < vertices.Length; ++i)
        {
            Vector3 coord = new(Math.Abs(vertices[i].x), Math.Abs(vertices[i].y), Math.Abs(vertices[i].z));
            if (coord.x > max.x || coord.y > max.y || coord.z > max.z)
                max = coord;
        }

        return max;
    }

    Vector3 VerticesCenter(Vector3[] vertices)
    {
        Vector3 center = new();

        for(int i = 0; i < vertices.Length; ++i)
        {
            center += vertices[i];
        }

        return center/vertices.Length;
    }

    void ReplaceModel(Vector3[] vertices)
    {
        Vector3 center = VerticesCenter(vertices);

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] -= center;
        }
    }

    void NormalizeModel(Vector3[] vertices)
    {
        Vector3 max = MaxCoord(vertices);

        for(int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = new Vector3(
                vertices[i].x / max.x,
                vertices[i].y / max.y,
                vertices[i].z / max.z
                );
        }
    }

    public String[] readFiles(String fileName)
    {
        var sr = new StreamReader(Application.dataPath + "/" + fileName);
        Debug.Log(Application.dataPath + "/" + fileName);
        var fileContents = sr.ReadToEnd();
        sr.Close();

        return fileContents.Split("\n"[0]);
    }

    Mesh CreateSphere()
    {
        Mesh msh = new Mesh();

        int nbParraleles = SphereParraleles;
        int nbMeridiens = SphereMeridien;
        int rayon = SphereRayon;

        Vector3[] vertices = new Vector3[(nbParraleles-1) * nbMeridiens + 2];
        int[] triangles = new int[((nbParraleles - 1) * nbMeridiens + nbMeridiens) * 6];
        
        for (int i = 0; i < nbMeridiens; ++i)
        {
            for (int j = 1; j < nbParraleles; ++j)
            {
                double phi = Math.PI * j / nbParraleles;
                double theta = 2 * Math.PI * i / nbMeridiens;
                
                vertices[i * (nbMeridiens-1) + (j-1)] = new Vector3(
                    (float)(rayon * Math.Cos(theta) * Math.Sin(phi)), 
                    (float)(rayon * Math.Sin(theta) * Math.Sin(phi)), 
                    (float)(rayon * Math.Cos(phi))
                );
            }
        }

        vertices[(nbParraleles - 1) * nbMeridiens] = new Vector3(0, 0, -rayon);
        vertices[(nbParraleles - 1) * nbMeridiens + 1] = new Vector3(0, 0, rayon);


        for (int i = 0; i < (nbParraleles - 1) * nbMeridiens * 6; i += 6)
        {
            int index = i / 6;
            if ((index+1) % (nbParraleles-1) == 0) continue;
            if (index < (nbParraleles - 1) * nbMeridiens - (nbMeridiens - 1))
            {
                triangles[i] = index;
                triangles[i + 1] = index + 1;
                triangles[i + 2] = index + (nbMeridiens-1);

                triangles[i + 3] = index + 1;
                triangles[i + 4] = index + nbMeridiens;
                triangles[i + 5] = index + (nbMeridiens-1);
            }
            else
            {
                triangles[i] = index;
                triangles[i + 1] = index + 1;
                triangles[i + 2] = (index + (nbMeridiens - 1)) % nbMeridiens;

                triangles[i + 3] = index + 1;
                triangles[i + 4] = (index + nbMeridiens) % nbMeridiens;
                triangles[i + 5] = (index + (nbMeridiens - 1)) % nbMeridiens;
            }
        }

        //Chapeaux Haut
        int offset = (nbParraleles - 1) * nbMeridiens * 6;
        int cpt = 0;
        for (int i = 0; i < (nbParraleles - 1) * nbMeridiens; i++)
        {
            if (i % (nbMeridiens - 1) == 0)
            {
                if (i < (nbParraleles - 1) * nbMeridiens - (nbMeridiens - 1))
                {
                    triangles[offset + cpt] = i;
                    triangles[offset + cpt + 1] = i + nbMeridiens - 1;
                    triangles[offset + cpt + 2] = (nbParraleles - 1) * nbMeridiens + 1;
                }
                else
                {
                    triangles[offset + cpt] = i;
                    triangles[offset + cpt + 1] = 0;
                    triangles[offset + cpt + 2] = (nbParraleles - 1) * nbMeridiens + 1;
                }

                cpt+=3;
            }
            else if (i%(nbMeridiens - 1) == nbMeridiens-2)
            {
                if (i < (nbParraleles - 1) * nbMeridiens - 1)
                {
                    triangles[offset + cpt] = i+nbMeridiens-1;
                    triangles[offset + cpt + 1] = i;
                    triangles[offset + cpt + 2] = (nbParraleles - 1) * nbMeridiens;
                }
                else
                {
                    triangles[offset + cpt] = nbMeridiens-2;
                    triangles[offset + cpt + 1] = i;
                    triangles[offset + cpt + 2] = (nbParraleles - 1) * nbMeridiens;
                }

                cpt += 3;
            }
        }


        msh.vertices = vertices.ToArray();
        msh.triangles = triangles;

        return msh;
    }

    Mesh CreateCylindre()
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

    Mesh CreatePlane()
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
    
    public Mesh CreateCube(Vector3 pos, int size = 1)
    {
        Mesh msh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[8 * 6];

        //Creation de tout les vertices du cubes
        vertices[0] = new Vector3(0 + pos.x,            0 + pos.y,          0 + pos.z);
        vertices[1] = new Vector3(1 * size + pos.x,     0 + pos.y,          0 + pos.z);
        vertices[2] = new Vector3(1 * size + pos.x,     1 * size + pos.y,   0 + pos.z);
        vertices[3] = new Vector3(0 + pos.x,            1 * size + pos.y,   0 + pos.z);
        vertices[4] = new Vector3(0 + pos.x,            0 + pos.y,          -1 * size + pos.z);
        vertices[5] = new Vector3(1 * size + pos.x,     0 + pos.y,          -1 * size + pos.z);
        vertices[6] = new Vector3(1 * size + pos.x,     1 + pos.y,          -1 * size + pos.z);
        vertices[7] = new Vector3(0 + pos.x,            1 * size + pos.y,   -1 * size + pos.z);

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