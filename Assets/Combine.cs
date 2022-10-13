using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Operation{
        Union,
        Intersection,
        Melange,
        Soustraction
    }

public class Combine : MonoBehaviour
{
    public Operation operation;    

    //public List<VoxelSphere> Spheres;

    [Header("Soustraction")]
    public VoxelSphere Sphere1;
    public VoxelSphere Sphere2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(operation){
            case Operation.Union:
                Union();
                break;
            case Operation.Intersection:
                Intersection();
                break;
            case Operation.Melange:
                Melange();
                break;
            case Operation.Soustraction:
                Soustraction(Sphere1, Sphere2);
                break;
        }
    }

    void Union(){

    }

    void Intersection(){

    }

    void Melange(){

    }

    void Soustraction(VoxelSphere s1, VoxelSphere s2){
        List<Cube> newCubes = new();

        foreach(Cube c1 in s1.GetCubes()){
            foreach(Cube c2 in s2.GetCubes()){
                if(c1.pos != c2.pos){
                    s1.GetCubes().Remove(c1);
                }
            }
        }

        gameObject.AddComponent<VoxelSphere>();
        gameObject.GetComponent<VoxelSphere>().UpdateMesh(newCubes);

        s1.UpdateMesh(newCubes);
    }
}
