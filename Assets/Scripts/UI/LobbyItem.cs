using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameField;
    [SerializeField] private TMP_Text players;

    private LobbiesList lobbies;
    private Lobby lobby;

    public void InitLobby(Lobby lobby, LobbiesList lobbiesList)
    {
        this.lobby = lobby;
        lobbies = lobbiesList;
        nameField.text = lobby.Name;
        players.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbies.JoinAsync(lobby);
    }

}