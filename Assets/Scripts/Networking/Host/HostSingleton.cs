using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour // enable global acces too HGM 
{
    public HostGameManager GameManager { get; private set; }



    private static HostSingleton instance;

    public static HostSingleton Instance  // singleton pattern 
    {
        get
        {
            if (instance != null) return instance;

            instance = FindFirstObjectByType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene");
                return null;
            }

            return instance;

        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void CreateHost()
    {
        GameManager = new HostGameManager();


    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }

}
