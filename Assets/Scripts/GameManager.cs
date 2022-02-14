using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update.
    private void Start()
    {
        words = GameObject.Find("Words");
        wordRows = new List<Transform>();

        foreach (Transform child in words.transform)
            wordRows.Add(child);

        characterIndex = 0;
        rowIndex = 0;
        maxLettersQuantity = 5;
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

        }
    }

    // Confirms the current row, and goes to the next one.
    public void NextRow()
    {
        characterIndex = 0;
        rowIndex++;
    }
}
