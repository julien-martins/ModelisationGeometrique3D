using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GommeController : MonoBehaviour
{
    private List<VoxelSphere> voxelObjects;

    private VoxelSphere _gomme;

    void Update()
    {
        foreach (var cube in _gomme.GetCubes())
        {
            foreach(var voxelObject in voxelObjects)
            {
                foreach (var otherCube in voxelObject.GetCubes())
                {
                    if (cube.pos.Equals(otherCube.pos))
                    {
                    }
                }
            }
        }
    }    
}
