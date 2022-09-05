using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeArray))]
public class NodeArrayEditor : Editor
{
    NodeArray obj;

    private void OnEnable()
    {
        obj = (NodeArray)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            obj.GenerateNodes();
        }
    }
}

[CustomEditor(typeof(ObstacleAvoidance))]
public class ObstacleAvoidanceEditor : Editor
{
    ObstacleAvoidance obj;

    private void OnEnable()
    {
        obj = (ObstacleAvoidance)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
    }

    private void SteeringFunctionField()
    {

    }
}

[CustomEditor(typeof(UnitController))]
public class UnitControllerEditor : Editor
{
    UnitController obj;

    private void OnEnable()
    {
        obj = (UnitController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (obj.FSM is null)
            return;

        string str = $"CURRENT STATE:\n";

        if (obj.FSM.CurrentState is null)
        {
            str += "NULL";
        }
        else
        {
            str += obj.FSM.CurrentStateType.Name;
        }
        GUILayout.Label(str);
    }
}