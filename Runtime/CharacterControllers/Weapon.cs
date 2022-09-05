using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    [SerializeField]
    float damage = 10;

    [SerializeField]
    GameObject weaponPrefab;

    [SerializeField]
    bool twoHanded;


    public float Damage => damage;
    public GameObject WeaponPrefab => weaponPrefab;
    public bool TwoHanded => twoHanded;
}