using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Globalization;

public class SettingsController : MonoBehaviour{

    [SerializeField]
    Toggle useVisualizationToggle;
    [SerializeField]
    TMP_InputField timeToActiveHandInput;
    [SerializeField]
    TMP_InputField playerNameInput;

    private static SettingsController _instance;
    public static SettingsController Instance {
        private set {
            _instance = value;
        }
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<SettingsController>();
                if (_instance == null) {
                    _instance = (new GameObject("Settings")).AddComponent<SettingsController>();
                }
            }

            return _instance;
        }
    }

    private void OnEnable() {
        if (useVisualizationToggle != null) {
            useVisualizationToggle.isOn = GetUseVisualization();
            useVisualizationToggle.onValueChanged.AddListener(SetUseVisualization);
        }

        if (timeToActiveHandInput != null) {
            timeToActiveHandInput.text = GetTimeToActiveHand().ToString();
            timeToActiveHandInput.onValueChanged.AddListener(TimeToActiveHandInputOnValueChanged);
        }

        if (playerNameInput != null) {
            playerNameInput.text = GetPlayerName();
            playerNameInput.onValueChanged.AddListener(SetPlayerName);
        }
    }

    private void OnDisable() {
        if(useVisualizationToggle != null) {
            useVisualizationToggle.onValueChanged.RemoveListener(SetUseVisualization);
            
        }
        if (timeToActiveHandInput != null) {
            timeToActiveHandInput.onValueChanged.RemoveListener(TimeToActiveHandInputOnValueChanged);
        }

        if (playerNameInput != null) {
            playerNameInput.onValueChanged.RemoveListener(SetPlayerName);
        }
    }


    public bool GetUseVisualization() {
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.useVisualizationKey)) {
            return PlayerPrefs.GetInt(PlayerPrefsKeys.useVisualizationKey) == 1 ? true : false;
        }
        return true; //default
    }

    public void SetUseVisualization(bool newValue) {
        PlayerPrefs.SetInt(PlayerPrefsKeys.useVisualizationKey, newValue ? 1 : 0);
        PlayerPrefs.Save();
    }

    public float GetTimeToActiveHand() {
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.timeToActiveHand)) {
            return PlayerPrefs.GetFloat(PlayerPrefsKeys.timeToActiveHand);
        }
        return 3f; //default
    }

    public void SetTimeToActiveHand(float newValue) {
        PlayerPrefs.SetFloat(PlayerPrefsKeys.timeToActiveHand, newValue);
        PlayerPrefs.Save();
    }

    public string GetPlayerName() {
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.playerName)) {
            return PlayerPrefs.GetString(PlayerPrefsKeys.playerName);
        }
        return "Player"; //default
    }

    public void SetPlayerName(string newValue) {
        PlayerPrefs.SetString(PlayerPrefsKeys.playerName, newValue.Trim());
        PlayerPrefs.Save();
    }

    void TimeToActiveHandInputOnValueChanged(string call) {
        float j = float.Parse(call, CultureInfo.InvariantCulture.NumberFormat);
        if (j > 0) {
            SetTimeToActiveHand(j);
        }
    }

}

public static class PlayerPrefsKeys{
    public static string useVisualizationKey = "useVisualization";
    public static string timeToActiveHand = "timeToActiveHand";
    public static string playerName = "playerName";
}
