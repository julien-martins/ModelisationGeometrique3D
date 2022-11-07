using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Eraser,
    Crayon,
    Static
}

public class Sphere : MonoBehaviour
{
    public int radius;
    public Mode Mode;
}
