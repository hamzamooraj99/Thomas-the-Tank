using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Texture2D cursorTexture;

    public Transform[] spawnPoints;

    [Header("Buttons")]
    [SerializeField] GameObject respawnButton;
    [SerializeField] GameObject playAgainButton;

    [Header("Losing Sounds")]
    [SerializeField] public AudioClip explosionSound;
    [SerializeField] public AudioClip objectiveLost;
    [SerializeField] public AudioClip goodLuck;

    [Header("Winning Stuffs")]
    [SerializeField] public AudioClip something;
    [SerializeField] public AudioClip missionAccomplished;
    [SerializeField] public AudioClip goodWork;

    private GameObject player;

    void Awake()
    {
        if(LevelManager.instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(respawnButton != null) respawnButton.SetActive(false);
        if(playAgainButton != null) playAgainButton.SetActive(false);
    }

    public void GameOver()
    {
        BGMusicManager.instance.StopTrack();
        SelectNewSpawnPoint();
        StartCoroutine(PlayGameOverSequence());
        UIManager _ui = GetComponent<UIManager>();
        if(_ui != null) _ui.ToggleDeathPanel();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void GameWin()
    {
        BGMusicManager.instance.StopTrack();
        SelectNewSpawnPoint();
        player.SetActive(false);
        StartCoroutine(PlayGameWinSequence());
        UIManager _ui = GetComponent<UIManager>();
        if(_ui != null) _ui.ToggleWinPanel();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void SelectNewSpawnPoint()
    {
        if(spawnPoints.Length > 0){
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform newSpawn = spawnPoints[randomIndex];
            Debug.Log(newSpawn.name);
            PlayerSpawnManager.spawnPosition = newSpawn.position;
            PlayerSpawnManager.spawnRotation = newSpawn.rotation;

            // Debug.Log($"[RespawnPlayer] New Spawn Set: {PlayerSpawnManager.spawnPosition}, {PlayerSpawnManager.spawnRotation.eulerAngles}");
        }
    }

    private IEnumerator PlayGameOverSequence()
    {
        SoundFXManager.instance.PlaySoundFXClip(explosionSound, transform, 0.7f);
        yield return new WaitForSeconds(explosionSound.length - 0.5f);

        BGMusicManager.instance.ChangeToLose();

        SoundFXManager.instance.PlaySoundFXClip(objectiveLost, transform, 1f);
        yield return new WaitForSeconds(objectiveLost.length + 0.2f);

        SoundFXManager.instance.PlaySoundFXClip(goodLuck, transform, 1f);
        yield return new WaitForSeconds(goodLuck.length);

        respawnButton.SetActive(true);
    }

    private IEnumerator PlayGameWinSequence()
    {
        BGMusicManager.instance.ChangeToWin();

        SoundFXManager.instance.PlaySoundFXClip(missionAccomplished, transform, 1f);
        yield return new WaitForSeconds(objectiveLost.length + 0.2f);

        SoundFXManager.instance.PlaySoundFXClip(goodWork, transform, 1f);
        yield return new WaitForSeconds(goodWork.length);

        playAgainButton.SetActive(true);
    }
}
