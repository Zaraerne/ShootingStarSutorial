using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : PresistentSingleton<ScoreManager>
{
    #region SCORE DISPLAY
    int score;
    int currentScore;
    [SerializeField] Vector3 scoreTextScale = new Vector3(1.2f, 1.2f, 1f);

    public void ResetScore()
    {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpdateText(0);
    }

    public void AddScore(int scorePoint)
    {
        currentScore += scorePoint;
        StartCoroutine(nameof(AddScoreCoroutine));
    }

    public int Sorce => score;

    IEnumerator AddScoreCoroutine()
    {
        ScoreDisplay.ScaleText(scoreTextScale);
        while (score < currentScore)
        {
            score += 1;
            ScoreDisplay.UpdateText(score);

            yield return null;
        }
        ScoreDisplay.ScaleText(Vector3.one);
    }
    #endregion


    #region HIGH SCORE SYSTEM
    [System.Serializable]
    public class PlayerScore
    {
        public int score;
        public string playerName;

        public PlayerScore(int sorce,string playerName)
        {
            this.score = sorce;
            this.playerName = playerName;
        }

    }
    readonly string SaveFileName = "player_score.json";
    string playerName = "NO Name";

    public bool HasNewHighScore => score > LoadPlayerScoreData().list[9].score;

    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }


    [System.Serializable]
    public class PlayerScoreData
    {
        public List<PlayerScore> list = new List<PlayerScore>();
    }

    public void SavePlayerScoreData()
    {
        var playerScoreData = LoadPlayerScoreData();

        playerScoreData.list.Add(new PlayerScore(score, playerName));

        playerScoreData.list.Sort((x, y) => y.score.CompareTo(x.score));

        SaveSystemTutorial.SaveSystem.SaveByJson(SaveFileName, playerScoreData);

    }

    public PlayerScoreData LoadPlayerScoreData()
    {
        var playerScoreData = new PlayerScoreData();
        if (SaveSystemTutorial.SaveSystem.SaveFileExists(SaveFileName))
        {
            playerScoreData = SaveSystemTutorial.SaveSystem.LoadFromJson<PlayerScoreData>(SaveFileName);
        }
        else
        {
            while(playerScoreData.list.Count < 10)
            {
                playerScoreData.list.Add(new PlayerScore(0, playerName));
            }
            SaveSystemTutorial.SaveSystem.SaveByJson(SaveFileName, playerScoreData);
        }

        return playerScoreData;
    }



    #endregion

}
