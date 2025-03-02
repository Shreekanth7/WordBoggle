using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LetterGrid : MonoBehaviour
{
    public GameObject letterTilePrefab;
    public float tileSpacing = 1.1f;
    public GridLayoutGroup gridLayoutGroup;
    private LetterTile[,] grid;
    private int gridSizeX;
    private int gridSizeY;

    public void GenerateCustomGrid(List<GridTile> gridData)
    {
        gridSizeX = Mathf.RoundToInt(Mathf.Sqrt(gridData.Count));
        gridSizeY = gridSizeX;
        gridLayoutGroup.constraintCount = gridSizeX;
        grid = new LetterTile[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                int index = y * gridSizeX + x;
                GridTile tileData = gridData[index];

                Vector3 spawnPos = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                GameObject newTile = Instantiate(letterTilePrefab, spawnPos, Quaternion.identity, transform);
                
                LetterTile tileScript = newTile.GetComponent<LetterTile>();
                tileScript.SetGridPosition(x, y);
                tileScript.SetLetter(tileData.letter[0]);
                
                if (tileData.tileType == 1) 
                {
                    tileScript.SetBlocked(true);
                }
                else if (tileData.tileType == 2) 
                {
                    tileScript.SetBonus(true);
                }

                grid[x, y] = tileScript;
            }
        }
    }
    
    public void RemoveTilesAndRefill(List<LetterTile> usedTiles)
{
    Dictionary<int, int> emptyColumns = new Dictionary<int, int>();

    HashSet<Vector2Int> removedTiles = new HashSet<Vector2Int>();

    foreach (LetterTile tile in usedTiles)
    {
        int x = tile.gridX;
        int y = tile.gridY;

        Vector2Int tilePos = new Vector2Int(x, y);

        if (removedTiles.Contains(tilePos))
        {
            Debug.LogWarning($"Tile at ({x}, {y}) was already removed!");
            continue;
        }

        if (grid[x, y] != null)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
            removedTiles.Add(tilePos);
        }

        if (!emptyColumns.ContainsKey(x))
        {
            emptyColumns[x] = 1;
        }
        else
        {
            emptyColumns[x]++;
        }
    }

    // Shift tiles downward
    foreach (var column in emptyColumns)
    {
        int x = column.Key;
        int emptyCount = column.Value;

        for (int y = 0; y < gridSizeY; y++)
        {
            if (grid[x, y] == null)
            {
                for (int shiftY = y + 1; shiftY < gridSizeY; shiftY++)
                {
                    if (grid[x, shiftY] != null)
                    {
                        grid[x, y] = grid[x, shiftY];
                        grid[x, shiftY] = null;
                        grid[x, y].SetGridPosition(x, y);
                        grid[x, y].transform.position = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                        break;
                    }
                }
            }
        }

        // Spawn new tiles at the top ONLY if needed
        for (int y = gridSizeY - emptyCount; y < gridSizeY; y++)
        {
            if (grid[x, y] == null) // ✅ Check if the space is actually empty before spawning
            {
                Vector3 spawnPos = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                GameObject newTile = Instantiate(letterTilePrefab, spawnPos, Quaternion.identity, transform);
                char newLetter = GetRandomLetter();
                LetterTile newTileScript = newTile.GetComponent<LetterTile>();
                newTileScript.SetLetter(newLetter);
                newTileScript.SetGridPosition(x, y);
                grid[x, y] = newTileScript;
            }
        }
    }
}

    public List<LetterTile> GetAdjacentTiles(LetterTile tile)
    {
        List<LetterTile> adjacentTiles = new List<LetterTile>();
        int x = tile.gridX;
        int y = tile.gridY;

        // Define possible movement directions (Top, Bottom, Left, Right)
        int[,] directions = { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = x + directions[i, 0];
            int newY = y + directions[i, 1];

            // Ensure the new position is within bounds
            if (newX >= 0 && newX < gridSizeX && newY >= 0 && newY < gridSizeY)
            {
                if (grid[newX, newY] != null) // Ensure the tile exists
                {
                    adjacentTiles.Add(grid[newX, newY]);
                }
            }
        }

        return adjacentTiles;
    }


    public void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); //Destroy all existing tiles
        }
        grid = new LetterTile[gridSizeX, gridSizeY]; //Reset grid array
    }

    
    char GetRandomLetter()
    {
        string letters = "EEEEEAAAIIOOOUUYNBCDFGHJKLMNPQRSTVWXYZ"; // Weighted distribution for common letters
        return letters[Random.Range(0, letters.Length)];
    }
}