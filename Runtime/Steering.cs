using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Steering
{
    
    
    //public static Vector3 CallDynamic(
    //    in SteeringFunctionCall call)
    //{
    //    return call.func switch
    //    {
    //        SteeringFunction.Seek => Seek(call.currentPos, call.targetPos),
    //        SteeringFunction.Flee => Flee(call.currentPos, call.targetPos),
    //        SteeringFunction.Avoid => Avoid(
    //                call.currentPos, call.targetPos,
    //                call.targetVel, call.timePredict),
    //        SteeringFunction.Pursuit => Pursuit(
    //                call.currentPos, call.targetPos,
    //                call.targetVel, call.timePredict),
    //        SteeringFunction.Average => Average(call.avgTargets),
    //        SteeringFunction.Separate => Separate(
    //                call.currentPos,
    //                call.targetPos,
    //                call.targetDistance),
    //        SteeringFunction.AvoidObstacle => AvoidObstacle(
    //                call.currentDir, call.currentPos,
    //                call.radius, call.avoidWeight),
    //        _ => Vector3.zero,
    //    };
    //}
    
    /// <summary>
    /// Returns a Direction from the position facing the target
    /// </summary>
    /// <param name="from"> Origin </param>
    /// <param name="target"> Target Position </param>
    /// <returns></returns>
    public static Vector3 Seek(Vector3 from, Vector3 target) => (target - from).normalized;
    /// <summary>
    /// Returns a Direction from the position away from the target
    /// </summary>
    /// <param name="from"> Origin </param>
    /// <param name="target"> Target Position </param>
    /// <returns></returns>
    public static Vector3 Flee(Vector3 from, Vector3 target) => (from - target).normalized;
    /// <summary>
    /// Returns a Direction from the position facing away from the future position of the target
    /// </summary>
    /// <param name="from"> Origin </param>
    /// <param name="target"> Target Position </param>
    /// <param name="targetVel"> Target's Current Velocity </param>
    /// <param name="timePredict"> Time at which to avoid the target </param>
    /// <returns></returns>
    public static Vector3 Avoid(Vector3 from, Vector3 target, Vector3 targetVel, float timePredict) {
        Vector3 posPredict = target + targetVel * timePredict;
        return (from - posPredict).normalized;
    }
    /// <summary>
    /// Returns a Direction from the position facing the future position of the target
    /// </summary>
    /// <param name="from"> Origin </param>
    /// <param name="target"> Target Position </param>
    /// <param name="targetVel"> Target's Current Velocity </param>
    /// <param name="timePredict"> Time at which to impact the target </param>
    /// <returns></returns>
    public static Vector3 Pursuit(Vector3 from, Vector3 target, Vector3 targetVel, float timePredict) {
        Vector3 posPredict = target + targetVel * timePredict;
        return (posPredict - from).normalized;
    }
    /// <summary>
    /// Get the Average direction from a Collection of directions
    /// </summary>
    /// <param name="values"> Directions </param>
    /// <returns></returns>
    public static Vector3 Average(in IEnumerable<Vector3> values)
    {
        Vector3 avg = Vector3.zero;
        int count = 0;
        foreach (var val in values) {
            avg += val;
            count++;
        }
        return avg / count;
    }
    /// <summary>
    /// Get a Direction facing towards or away from the target, depending on the distance factor
    /// </summary>
    /// <param name="from"></param>
    /// <param name="target"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector3 Separate(Vector3 from, Vector3 target, float distance)
    {
        var dir = (from - target);
        return dir.normalized * ((distance - dir.magnitude)/distance);
    }

    public static Vector3 AvoidObstacle(Vector3 currentDir, Vector3 currentPos, float radius, float avoidWeight)
    {
        Vector3 dir = currentDir;
        var obstacles = Physics.OverlapSphere(currentPos, radius)
            .Where(c => c.gameObject.CompareTag("Obstacle"))
            .ToArray();

        //Debug.DrawLine(currentPos, currentPos + dir.normalized * 5, Color.yellow);

        int count = obstacles.Length;
        if (count > 0)
        {
            float curDistance = 0;
            int curIndex = 0;
            curDistance = Vector3.Distance(obstacles[0].transform.position, currentPos);
            for (int i = 0; i < count; i++)
            {
                var newDist = Vector3.Distance(obstacles[i].transform.position, currentPos);

                Debug.DrawLine(currentPos, obstacles[i].transform.position, Color.green);

                if (newDist < curDistance)
                {
                    curDistance = newDist;
                    curIndex = i;
                }


            }
            //Debug.DrawLine(currentPos, obstacles[curIndex].transform.position, Color.black);
            var dirToObj = (currentPos - obstacles[curIndex].transform.position).normalized
                * ((radius - curDistance) / radius) * avoidWeight;
            //Debug.DrawLine(currentPos, currentPos + dirToObj.normalized * 5, Color.blue);
            dir += dirToObj;
        }


        return dir;
    }
}

/*
 * Leader: use Seek
 * Align: use Average Function with Direction of all Entities
 * Cohesion: use Average Function with Postion of all Entities. Then Seek the Position
 * Separation: Create Weighted Avoid Function
 *      > dir
 */
