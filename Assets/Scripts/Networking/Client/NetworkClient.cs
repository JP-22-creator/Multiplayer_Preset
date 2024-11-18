using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private const string menuSceneName = "Menu";
    private NetworkManager networkManager;

    public NetworkClient(NetworkManager nM)
    {
        networkManager = nM;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public void Dispose() // bcs we arnt a MonoB / is called from class using this 
    {
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

    private void OnClientDisconnect(ulong cliendId)
    {
        if (cliendId != 0 && cliendId != networkManager.LocalClientId) // safty that its not the Host
        {
            return;
        }

        if (SceneManager.GetActiveScene().name != menuSceneName) // if we are still in the game
        {
            SceneManager.LoadScene(menuSceneName);
        }

        if (networkManager.IsConnectedClient) // maybe we have to put this first!
        {
            networkManager.Shutdown();
        }

    }



}