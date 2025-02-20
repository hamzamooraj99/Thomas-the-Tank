using System;
using System.Collections;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor.Recorder.Encoder;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTankInfo : MonoBehaviour
{
    [SerializeField] TankData tankData;

    private string tankName;
    private int currBattery;
    private bool isPlayable;

    [HideInInspector] public WeaponData weapon;
    [HideInInspector] public int maxAmmo;

    #region Getters
    public string GetName() => tankName;
    public int GetBattery() => currBattery;
    public bool GetPlayableFlag() => isPlayable;
    #endregion

    #region Setters
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
        currBattery = tankData.battery;
        weapon = Instantiate(tankData.weapon);
        maxAmmo = weapon.totalAmmo;
        isPlayable = tankData.playable;
    }

    public void TakeDamage(int damage)
    {
        currBattery -= damage;
        if(currBattery <= 0){
            Destroy(gameObject);
            Debug.Log($"{tankData.name} destroyed");
        }
    }

    public void RestoreBattery(int restoration)
    {
        currBattery = Mathf.Min(currBattery + restoration, 100);
    }
}
