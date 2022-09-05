using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBuilder : MonoBehaviour
{
    [Header("Game")]
    [SerializeField]
    UnitModel unitPrefab;
    //[SerializeField]
    //Vector3 startRot;
    [SerializeField]
    Transform teamTransform;
    [SerializeField]
    UnitModel captain;
    [SerializeField]
    Vector3 spacing = new Vector3(1.0f, 0.0f, 1.5f);
    [SerializeField]
    List<(UnitModel, TeamBuilderItem)> units = new List<(UnitModel, TeamBuilderItem)>();

    [Header("GUI")]
    [SerializeField]
    Transform layoutTransform;
    [SerializeField]
    TeamBuilderItem teamItemPrefab;

    static readonly Vector3[] offsets = new Vector3[]
    {
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(-2, 0, 1),
        new Vector3(2, 0, 1),
        new Vector3(-2, 0, -1),
        new Vector3(2, 0, -1),
    };

    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 pos = GetPos(i + 1);
            UnitModel unit = Instantiate(unitPrefab, pos, captain.transform.rotation, teamTransform);
            var item = GetNew(unit);
            units.Add((unit, item));
        }
    }

    TeamBuilderItem GetNew(UnitModel unit)
    {
        var item = Instantiate(teamItemPrefab, layoutTransform);
        item.model = unit;
        return item;
    }

    Vector3 GetPos(int num)
    {
        if (num <= 0 || num > 8)
        {
            throw new System.Exception("Invalid Arg in TeamBuilder->GetPos()");
        }
        var pos = captain.transform.position;

        pos += offsets[num - 1].MultBy(spacing);

        return pos;

    }
}

#region OLD_CODE
/*
//Vector3 Mult(Vector3 a, Vector3 b) => new Vector3
//{
//    x = a.x * b.x,
//    y = a.y * b.y,
//    z = a.z * b.z,
//};
//public void RemoveUnit(int index)
//{
//    var toR = units[index];
//    Destroy(toR.Item1);
//    Destroy(toR.Item2);
//    units.RemoveAt(index);
//}
//public void AddUnit()
//{
//    if (units.Count >= 8)
//        return;
//    Vector3 pos = GetPos(units.Count);
//    UnitModel unit = Instantiate(unitPrefab, pos, captain.transform.rotation, teamTransform);
//    var item = GetNew(unit);
//    units.Add((unit, item));
//}
//private void Rebuild()
//{
//    bool red = false;
//    for (int i = 0; i < units.Count; i++)
//    {
//        if (red)
//        {
//        }
//        else
//        {
//            if (units[i].Item1 == null)
//            {
//                red = true;
//            }
//        }
//    }
//}
*/
#endregion