using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using static PlayersScoreController;

public class CurrentGamePatientData : MonoBehaviour
{
    private List<PlayerScore> savedPlayersScores = new List<PlayerScore>();
    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public async UniTask<List<PlayerScore>> HandleScoreHistory(int newScore)
    {
        savedPlayersScores.Clear();
        FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, "saves.csv"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamReader dd = new StreamReader(fileStream, Encoding.UTF8);
        string str = await dd.ReadToEndAsync();
        string[] lines = str.Split(new string[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(';');
            int tempScore;
            if (values.Length == 2 && int.TryParse(values[1], out tempScore))
            {
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
}
