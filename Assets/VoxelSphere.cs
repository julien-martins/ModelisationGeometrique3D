using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSphere : MonoBehaviour
{

    public MeshGenerator meshGenerator;

    CombineInstance[] combine;

    [Header("Parameters")]
    public Vector3 Center = Vector3.zero;
    public int Radius = 4;

    public int CubeSize = 1;

    public Material mat;

    private List<Cube> _meshes = new List<Cube>();

    // Start is called before the first frame update
    void Start()
    {
        DrawVoxelSphere();
    }

    void DrawVoxelSphere(){
        int width = 2 * Radius / CubeSize;
        int height = 2 * Radius / CubeSize;
        int depth = 2 * Radius / CubeSize;

        Cube mesh = new Cube();

        for(int z = 1; z < depth; ++z){
            for(int y = 1; y < height; ++y){
                for(int x = 1; x < width; ++x){
                    Vector3 cubePos = new Vector3(
                            Center.x + x * CubeSize - width/2 - CubeSize/2, 
                            Center.y + y * CubeSize - height/2 - CubeSize/2, 
                            Center.z + z * CubeSize - depth/2 - CubeSize/2);

                    if(PointInCircle(cubePos, Radius, Center)) continue;
                    
                    mesh.mesh = meshGenerator.CreateCube(cubePos, CubeSize);
                    mesh.pos = cubePos;

                    _meshes.Add(mesh);
                }
            }
        }

        //Draw Each Cube
        foreach(Cube c in _meshes){
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();

            go.GetComponent<MeshFilter>().mesh = c.mesh;
            go.GetComponent<MeshRenderer>().material = mat;
            go.transform.parent = this.transform;
        }

    }

    public List<Cube> GetCubes() => _meshes;

    public void UpdateMesh(List<Cube> cubes){
        _meshes = cubes;

        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        DrawVoxelSphere();
    }

    bool PointInCircle(Vector3 point, float radius, Vector3 center){
        float value =  
            Mathf.Pow(point.x - center.x, 2) + 
            Mathf.Pow(point.y - center.y, 2) + 
            Mathf.Pow(point.z - center.z, 2) - Mathf.Pow(radius, 2);

        return value > 0;
    }

    private void OnDrawGizmos() {
        //gameObject.GetComponent<MeshFilter>().mesh = CreateVoxelSphere();
    }
}
