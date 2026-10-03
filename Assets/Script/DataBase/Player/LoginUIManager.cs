using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class LoginUIManager : MonoBehaviour
{
    private PlayerRepository playerRepo;
    private SaveRepository saveRepo;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject LoginPage;
    [SerializeField] private GameObject StartPage;
    private int loggedInPlayerId = -1;

    void Awake()
    {
        LoginPage.SetActive(false);
        StartPage.SetActive(true);
    }

    void Start()
    {
        var db = DbProvider.Connection;
        playerRepo = new PlayerRepository(db);
        saveRepo = new SaveRepository(db);
    }

    public void OnRegister()
    {
        string name = nameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            statusText.text = "Pls Fill Your name";
            return;
        }

        var existing = playerRepo.GetByName(name);
        if (existing != null)
        {
            Debug.Log("This username exists!!!");
            statusText.text = "Username already exists. Use Login instead.";
            return;
        }

        Player player = playerRepo.AddPlayer(name);
        statusText.text = "Registered & logged in as " + name;
        GameSession.PlayerId = player.Id;
        loggedInPlayerId = player.Id;
    }

    public void OnLogIn()
    {
        string username = nameInput.text.Trim();
        if (string.IsNullOrEmpty(username)) return; // If input string is null

        var player = playerRepo.GetByName(username);
        // Check log in information
        if (player == null)
        {
            statusText.text = "Username doesn't exist. Please Register.";
            return;
        }

        statusText.text = "Logged in as " + username;
        loggedInPlayerId = player.Id;
        GameSession.PlayerId = player.Id;  //loggedInPlayerId
        Debug.Log("Player ID: " + loggedInPlayerId);

        if (loggedInPlayerId > 0) EnterGame(loggedInPlayerId);
    }

    // เช็คว่า player คนนี้เคยมี save ไว้ไหม ถ้ามีให้กลับไป scene/ตำแหน่งที่เซฟไว้ ถ้าไม่มีให้เริ่มที่ Hub
    private void EnterGame(int playerId)
    {
        var save = saveRepo.Load(playerId);
        string sceneName = save != null ? save.SceneName : "Hub";

        if (string.IsNullOrEmpty(sceneName))
            sceneName = "Hub";

        Debug.Log(save != null
            ? $"EnterGame: พบ save เดิม -> โหลด scene {sceneName}"
            : "EnterGame: ไม่มี save เดิม -> เริ่มที่ Hub");

        SceneManager.LoadScene(sceneName);
    }

    public void OnPlayGame()
    {
        LoginPage.SetActive(true);
        StartPage.SetActive(false);
    }

}
