using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    private const int maxConnections = 4;
    private const String gameSceneName = "Game";
    private Allocation allocation;
    private String joinCode;
    private String lobbyID;


    private NetworkServer networkServer;

    public async void Dispose()
    {
        networkServer?.Dispose();
        HostSingleton.Instance.StopCoroutine(nameof(HearthBeatLobby));

        if (!string.IsNullOrEmpty(lobbyID)) // we still have a lobby ID
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyID);
            }
            catch (LobbyServiceException LSE)
            {
                Debug.Log(LSE);
            }
        }
    }

    public async Task StartHostAsync()
    {

        allocation = await GetAllocation();
        if (allocation == null) return;

        joinCode = await GetJoinCode();
        // this is relay 


        await GetLobby();
        // put lobby online and start hearthbeat



        // process of changing transport too relay if needed!
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>(); // get acces
        RelayServerData relayServerData = new RelayServerData(allocation, "udp"); // package the select
        transport.SetRelayServerData(relayServerData); // set too use relay


        networkServer = new NetworkServer(NetworkManager.Singleton);


        UserData userData = new UserData
        {
            userName = "Get Name for Player in Bootstrap",  // TODO get playername
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        // Disables clients from chaning scene 
        // When a client connects to server it will be brought too this scene
        // Inits the NetworkObjects correctl

    }

    private async Task<Allocation> GetAllocation()
    {
        Allocation alloc;
        try
        {
            alloc = await Relay.Instance.CreateAllocationAsync(maxConnections);// returns the alloc we got
        }
        catch (Exception E)
        {
            Debug.Log(E.Message);
            return null;
        }

        return alloc;
    }

    private async Task<String> GetJoinCode()
    {
        string code;
        try
        {
            code = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId); // returns joinCode for this alloc 
            Debug.Log(code);
        }
        catch (Exception E)
        {
            Debug.Log(E.Message);
            return "";
        }

        return code;
    }



    private async Task GetLobby()
    {
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<String, DataObject>() // parse information too Lobbie object on network
            {
                {
                    "JoinCode", // key
                    new DataObject( // value
                    visibility: DataObject.VisibilityOptions.Member, // TODO look into Member privat public
                    value: joinCode
                    )
                }
            };

            //TODO get Lobbyname
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("Lobby name here", maxConnections, lobbyOptions);
            lobbyID = lobby.Id;
            HostSingleton.Instance.StartCoroutine(HearthBeatLobby(15f));
        }
        catch (LobbyServiceException LSE)
        {
            Debug.LogError(LSE.Message);
            return;
        }
    }



    private IEnumerator HearthBeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyID); // has to ping too keep  the lobbie exisiting
            yield return delay;
        }
    }


}