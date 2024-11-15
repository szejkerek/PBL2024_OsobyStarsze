using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Text;

public class PlayersScoreController : MonoBehaviour{

    private List<PlayerScore> savedPlayersScores = new List<PlayerScore>();

    private static PlayersScoreController _instance;
    public static PlayersScoreController Instance {
        private set {
            _instance = value;
        }
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<PlayersScoreController>();
                if (_instance == null) {
                    _instance = (new GameObject("PlayersScoreController")).AddComponent<PlayersScoreController>();
                }
            }

            return _instance;
        }
    }

    public async UniTask<List<PlayerScore>> HandleScoreHistory(int newScore) {
        savedPlayersScores.Clear();
        FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, "saves.csv"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamReader dd = new StreamReader(fileStream, Encoding.UTF8);
        string str = await dd.ReadToEndAsync();
        string[] lines = str.Split(new string[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++) {
            string[] values = lines[i].Split(';');
            int tempScore;
            if (values.Length == 2 && int.TryParse(values[1], out tempScore)) {
                savedPlayersScores.Add(new PlayerScore(values[0], tempScore));
            }
        }

        PlayerScore newPlayerScore = new PlayerScore(SettingsController.Instance.GetPlayerName(), newScore);
        savedPlayersScores.Add(newPlayerScore);
        byte[] buffor = Encoding.ASCII.GetBytes(newPlayerScore.ToString());
        await fileStream.WriteAsync(buffor, 0, buffor.Length);

        savedPlayersScores.Sort((x, y) => {
            return x.score < y.score ? 1 : -1;
        });

        await fileStream.FlushAsync();

        return new List<PlayerScore>(savedPlayersScores);
    }

    public class PlayerScore {
        public string playerName;
        public int score;
        public PlayerScore(string _playerName, int _score) {
            playerName = _playerName;
            score = _score;
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.Append(playerName);
            builder.Append(";");
            builder.Append(score.ToString());
            builder.Append(Environment.NewLine);
            return builder.ToString();
        }
    }

}
