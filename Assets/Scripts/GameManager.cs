using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Used variables.
    public string currentGuess;
    public string randomWord;
    public GameObject gameEndPanel;
    public TextMeshProUGUI gameEndText;
    public GameObject words;
    private List<Transform> wordRows;
    private int characterIndex;
    private int rowIndex;
    private int maxLettersQuantity;
    private bool isGameActive;

    // Start is called before the first frame update.
    private void Start()
    {
        randomWord = GetRandomWord();
        wordRows = new List<Transform>();

        foreach (Transform child in words.transform)
            wordRows.Add(child);

        characterIndex = 0;
        rowIndex = 0;
        maxLettersQuantity = 5;
        currentGuess = string.Empty;
        isGameActive = true;

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
        if (rowIndex == wordRows.Count - 1)
            GameEndLost();
        else if (characterIndex == maxLettersQuantity)
        {
            bool hasWon = CompareTarget();

            characterIndex = 0;
            rowIndex++;
            currentGuess = string.Empty;

            if (hasWon)
                GameEndWon();
        }
    }

    // Compares the guess with the selected random word. Returns true in case the user discovers the hidden word.
    private bool CompareTarget()
    {
        int correctPositions = 0;
        Transform wordRow = wordRows[rowIndex];

        // Iterate through each letter of the guessed used input.
        for (int i = 0; i < currentGuess.Length; i++)
        {
            Transform letter = wordRow.transform.GetChild(i);

            // If random word contains the current guessed letter.
            if (randomWord.Contains(currentGuess[i]))
            {
                // If letter is at the expected position.
                if (randomWord[i].Equals(currentGuess[i]))
                {
                    letter.GetComponent<Image>().color = Color.green;
                    correctPositions++;
                }
                // If letter is not at the expected position.
                else
                    letter.GetComponent<Image>().color = Color.yellow;
            }
            // If guessed letter is not present on the random word.
            else
                letter.GetComponent<Image>().color = Color.grey;
        }

        // The user wins in case correctPositions match the word length.
        return correctPositions == currentGuess.Length;
    }

    // Called when the user loses the game. Shows UI that displays the answer, and lets the user restart the game.
    private void GameEndLost()
    {
        string lostText = $"You lost :(\n\nThe word was: \"{randomWord}\"";
        gameEndText.text = lostText;
        isGameActive = false;
        gameEndPanel.SetActive(true);
    }

    // Called when the user wins the game. Shows UI that displays the answer, and lets the user restart the game.
    private void GameEndWon()
    {
        string lostText = $"You won :)\n\nThe word was: \"{randomWord}\"";
        gameEndText.text = lostText;
        isGameActive = false;
        gameEndPanel.SetActive(true);
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

    // Gets a random word to be discovered.
    public string GetRandomWord()
    {
        return "GREAT";
    }  

    // Returns the game active state.
    public bool getIsGameActive()
    {
        return isGameActive;
    }

    // Restarts the game by restarting the scene.
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
