using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private Dictionary<ulong, string> clientTOAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authTOUserData = new Dictionary<string, UserData>();

    private NetworkManager networkManager;

    public NetworkServer(NetworkManager nM)
    {
        networkManager = nM;
        networkManager.ConnectionApprovalCallback += ApprovalCheck; // triggered on connection
                                                                    // can retrive some connection info
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        UserData userData = Desirealise(request);
        // now we can check if we want to approve

        clientTOAuth[request.ClientNetworkId] = userData.userAuthId;
        authTOUserData[userData.userAuthId] = userData;
        // use instead of .Add() bcs it will override the existing value or add if new





        response.Approved = true;
        response.CreatePlayerObject = true;
        //maybe we can change the spawn position





    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong cliendId)
    {
        if (clientTOAuth.TryGetValue(cliendId, out string authId))
        {
            clientTOAuth.Remove(cliendId);
            authTOUserData.Remove(authId);
        }
    }

    private UserData Desirealise(NetworkManager.ConnectionApprovalRequest input)
    {
        string payload = System.Text.Encoding.UTF8.GetString(input.Payload); // Byte -> String 
        return JsonUtility.FromJson<UserData>(payload); // String -> JSON -> UserData

    }

    public void Dispose()
    {
        if (networkManager != null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.OnServerStarted -= OnNetworkReady;
        }

        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }

    }
}