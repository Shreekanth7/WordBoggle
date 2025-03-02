using System.Collections.Generic;

[System.Serializable]
public class LevelDataList
{
    public List<LevelData> data;
}

[System.Serializable]
public class LevelData
{
    public int bugCount;
    public int wordCount;
    public float timeSec;
    public int totalScore;
    public GridSize gridSize;
    public List<GridTile> gridData;
}

[System.Serializable]
public class GridSize
{
    public int x;
    public int y;
}

[System.Serializable]
public class GridTile
{
    public int tileType;
    public string letter;
}