using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(fileName = "New OA Preset")]
public class ObstacleAvoidance : ScriptableObject
{
    [SerializeField]
    List<SteeringFunctionCall> steeringFunctions;

    
}

[Serializable]
public struct SteeringFunctionCall
{
    public SteeringFunction func;
    public Vector3 currentPos;
    public Vector3 currentDir;
    public Vector3 targetPos;
    public Vector3 targetDir;
    public Vector3 targetVel;
    
    
    public float timePredict;
    public IEnumerable<Vector3> avgTargets;
    public float targetDistance;
    public float radius;
    public float avoidWeight;
}

public struct SteeringFunctionParams
{

}

public enum SteeringFunction
{
    Seek,
    Flee,
    Avoid,
    Pursuit,
    Average,
    Separate,
    AvoidObstacle,
}