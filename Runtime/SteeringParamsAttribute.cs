using System;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
sealed class SteeringParamsAttribute : Attribute
{
    
    public SteeringParamsAttribute(string positionalString)
    {
        
    }

}

public enum SteeringParams
{
    CurrentPos,
    CurrentDir,
    Target,
    TargetVel,
    TimePredict,
    AverageInput,
    TargetDistance,
    TargetD
}