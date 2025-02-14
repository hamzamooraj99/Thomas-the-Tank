using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class TankInfo : MonoBehaviour
{
    [SerializeField] TankData tankData;

    // [Header("UI References")]
    // [SerializeField] Slider batteryBar;
    // [SerializeField] TMP_Text batteryAmount;
    // [SerializeField] Slider armourBar;
    // [SerializeField] TMP_Text armourAmount;

    private string tankName;
    private int currArmour;
    private int currBattery;
    private bool isPlayable;
    private bool isFriendly;

    private WeaponData weapon;

    private EnemyAI enemyAI;

    #region Getters
    public string GetName() => tankName;
    public int GetArmour() => currArmour;
    public int GetBattery() => currBattery;
    public WeaponData GetWeapon() => weapon;
    public bool GetPlayableFlag() => isPlayable;
    public bool GetFriendlyFlag() => isFriendly;
    public EnemyAI GetEnemyAI() => enemyAI;
    #endregion

    #region Setters
    public void SetArmour(int armour) => currArmour = armour;
    public void SetBattery(int battery) => currBattery = battery;
    #endregion

    void Awake()
    {
        InitialiseTank();
    }

    void InitialiseTank()
    {
        tankData = Instantiate(tankData);
        tankName = tankData.name;
        currArmour = tankData.armour;
        currBattery = tankData.battery;
        weapon = tankData.weapon;
        isPlayable = tankData.playable;
        isFriendly = tankData.friendly;

        if(!isPlayable && !isFriendly){
            enemyAI = GetComponent<EnemyAI>();
        }

        // string play = isPlayabale ? "playable" : "not playable";
        // Debug.Log($"Tank {tankData.name} initialised with {currArmour} armour, {currBattery} battery and a {weapon.name}. The character is {play}");
    }

    public void TakeDamage(int damage)
    {
        currArmour -= damage;
        if(currArmour <= 0){
            Destroy(gameObject);
            Debug.Log($"{tankData.name} destroyed");
        }
    }

    public void UpdateHealth()
    {
        
    }
}
