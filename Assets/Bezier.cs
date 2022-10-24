using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public List<Transform> points;

    Bezier Parent;
    Bezier Child;

    float Bernstein(int i, int n, float u)
    {
        return (Fact(n) / (Fact(i) * (Fact(n - i)))) * Mathf.Pow(u, i) * Mathf.Pow(1 - u, n - i);
    }

    int Fact(int n)
    {
        return n > 1 ? n * Fact(n - 1) : 1;
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

        //Calculate new points
        Gizmos.color = Color.red;
        List<Vector3> newPoints = BezierCurve(points_pos);

        for(int i = 0; i < newPoints.Count - 1; ++i){
            Debug.Log(newPoints[i]);
            Debug.Log(newPoints[i+1]);

            Gizmos.DrawLine(newPoints[i], newPoints[i + 1]);
        }

    }
    
    List<Vector3> BezierCurve(List<Vector3> points){
        List<Vector3> result = new();

        for(float i = 0.0f; i <= 1.0f; i += 0.01f){
            Vector3 newPoint = Vector3.zero;
            for(int j = 0; j < points.Count; j += 1){
                newPoint += Bernstein(j, points.Count - 1, i) * points[j];                
            }
            result.Add(newPoint);
        }
        Debug.Log(result.Count);

        return result;
    }
}
