using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cube{
    public Vector3 pos;
    public Mesh mesh;
}

public struct Sphere{
    public Sphere(Vector3 center, int radius){
        this.center = center;
        this.radius = radius;
        this.cubes = new List<Cube>();
    }
    public Vector3 center;
    public int radius;
    public List<Cube> cubes;
}

public class VoxelGrid : MonoBehaviour
{
    public int Width = 20;
    public int Height = 20;
    public int Depth = 20;

    public int CubeSize = 1;

    public Material MatCube;

    public MeshGenerator MeshGenerator;

    public List<Sphere> Spheres;
    private List<Cube> _cubes;

    private Vector3Int minBox;
    private Vector3Int maxBox;

    // Start is called before the first frame update
    void Start()
    {
        _cubes = new List<Cube>();
        Spheres = new List<Sphere>();

        Spheres.Add(new Sphere(new Vector3(0, 0, 0), 5));
        Spheres.Add(new Sphere(new Vector3(-5, 0, 0), 3));
        //Spheres.Add(new Sphere(new Vector3(3, 0, 0), 3));

        //InitializeGridSize();
        minBox = new Vector3Int(-Width, -Height, -Depth);
        maxBox = new Vector3Int(Width, Height, Depth);

        UpdateGrid();
        
        Intersection();
        //Union();
        
        DrawGrid(); 
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(minBox, 0.4f);
        Gizmos.DrawSphere(maxBox, 0.4f);

    }

    // Update is called once per frame
    void Update()
    {
         
    }

    void InitializeGridSize(){
        Vector3 minSphereCenter = Spheres[0].center;
        Vector3 maxSphereCenter = Spheres[0].center;

        int radiusMin = Spheres[0].radius;
        int radiusMax = Spheres[0].radius;

        for(int i = 1; i < Spheres.Count; ++i){
            minSphereCenter = Vector3.Min(Spheres[i].center, minSphereCenter);
            maxSphereCenter = Vector3.Max(Spheres[i].center, maxSphereCenter);
        }

        minBox = Vector3Int.FloorToInt(minSphereCenter - new Vector3(radiusMin + 1, radiusMin + 1, radiusMin + 1));
        maxBox = Vector3Int.FloorToInt(maxSphereCenter + new Vector3(radiusMax + 1, radiusMax + 1, radiusMax + 1));
    }
    void UpdateGrid(){
        _cubes.Clear();
        Cube mesh = new Cube();

        Vector3Int size = (maxBox - minBox);

        for(int z = minBox.z; z < maxBox.z; ++z){
            for(int y = minBox.y; y < maxBox.y; ++y){
                for(int x = minBox.x; x < maxBox.x; ++x){
                    Vector3 cubePos = new Vector3(
                            x * CubeSize - CubeSize/2, 
                            y * CubeSize - CubeSize/2, 
                            z * CubeSize - CubeSize/2);

                    foreach(Sphere sphere in Spheres){
                        if(PointInCircle(cubePos, sphere.radius, sphere.center)){
                            mesh.mesh = MeshGenerator.CreateCube(cubePos, CubeSize);
                            mesh.pos = cubePos;

                            sphere.cubes.Add(mesh);
                            //_cubes.Add(mesh);
                        }
                    }

                    
                }
            }
        }
    }

    void Intersection(){
        for(int j = 0; j < Spheres[0].cubes.Count; ++j){
            for(int i = 1; i < Spheres.Count; ++i){
                for(int k = 0; k < Spheres[i].cubes.Count; ++k){
                    if(Spheres[0].cubes[j].pos == Spheres[i].cubes[k].pos){
                        _cubes.Add(Spheres[0].cubes[j]);
                    }
                }
            }
        }
    }

    void Union(){
        for(int j = 0; j < Spheres[0].cubes.Count; ++j){
            for(int i = 1; i < Spheres.Count; ++i){
                for(int k = 0; k < Spheres[i].cubes.Count; ++k){
                    if(Spheres[0].cubes[j].pos == Spheres[i].cubes[k].pos) continue;

                    _cubes.Add(Spheres[0].cubes[j]);
                    _cubes.Add(Spheres[i].cubes[k]);
                }
            }
        }
    }

    void DrawGrid(){
        //Clear cubes
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach(Cube c in _cubes){
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();

            go.GetComponent<MeshFilter>().mesh = c.mesh;
            go.GetComponent<MeshRenderer>().material = MatCube;
            go.transform.parent = this.transform;
        }
    }


    bool PointInCircle(Vector3 point, float radius, Vector3 center){
        float value =  
            Mathf.Pow(point.x - center.x, 2) + 
            Mathf.Pow(point.y - center.y, 2) + 
            Mathf.Pow(point.z - center.z, 2) - Mathf.Pow(radius, 2);

        return value < 0;
    }
}
