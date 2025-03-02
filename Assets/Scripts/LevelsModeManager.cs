using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class LevelsModeManager : MonoBehaviour
{
    public LetterGrid letterGrid;
    public WordValidator wordValidator;
    public TMP_Text objectiveText, totalScoreText, timerText, messageText, levelText;

    private int totalScore = 0;
    private int wordsFormed = 0;
    private int requiredWords = 0;
    private int requiredScore = 0;
    private float timeLimit = 0f;
    private float timeRemaining = 0f;
    private bool isTimedLevel = false;
    private List<LetterTile> selectedTiles = new List<LetterTile>();
    private LevelDataList levelData;
    private HashSet<string> enteredWords = new HashSet<string>();
    private int currentLevelIndex = 0; // ✅ Track current level index

    void Start()
    {
        LoadLevelData();
    }
    
    void Update()
    {
        if (isTimedLevel && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
            if (timeRemaining <= 0)
            {
                EndLevel(false);
            }
        }
    }

    void LoadLevelData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "levelData.json");

        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);
            try
            {
                levelData = JsonConvert.DeserializeObject<LevelDataList>(jsonText);

                if (levelData == null || levelData.data == null || levelData.data.Count == 0)
                {
                    Debug.LogError("No levels found in JSON!");
                    return;
                }

                LoadLevel(currentLevelIndex); // ✅ Load the first level
            }
            catch (Exception e)
            {
                Debug.LogError("JSON Parsing Error: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("levelData.json not found!");
        }
    }

    void LoadLevel(int levelIndex)
    {
        if (levelIndex >= levelData.data.Count)
        {
            Debug.Log("🎉 All levels completed! Restarting game...");
            currentLevelIndex = 0; // ✅ Restart from level 0
            LoadLevel(currentLevelIndex);
            return;
        }

        // ✅ Destroy previous grid before generating a new one
        letterGrid.ClearGrid();

        LevelData level = levelData.data[levelIndex];
        Debug.Log($"🔄 Loading Level {levelIndex + 1}: {level.wordCount} words, Grid: {level.gridSize.x}x{level.gridSize.y}");

        letterGrid.GenerateCustomGrid(level.gridData);
        requiredWords = level.wordCount;
        requiredScore = level.totalScore;
        timeLimit = level.timeSec;
        timeRemaining = timeLimit;
        isTimedLevel = timeLimit > 0;

        // ✅ Reset progress for the new level
        totalScore = 0;
        wordsFormed = 0;
        enteredWords.Clear();
        selectedTiles.Clear();
        UpdateUI();
    }

    public void SubmitWord()
    {
        string word = GetSelectedWord().ToUpper();

        if (enteredWords.Contains(word))
        {
            ShowMessage("Word already used!");
            return;
        }
        if (wordValidator.IsValidWord(word))
        {
            enteredWords.Add(word);
            int wordScore = CalculateWordScore(word);

            foreach (LetterTile tile in selectedTiles)
            {
                if (tile.IsBonusTile())
                {
                    wordScore += 20;
                    ShowMessage($"Bonus letter used! +20 points");
                }
            }

            totalScore += wordScore;
            wordsFormed++;

            UnlockBlockedTiles();
            CheckLevelCompletion();
            UpdateUI();
        }

        DeselectTiles();
    }

    void UnlockBlockedTiles()
    {
        foreach (LetterTile tile in selectedTiles)
        {
            List<LetterTile> adjacentTiles = letterGrid.GetAdjacentTiles(tile);
            foreach (LetterTile adjacent in adjacentTiles)
            {
                if (adjacent.IsBlockedTile())
                {
                    adjacent.UnlockTile();
                    ShowMessage("Blocked tile unlocked!");
                }
            }
        }
    }

    int CalculateWordScore(string word)
    {
        return word.Length * 10;
    }

    void CheckLevelCompletion()
    {
        if ((requiredWords > 0 && wordsFormed >= requiredWords) ||
            (requiredScore > 0 && totalScore >= requiredScore))
        {
            EndLevel(true);
        }
    }

    void EndLevel(bool success)
    {
        if (success)
        {
            Debug.Log($"✅ Level {currentLevelIndex + 1} Completed!");
            currentLevelIndex++; // ✅ Move to next level
            LoadLevel(currentLevelIndex); // ✅ Load the next level
        }
        else
        {
            Debug.Log("❌ Level Failed! Restarting...");
            LoadLevel(currentLevelIndex); // ✅ Restart the current level
        }
    }

    public void SelectTile(LetterTile tile)
    {
        if (!selectedTiles.Contains(tile))
        {
            selectedTiles.Add(tile);
            tile.SelectTile();
        }
    }

    void DeselectTiles()
    {
        foreach (LetterTile tile in selectedTiles)
        {
            tile.DeselectTile();
        }
        selectedTiles.Clear();
    }

    void UpdateUI()
    {
        totalScoreText.text = "Total Score: " + totalScore;
        objectiveText.text = "Words: " + wordsFormed + "/" + requiredWords;
        timerText.text = isTimedLevel ? "Time: " + Mathf.CeilToInt(timeRemaining) : "";
        levelText.text = "Level: " + currentLevelIndex;
    }

    string GetSelectedWord()
    {
        string word = "";
        foreach (LetterTile tile in selectedTiles)
        {
            word += tile.GetLetter();
        }
        return word;
    }

    void ShowMessage(string msg)
    {
        messageText.text = msg;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), 2f);
    }

    void ClearMessage()
    {
        messageText.text = "";
    }
}
