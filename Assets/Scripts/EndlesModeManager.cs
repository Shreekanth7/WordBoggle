using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class EndlessModeManager : MonoBehaviour
{
    public LetterGrid letterGrid;
    public WordValidator wordValidator;
    public TMP_Text totalScoreText;
    public TMP_Text averageScoreText;
    public TMP_Text messageText;
    
    private int totalScore = 0;
    private int wordsFormed = 0;
    private List<GridTile> gridData;
    private List<LetterTile> selectedTiles = new List<LetterTile>();
    private HashSet<string> enteredWords = new HashSet<string>();

    void Start()
    {
        if (letterGrid != null)
        {
            List<GridTile> defaultGridData = GenerateDefaultGridData();
            letterGrid.GenerateCustomGrid(defaultGridData);
        }
        else
        {
            Debug.LogError("LetterGrid reference is missing in EndlessModeManager!");
        }

        UpdateUI();
    }

    public void SelectTile(LetterTile tile)
    {
        if (!selectedTiles.Contains(tile))
        {
            selectedTiles.Add(tile);
            tile.SelectTile();
        }
    }

    public void OnExitClicked()
    {
        SceneLoader.Instance.LoadSceneAsync("MainMenu");
    }
    
    public void SubmitWord()
    {
        Debug.Log($"selectedTiles.Count = {selectedTiles.Count}");
        if (selectedTiles.Count == 0) return;

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
            totalScore += wordScore;
            wordsFormed++;

            UpdateUI();

            letterGrid.RemoveTilesAndRefill(selectedTiles);
        }

        DeselectTiles();
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

    int CalculateWordScore(string word)
    {
        return word.Length * 10; // 10 points per letter
    }

    void DeselectTiles()
    {
        foreach (LetterTile tile in selectedTiles)
        {
            tile.DeselectTile();
        }
        selectedTiles.Clear();
    }
    
    List<GridTile> GenerateDefaultGridData()
    {
        List<GridTile> gridData = new List<GridTile>();

        for (int i = 0; i < 16; i++) // 4x4 grid = 16 tiles
        {
            GridTile tile = new GridTile
            {
                tileType = 0, // Normal tile
                letter = GetRandomLetter().ToString()
            };
            gridData.Add(tile);
        }

        return gridData;
    }

    char GetRandomLetter()
    {
        string letters = "EEEEEAAAIIOOOUUYNBCDFGHJKLMNPQRSTVWXYZ"; // Weighted distribution for common letters
        return letters[Random.Range(0, letters.Length)];
    }
    
    void UpdateUI()
    {
        totalScoreText.text = "Total Score: " + totalScore;
        averageScoreText.text = wordsFormed > 0 ? "Avg Score: " + (totalScore / wordsFormed) : "Avg Score: 0";
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
