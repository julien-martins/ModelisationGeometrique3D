using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cube{
    public Vector3 pos;
    public Mesh mesh;
}

public class VoxelGrid : MonoBehaviour
{
    public int Width = 10;
    public int Height = 10;
    public int Depth = 10;

    public int CubeSize = 1;

    Material MatCube;

    MeshGenerator MeshGenerator;

    private List<Cube> _cubes;

    private Vector3Int minBox;
    private Vector3Int maxBox;

    public List<Sphere> targetObjects;
    public Sphere Eraser;
    public Sphere Crayon;
    
    // Start is called before the first frame update
    void Start()
    {
        MeshGenerator = GetComponent<MeshGenerator>();
        MatCube = MeshGenerator.mat;
        
        _cubes = new List<Cube>();

        minBox = new Vector3Int(-Width, -Height, -Depth);
        maxBox = new Vector3Int(Width, Height, Depth);

        //Initialize Grid
        
        //Intersection();
        //Union();
        
        UpdateGrid();
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
        //UpdateGrid();
        //DrawGrid();
       
        if(Eraser != null)
            UpdateEraser();
      
        if(Crayon != null)
            UpdateCrayon();
        
        DrawGrid();
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

                    foreach(Sphere targetObject in targetObjects){
                        if(PointInCircle(cubePos, targetObject.radius, targetObject.transform.position) &&
                           !PointInCircle(cubePos, Eraser.radius, Eraser.transform.position)){
                            mesh.mesh = MeshGenerator.CreateCube(cubePos, CubeSize);
                            mesh.pos = cubePos;

                            _cubes.Add(mesh);
                        }
                    }

                    
                    
                }
            }
        }
    }

    void UpdateEraser()
    {
        for (int z = minBox.z; z < maxBox.z; ++z)
        {
            for (int y = minBox.y; y < maxBox.y; ++y)
            {
                for (int x = minBox.x; x < maxBox.x; ++x)
                {
                    Vector3 cubePos = new Vector3(
                        x * CubeSize - CubeSize/2,
                        y * CubeSize - CubeSize/2,
                        z * CubeSize - CubeSize/2
                    );

                    List<Cube> toRemove = new();
                    if (PointInCircle(cubePos, Eraser.radius, Eraser.transform.position))
                    {
                        foreach(Cube cube in _cubes){
                            if (cube.pos == cubePos)
                            {
                                toRemove.Add(cube);
                            }
                        }
                    }

                    foreach (var cube in toRemove)
                    {
                        _cubes.Remove(cube);
                    }
                }
            }
        }
    }

    void UpdateCrayon()
    {
        for (int z = minBox.z; z < maxBox.z; ++z)
        {
            for (int y = minBox.y; y < maxBox.y; ++y)
            {
                for (int x = minBox.x; x < maxBox.x; ++x)
                {
                    Vector3 cubePos = new Vector3(
                        x * CubeSize - CubeSize/2,
                        y * CubeSize - CubeSize/2,
                        z * CubeSize - CubeSize/2
                    );

                    if (PointInCircle(cubePos, Crayon.radius, Crayon.transform.position))
                    {
                        Cube cube = new Cube();
                        cube.mesh = MeshGenerator.CreateCube(cubePos, CubeSize);
                        cube.pos = cubePos;
                        _cubes.Add(cube);
                    }

                }
            }
        }
    }
    /*
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
    */
    bool AlreadyCube(Vector3 cubePos)
    {
        foreach (Cube cube in _cubes)
        {
            if (cubePos == cube.pos) return true;
        }

        return false;
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
