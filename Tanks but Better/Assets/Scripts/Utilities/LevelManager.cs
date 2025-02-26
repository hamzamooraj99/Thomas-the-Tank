using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Texture2D cursorTexture;

    public Transform[] spawnPoints;

    [SerializeField] public AudioClip explosionSound;

    private GameObject player;

    void Awake()
    {
        if(LevelManager.instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void GameOver()
    {
        SoundFXManager.instance.PlaySoundFXClip(explosionSound, transform, 0.7f);
        UIManager _ui = GetComponent<UIManager>();
        if(_ui != null) _ui.ToggleDeathPanel();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
