using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween
{
    //Getters for the public variables, set using the constructor.
    public Transform Target { get; }
    public Vector3 StartPos { get; }
    public Vector3 EndPos { get; }
    public float StartTime { get; }
    public float Duration { get; }
    
    //Constructor for the Tween class, setting the public variables, all values are passed in as parameters.
    public Tween (  Transform target, 
        Vector3 startPos, 
        Vector3 endPos, 
        float startTime, 
        float duration )
    {
        Target = target;
        StartPos = startPos;
        EndPos = endPos;
        StartTime = startTime;
        Duration = duration;
    }
}