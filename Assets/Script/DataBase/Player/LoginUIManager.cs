using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginUIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject LoginPage;
    [SerializeField] private GameObject StartPage;

    private PlayerRepository playerRepo;
    private SaveRepository saveRepo;

    void Awake()
    {
        if (LoginPage != null) LoginPage.SetActive(false);
        if (StartPage != null) StartPage.SetActive(true);
    }

    void Start()
    {
        var db = DbProvider.Connection;
        playerRepo = new PlayerRepository(db);
        saveRepo = new SaveRepository(db);
    }

    public void OnRegister()
    {
        string name = ReadName();
        if (string.IsNullOrEmpty(name))
        {
            SetStatus("Pls Fill Your name");
            return;
        }

        if (playerRepo.GetByName(name) != null)
        {
            SetStatus("Username already exists. Use Login instead.");
            return;
        }

        Players player = playerRepo.AddPlayer(name);
        GameSession.PlayerId = player.Id;
        SetStatus("Registered & logged in as " + name);
        EnterGame(player.Id);
    }

    public void OnLogIn()
    {
        string name = ReadName();
        if (string.IsNullOrEmpty(name))
        {
            SetStatus("Pls Fill Your name");
            return;
        }

        Players player = playerRepo.GetByName(name);
        if (player == null)
        {
            SetStatus("Username doesn't exist. Please Register.");
            return;
        }

        GameSession.PlayerId = player.Id;
        SetStatus("Logged in as " + name);
        EnterGame(player.Id);
    }

    // ปุ่ม Play ในหน้าแรก — ไปหน้า Login (ถ้าซีนไม่มีหน้า Login จะเข้าเกมแบบไม่เซฟ)
    public void OnPlayGame()
    {
        if (LoginPage == null)
        {
            SceneManager.LoadScene(GameSession.GameScene);
            return;
        }

        LoginPage.SetActive(true);
        if (StartPage != null) StartPage.SetActive(false);
    }

    // มี save เดิมให้เข้าซีนที่เซฟไว้ ถ้าไม่มีเริ่มที่ซีนเกมหลัก
    private void EnterGame(int playerId)
    {
        SaveData save = saveRepo.Load(playerId);
        string sceneName = save != null ? save.SceneName : null;

        if (string.IsNullOrEmpty(sceneName) || !Application.CanStreamedLevelBeLoaded(sceneName))
            sceneName = GameSession.GameScene;

        SceneManager.LoadScene(sceneName);
    }

    private string ReadName()
    {
        return nameInput != null ? nameInput.text.Trim() : "";
    }

    private void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }
}
