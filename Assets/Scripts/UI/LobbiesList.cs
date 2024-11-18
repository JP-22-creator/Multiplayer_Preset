using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;

    private bool isJoining = false;
    private bool isRefreshing = false;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;
        else isRefreshing = true;

        try
        {
            // Which kind of lobbies we want to show
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 10; // how many
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter( // make sure >0 free slots in lobby ( dont show full )
                    field: QueryFilter.FieldOptions.AvailableSlots, // what to check
                    op: QueryFilter.OpOptions.GT, // which operation
                    value: "0"
                ),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };

            QueryResponse query = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in lobbyItemParent) // clear ui
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in query.Results) // update ui
            {
                LobbyItem li = Instantiate(lobbyItemPrefab, lobbyItemParent);
                li.InitLobby(lobby, this);
            }


        }
        catch (LobbyServiceException LSE)
        {
            Debug.Log(LSE.Message);
        }





        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        else isJoining = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id); // return lobby that enables alloc
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);

        }
        catch (LobbyServiceException LSE)
        {
            Debug.LogError(LSE.Message);
        }

        isJoining = false;
    }
}