using UnityEngine;

public class TankInfo : MonoBehaviour
{
    [SerializeField] TankData tankData;

    private int currArmour;
    private int currBattery;
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

    void Start()
    {
        InitialiseTank();
    }

    void InitialiseTank()
    {
        tankData = Instantiate(tankData);
        currArmour = tankData.armour;
        currBattery = tankData.battery;
        weapon = tankData.weapon;

        Debug.Log($"Tank {tankData.name} initialised with {currArmour} armour, {currBattery} battery and a {weapon.name}");
    }

    public void TakeDamage(int damage)
    {
        currArmour -= damage;
        if(currArmour <= 0){
            Destroy(gameObject);
            Debug.Log($"{tankData.name} destroyed");
        }
    }
}
