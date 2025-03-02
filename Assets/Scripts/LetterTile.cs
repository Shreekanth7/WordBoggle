using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterTile : MonoBehaviour
{
    private char letter;
    private bool isSelected = false;
    private bool isBlocked = false;
    private bool isBonus = false;
    [SerializeField] private Image image;
    public int gridX;
    public int gridY;
    public TMP_Text letterText; // Assign a UI Text component in the prefab
    public Image bonusImagee;
    public Image blockedImage;

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    public void SetLetter(char newLetter)
    {
        letter = newLetter;
        letterText.text = letter.ToString();
    }

    public char GetLetter()
    {
        return letter;
    }

    public void SelectTile()
    {
        if (isBlocked) return;

        isSelected = true;
        image.color = Color.green;
    }

    public void DeselectTile()
    {
        isSelected = false;
        image.color = Color.white;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
    
    public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
        blockedImage.gameObject.SetActive(isBlocked);
    }

    public bool IsBlockedTile()
    {
        return isBlocked;
    }

    public void UnlockTile()
    {
        isBlocked = false;
        blockedImage.gameObject.SetActive(isBlocked);
    }

    public void SetBonus(bool bonus)
    {
        isBonus = bonus;
        bonusImagee.gameObject.SetActive(isBonus);
    }

    public bool IsBonusTile()
    {
        return isBonus;
    }
}