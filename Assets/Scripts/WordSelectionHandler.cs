using System.Collections.Generic;
using UnityEngine;

public class WordSelectionHandler : MonoBehaviour
{
    public EndlessModeManager endlessModeManager;
    private List<LetterTile> selectedTiles = new List<LetterTile>();
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            selectedTiles.Clear();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SubmitWord();
            Debug.Log($"Mouse released");
        }

        if (isDragging)
        {
            DetectTileSelection();
        }
    }

    void DetectTileSelection()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); // ✅ More reliable than OverlapPoint

        if (hit.collider != null)
        {
            LetterTile tile = hit.collider.GetComponent<LetterTile>();
            if (tile != null && !selectedTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
                tile.SelectTile();
                endlessModeManager.SelectTile(tile); // ✅ Send selection to LevelsModeManager
            }
        }
    }



    void SubmitWord()
    {
        if (selectedTiles.Count > 0)
        {
            //  EndlessModeManager's SubmitWord()
            endlessModeManager.SubmitWord();
            ResetSelection();
        }
    }

    void ResetSelection()
    {
        foreach (LetterTile tile in selectedTiles)
        {
            tile.DeselectTile();
        }
        selectedTiles.Clear();
    }
}