using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject winPanel;

    public void ToggleDeathPanel() => deathPanel.SetActive(!deathPanel.activeSelf);
    public void ToggleWinPanel() => winPanel.SetActive(!winPanel.activeSelf);
}
