using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{

    private const string MenuSceneName = "Menu";
    private JoinAllocation joinAllocation;
    private NetworkClient networkClient;

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync(); // very important

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public async Task StartClientAsync(string joinCode)
    {
        joinAllocation = await GetAlloction(joinCode);
        if (joinAllocation == null) return;

        // setup too use Relay
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(joinAllocation, "udp");
        transport.SetRelayServerData(relayServerData);


        //Send locally grabbed data too server
        UserData userData = new UserData
        {
            userName = "Get Playername from Bootstrap", //TODO get playername
            userAuthId = AuthenticationService.Instance.PlayerId // available after authentication / uniq id 
        };

        byte[] payloadBytes = SerializeData(userData);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes; // will be send when connecting


        NetworkManager.Singleton.StartClient();
        // this should already change the Scene 
    }


    private async Task<JoinAllocation> GetAlloction(string code)
    {
        JoinAllocation alloc;
        try
        {
            alloc = await Relay.Instance.JoinAllocationAsync(code);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        return alloc;
    }


    private byte[] SerializeData(UserData userData)
    {
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        return payloadBytes;
    }


    public void GoToMenu()
    {
        // Authenticate player
        SceneManager.LoadScene(MenuSceneName);
    }

    public void Dispose()
    {

        networkClient?.Dispose();
    }
}
