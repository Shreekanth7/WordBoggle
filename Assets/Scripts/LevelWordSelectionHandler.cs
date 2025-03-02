using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelWordSelectionHandler : MonoBehaviour
{
    public LevelsModeManager levelsModeManager; // Reference to Levels Mode Manager
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
        }

        if (isDragging)
        {
            DetectTileSelection();
        }
    }

    void DetectTileSelection()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            LetterTile tile = hit.collider.GetComponent<LetterTile>();
            if (tile != null && !selectedTiles.Contains(tile))
            {
                selectedTiles.Add(tile);
                tile.SelectTile();
                levelsModeManager.SelectTile(tile);
            }
        }
    }

    void SubmitWord()
    {
        if (selectedTiles.Count > 0)
        {
            levelsModeManager.SubmitWord();
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