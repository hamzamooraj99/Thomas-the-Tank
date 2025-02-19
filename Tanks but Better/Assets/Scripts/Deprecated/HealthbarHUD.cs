using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Slider batterySlider;
    [SerializeField] public Image fillImage;
    [SerializeField] public TMP_Text batteryLevel;

    [Header("Colour Ranges")]
    public Color fullCharge = Color.green;
    public Color halfCharge = new Color(1f, 0.65f, 0f);
    public Color lowCharge = Color.red;

    void Start()
    {
        UpdateBatteryUI(1f);
    }

    public void UpdateBatteryUI(float level)
    {
        batterySlider.value = level;
        fillImage.color = GetBatteryColor(level);
        batteryLevel.text = (Mathf.RoundToInt(level)*100).ToString();
    }

    private Color GetBatteryColor(float level)
    {
        if(level > 0.6f)
            return fullCharge;
        else if(level > 0.3f)
            return halfCharge;
        else
            return lowCharge;
    }

}
