using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;


    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }



}
