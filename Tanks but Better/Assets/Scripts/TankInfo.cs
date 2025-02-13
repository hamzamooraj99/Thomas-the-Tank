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

    private int currArmour;

    private int currBattery;
    private bool isPlayabale;

    private WeaponData weapon;

    #region Getters
    public int GetArmour()
    {
        return currArmour;
    }
    public int GetBattery()
    {
        return currBattery;
    }
    public WeaponData GetWeapon()
    {
        return weapon;
    }
    #endregion

    #region Setters
    public void SetArmour(int armour)
    {
        currArmour = armour;
    }
    public void SetBattery(int battery)
    {
        currBattery = battery;
    }
    #endregion

    void Awake()
    {
        tankData = Instantiate(tankData);   
    }
    void Start()
    {
        InitialiseTank();
        
    }

    void InitialiseTank()
    {
        currArmour = tankData.armour;
        currBattery = tankData.battery;
        weapon = tankData.weapon;
        isPlayabale = tankData.playable;

        string play = isPlayabale ? "playable" : "not playable";
        Debug.Log($"Tank {tankData.name} initialised with {currArmour} armour, {currBattery} battery and a {weapon.name}. The character is {play}");
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
