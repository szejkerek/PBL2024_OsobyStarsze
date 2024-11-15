using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreRow : MonoBehaviour{

    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI score;

    public void Init(string _playerName, string _score) {
        playerName.text = _playerName;
        score.text = _score;
    }

}
