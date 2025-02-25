using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Texture2D cursorTexture;

    void Awake()
    {
        if(LevelManager.instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        UIManager _ui = GetComponent<UIManager>();
        if(_ui != null) _ui.ToggleDeathPanel();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
