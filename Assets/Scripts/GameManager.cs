using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Used variables.
    private GameObject words;
    private List<Transform> wordRows;
    private int characterIndex;
    private int rowIndex;
    private int maxLettersQuantity;
    public string currentGuess;
    public string randomWord;

    // Start is called before the first frame update.
    private void Start()
    {
        randomWord = GetRandomWord();
        words = GameObject.Find("Words");
        wordRows = new List<Transform>();

        foreach (Transform child in words.transform)
            wordRows.Add(child);

        characterIndex = 0;
        rowIndex = 0;
        maxLettersQuantity = 5;
        currentGuess = string.Empty;
    }

    // Displays the selected character key at the "guess word" letter box.
    public void DisplayLetter(string letterValue)
    {
        if (characterIndex < maxLettersQuantity)
        {
            Transform wordRow = wordRows[rowIndex];
            Transform letter = wordRow.transform.GetChild(characterIndex);
            TextMeshProUGUI letterText = letter.GetChild(0).GetComponent<TextMeshProUGUI>();
            letterText.text = letterValue;
            currentGuess += letterValue;

            characterIndex++;
        }
    }

    // Removes the last character.
    public void UndoLastCharacter()
    {
        if (characterIndex > 0)
        {
            characterIndex--;

            Transform wordRow = wordRows[rowIndex];
            Transform letter = wordRow.transform.GetChild(characterIndex);
            TextMeshProUGUI letterText = letter.GetChild(0).GetComponent<TextMeshProUGUI>();
            letterText.text = string.Empty;
            currentGuess = currentGuess.Remove(currentGuess.Length - 1); 
        }
    }

    // Confirms the current row, and goes to the next one.
    public void ConfirmRow()
    {
        if (characterIndex == maxLettersQuantity)
        {
            CompareTarget();
            characterIndex = 0;
            rowIndex++;
            currentGuess = string.Empty;
        }
    }

    public void CompareTarget()
    {
        // if ()
    }

    // Returns the current character index value.
    public int GetCharacterIndex()
    {
        return characterIndex;
    }

    // Returns the max letter quantity value.
    public int GetMaxLettersQuantity()
    {
        return maxLettersQuantity;
    }

    public string GetRandomWord()
    {
        return "GREAT";
    }    
}
