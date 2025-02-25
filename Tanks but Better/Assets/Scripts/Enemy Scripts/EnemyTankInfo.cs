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
    private int maxBattery;
    private bool isPlayable;

    [Header("Enemy UI References")]
    [SerializeField] Slider healthBar;
    [SerializeField] Camera cameraPosition;
    [SerializeField] Transform enemy;
    [SerializeField] Vector3 offset;

    [HideInInspector] public WeaponData weapon;
    [HideInInspector] public int maxAmmo;

    public delegate void DamageTakenHandler();
    public event DamageTakenHandler onDamageTaken;

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

        if (healthBar == null)
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"{gameObject.name}: No Canvas found!");
            }
            else
            {
                healthBar = canvas.GetComponentInChildren<Slider>();
                if (healthBar == null)
                {
                    Debug.LogError($"{gameObject.name}: Canvas found, but no Slider found!");
                }
            }
        }

        if (healthBar == null)
            Debug.LogError($"{gameObject.name}: No Health Bar in Awake");
        else
            Debug.Log($"{gameObject.name}: Health Bar in Awake");
    }

    void Start()
    {
        if(healthBar == null)
            Debug.Log("No Health Bar in Start");
        else
            Debug.Log("Health Bar in Start");
        UpdateHealthBar(currBattery);    
    }

    void Update()
    {        
        healthBar.transform.rotation = cameraPosition.transform.rotation;
        healthBar.transform.position = enemy.transform.position + offset;
    }

    void InitialiseTank()
    {
        tankData = Instantiate(tankData);
        tankName = tankData.name;
        currBattery = tankData.battery;
        maxBattery = tankData.battery;
        weapon = Instantiate(tankData.weapon);
        maxAmmo = weapon.totalAmmo;
        isPlayable = tankData.playable;
        // Debug.Log($"{currBattery} battery left");
    }

    public void TakeDamage(int damage)
    {
        currBattery -= damage;
        UpdateHealthBar(currBattery);
        // Debug.Log($"Tank taken {damage} damage. Current Battery = {currBattery}");
        if(currBattery <= 0){
            Destroy(gameObject);
            Debug.Log($"{tankData.name} destroyed");
        }
        onDamageTaken?.Invoke();
    }

    public void RestoreBattery(int restoration)
    {
        currBattery = Mathf.Min(currBattery + restoration, 100);
    }

    private void UpdateHealthBar(int currValue)
    {
        healthBar.value = (float) currValue / maxBattery;
    }
}
