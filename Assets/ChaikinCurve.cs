using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaikinCurve : MonoBehaviour
{
    public List<Transform> points;
    public int Iteration = 5;

    List<Vector3> newPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        List<Vector3> pointPos = new();
        foreach(Transform point in points){
            pointPos.Add(point.position);
        }

        newPoints = ChaikinCalcul(pointPos);

        for(int i = 0; i < points.Count-1; ++i){
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(points[i].position, 0.2f);
            Gizmos.DrawSphere(points[i+1].position, 0.2f);
        }

        for(int i = 0; i < newPoints.Count-1; ++i){
            Gizmos.color = Color.red;
            Gizmos.DrawLine(newPoints[i], newPoints[i+1]);
        }

    }

    List<Vector3> ChaikinCalcul(List<Vector3> points){
        List<Vector3> result = points;

        for(int i = 0; i < Iteration; ++i){
            List<Vector3> pointIterate = new();

            for(int j = 0; j < result.Count-1; ++j){
                //float dist = Vector3.Distance(points[i+1].position, points[i].position);

                //Q point
                pointIterate.Add((3*result[j]) /4 + result[j+1] / 4);

                //R point
                pointIterate.Add(result[j] / 4 + (3*result[j+1]) /4);
            }

            result = pointIterate;
        }

        return result;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
