using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public List<Transform> points;

    void polyBernstein(){

    }

    float bernstein(int i, int n, float t)
    {
        return (Factoriel(n) / (Factoriel(i) * (Factoriel(n - i)))) * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
    }

    int Factoriel(int n)
    {
        return n > 1 ? n * Factoriel(n - 1) : 1;
    }

    // Start is called before the first frame update
    void OnDrawGizmos()
    {
        List<Vector3> points_pos = new();
        foreach(Transform p in points){
            points_pos.Add(p.position);
        }
        
        Gizmos.color = Color.blue;
        for(int i = 0; i < points_pos.Count - 1; ++i){
            Gizmos.DrawLine(points_pos[i], points_pos[i+1]);
        }

        Gizmos.color = Color.red;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
