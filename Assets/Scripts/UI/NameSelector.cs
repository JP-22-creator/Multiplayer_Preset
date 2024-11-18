using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{

    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button continueButton;

    public const String PlayerNameKey = "PlayerName";

    private void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        } // is server


        nameInput.text = PlayerPrefs.GetString(PlayerNameKey, "NAME");

    }



    public void Continue()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameInput.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}