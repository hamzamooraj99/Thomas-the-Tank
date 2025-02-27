using UnityEngine;

[CreateAssetMenu(fileName = "TankData", menuName = "Character/Tank")]

public class TankData : ScriptableObject
{
    [Header("Tank Info")]
    public new string name;
    public bool playable;
    public bool friendly;

    [Header("Tank Stats")]
    public int battery;
    public WeaponData weapon;
}
