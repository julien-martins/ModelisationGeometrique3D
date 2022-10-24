using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermine : MonoBehaviour
{
    public GameObject P0;
    public GameObject P1;

    public GameObject V0;
    public GameObject V1;

    private Vector3 p0, p1, v0, v1;

    private void OnDrawGizmos() {
        p0 = P0.transform.position;
        p1 = P1.transform.position;

        v0 = V0.transform.position;
        v1 = V1.transform.position;

        List<Vector3> points = HermineCalc(p0, p1, v0, v1);

        Gizmos.color = Color.red;
        for(int i = 0; i < points.Count - 1; ++i){
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    float F1(float u) => 2*Mathf.Pow(u, 3) - 3*Mathf.Pow(u, 2) + 1;
    float F2(float u) => -2*Mathf.Pow(u, 3) + 3*Mathf.Pow(u, 2);
    float F3(float u) => Mathf.Pow(u, 3) - 2*Mathf.Pow(u, 2) + u;
    float F4(float u) => Mathf.Pow(u, 3) - Mathf.Pow(u, 2);

    List<Vector3> HermineCalc(Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1) {
        
        List<Vector3> newPoints = new();

        for(float u = 0.0f; u <= 1.0f; u += 0.01f){
            Vector3 point = F1(u) * p0 + F2(u) * p1 + F3(u) * v0 + F4(u) * v1;
            newPoints.Add(point);
        }

        return newPoints;
    }
}
