using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public List<Transform> points;

    List<Vector3> ControlPoints {get; set;}
    List<Vector3> CurvePoints {get; set;}

    [SerializeField] Bezier Child;
    [SerializeField] Bezier Parent;

    float Bernstein(int i, int n, float u)
    {
        return (Fact(n) / (Fact(i) * (Fact(n - i)))) * Mathf.Pow(u, i) * Mathf.Pow(1 - u, n - i);
    }

    int Fact(int n)
    {
        return n > 1 ? n * Fact(n - 1) : 1;
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

        return result;
    }

    void OnDrawGizmos()
    {
        ControlPoints = new();
        CurvePoints = new();

        foreach(Transform p in points){
            ControlPoints.Add(p.position);
        }
        
        if(Child != null){
            ControlPoints[ControlPoints.Count-1] = (ControlPoints[ControlPoints.Count - 2] + Child.ControlPoints[1]) / 2;
            
            points[ControlPoints.Count-1].gameObject.SetActive(false);
        }

        if(Parent != null){
            ControlPoints[0] = (Parent.ControlPoints[ControlPoints.Count - 2] + ControlPoints[1]) / 2;

            points[0].gameObject.SetActive(false);
        }

        Gizmos.color = Color.blue;
        for(int i = 0; i < ControlPoints.Count - 1; ++i){
            Gizmos.DrawLine(ControlPoints[i], ControlPoints[i+1]);
        }

        //Calculate new points
        Gizmos.color = Color.red;
        CurvePoints = BezierCurve(ControlPoints);

        for(int i = 0; i < CurvePoints.Count - 1; ++i){
            Gizmos.DrawLine(CurvePoints[i], CurvePoints[i + 1]);
        }

    }
    
}
