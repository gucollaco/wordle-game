using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private GameObject words;
    private List<Transform> wordRows;
    private int characterIndex;
    private int rowIndex;

    // Start is called before the first frame update.
    private void Start()
    {
        words = GameObject.Find("Words");
        wordRows = new List<Transform>();

        foreach (Transform child in words.transform)
        {
            Debug.Log(child);
            wordRows.Add(child);
        }

        characterIndex = 0;
        rowIndex = 0;
    }

    public void DisplayLetter(string letterValue)
    {
        Transform wordRow = wordRows[rowIndex];
        Transform letter = wordRow.transform.GetChild(characterIndex);
        TextMeshProUGUI letterText = letter.GetChild(0).GetComponent<TextMeshProUGUI>();
        letterText.text = letterValue;

        characterIndex++;
    }
}
