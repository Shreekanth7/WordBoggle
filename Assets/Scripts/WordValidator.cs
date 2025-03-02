using System.Collections.Generic;
using UnityEngine;

public class WordValidator : MonoBehaviour
{
    private HashSet<string> validWords = new HashSet<string>();

    void Start()
    {
        LoadWordList();
    }

    void LoadWordList()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("wordList");
        if (wordFile != null)
        {
            string[] words = wordFile.text.Split('\n');
            foreach (string word in words)
            {
                validWords.Add(word.Trim().ToUpper());
            }
            Debug.Log("Word list loaded: " + validWords.Count + " words.");
        }
        else
        {
            Debug.LogError("wordList.txt not found in Resources!");
        }
    }

    public bool IsValidWord(string word)
    {
        return validWords.Contains(word.ToUpper());
    }
}