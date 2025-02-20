using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Awake()
    {
        if(slider == null)
            Debug.LogError("Slider not assigned");
    }
    public void UpdateHealthBar(int currValue, int maxValue)
    {
        if(slider != null)
            slider.value = currValue / maxValue;
        else
            Debug.LogError("Slider is null");
    }
}
