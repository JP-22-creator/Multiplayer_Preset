using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

    async void Start() // game startup -> check server or client 
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        // check if we are server or not
    }

    private async Task LaunchInMode(bool isDedicatedServer) // create the needed objects
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            // the Singletons allow acces too the functionality

            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost(); // has to be created first bcs the scene changes  too fast for DDOL to call

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool auth = await clientSingleton.CreateClient(); // this one connects too the UGS



            if (auth)
            {
                clientSingleton.GameManager.GoToMenu(); // change scene
            }


        }
    }

}
