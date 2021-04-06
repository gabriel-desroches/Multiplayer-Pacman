using UnityEngine;

public class UIManager : MonoBehaviour
{
    private const string playerNamePrefKey = "PlayerName";
    public void SetPlayerName(string PlayerName)
    {
        print("SetPlayerName Called");
        if (string.IsNullOrWhiteSpace(PlayerName))
            return;

        PlayerPrefs.SetString(playerNamePrefKey, PlayerName);
    }
}
