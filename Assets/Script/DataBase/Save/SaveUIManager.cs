using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerInfoText;
    [SerializeField] private TMP_Text statusText;

    private PlayerRepository playerRepo;
    private SaveRepository saveRepo;

    void Start()
    {
        var db = DbProvider.Connection;
        playerRepo = new PlayerRepository(db);
        saveRepo = new SaveRepository(db);

        ShowPlayerInfo();
    }

    void ShowPlayerInfo()
    {
        // var player = playerRepo.GetById(GameSession.PlayerId); // Player's ID จาก LogIn scene
        // if (player == null) return;
        // playerInfoText.text = "Welcome: " + player.Name + "\nDay: " + player.Day + "\nGold: " + player.Gold;
    }

    public void OnSave()
    {
        // var player = playerRepo.GetById(GameSession.PlayerId);
        // var playerObj = GameObject.FindGameObjectWithTag("Player");
        // if (player == null || playerObj == null) return;

        // var pos = playerObj.transform.position;
        // string sceneName = SceneManager.GetActiveScene().name;

        // saveRepo.Save(GameSession.PlayerId, sceneName, player.Day, player.Gold, pos.x, pos.y, pos.z);
        // statusText.text = $"Saved at ({pos.x:0.0}, {pos.y:0.0}, {pos.z:0.0})";

        // ShowPlayerInfo();
    }

    public void OnLoad()
    {
        SaveData save = saveRepo.Load(GameSession.PlayerId);
        if (save == null)
        {
            statusText.text = "No saved data";
            return;
        }

        statusText.text = $"Loaded: ({save.PosX}, {save.PosY}, {save.PosZ}) \n Day: {save.Day}\n Gold: {save.Gold}";
    }

    public void OnLogOut()
    {
        GameSession.LogOut();
        SceneManager.LoadScene("Hub");
    }

}
