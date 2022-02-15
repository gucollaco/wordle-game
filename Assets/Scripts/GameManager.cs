using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Used variables.
    public string randomWord;
    public GameObject gameEndPanel;
    public TextMeshProUGUI gameEndText;
    public GameObject words;
    private List<Transform> wordRows;
    private int characterIndex;
    private int rowIndex;
    private int maxLettersQuantity;
    private bool isGameActive;
    private List<string> possibleWords;
    private List<Button> currentGuessButtons;
    private string currentGuess;

    // Start is called before the first frame update.
    private void Start()
    {
        InitializeLists();
        randomWord = GetRandomWord();
        ResetVariables();
    }

    // Initializes the used lists.
    private void InitializeLists()
    {
        currentGuessButtons = new List<Button>();
        InitializePossibleWords();
        InitializeWordRows();
    }


    // Initializes the possible words list.
    private void InitializePossibleWords()
    {
        possibleWords = new List<string>();
        string path = "Assets/Resources/wordsList.txt";
        ReadWords(path, possibleWords);
    }

    // Initializes the word rows list.
    private void InitializeWordRows()
    {
        wordRows = new List<Transform>();
        foreach (Transform child in words.transform)
            wordRows.Add(child);
    }

    // Resets some variables once the game begins/restarts.
    private void ResetVariables()
    {
        characterIndex = 0;
        rowIndex = 0;
        maxLettersQuantity = 5;
        currentGuess = string.Empty;
        isGameActive = true;
    }

    // Displays the selected character key at the "guess word" letter box.
    public void DisplayLetter(string letterValue, Button button)
    {
        if (characterIndex < maxLettersQuantity)
        {
            Transform wordRow = wordRows[rowIndex];
            Transform letter = wordRow.transform.GetChild(characterIndex);
            TextMeshProUGUI letterText = letter.GetChild(0).GetComponent<TextMeshProUGUI>();
            letterText.text = letterValue;
            
            currentGuess += letterValue;
            currentGuessButtons.Add(button);

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
            currentGuessButtons.RemoveAt(currentGuessButtons.Count - 1);
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
            currentGuessButtons.Clear();

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
            // Getting reference to the images, which will have their color changed.
            Image keyboardButtonImage = currentGuessButtons[i].GetComponent<Image>();
            Transform letter = wordRow.transform.GetChild(i);
            Image letterImage = letter.GetComponent<Image>();

            // If random word contains the current guessed letter.
            if (randomWord.Contains(currentGuess[i]))
            {
                // If letter is at the expected position.
                if (randomWord[i].Equals(currentGuess[i]))
                {
                    // Setting both keyboard and letter box as green.
                    letterImage.color = Color.green;
                    keyboardButtonImage.color = Color.green;

                    correctPositions++;
                }
                // If letter is not at the expected position.
                else
                {
                    // Setting letter box as yellow.
                    letterImage.color = Color.yellow;

                    // Only sets keyboard as yellow in case it has not been green already.
                    if (keyboardButtonImage.color != Color.green)
                        keyboardButtonImage.color = Color.yellow;
                }
            }
            // If guessed letter is not present on the random word.
            else
            {
                letterImage.color = Color.grey;
                keyboardButtonImage.color = Color.grey;
            }
        }

        // The user wins in case correctPositions match the word length.
        return correctPositions == currentGuess.Length;
    }

    // Gets the words from a file, and adds into a list.
    private void ReadWords(string path, List<string> list)
    {
        // Read the text from the file.
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadToEnd();

        // Separate them for each ',' character
        string[] words = text.Split(',');

        // Add words into the list
        foreach (string word in words)
            list.Add(word);

        // Close the reader.
        reader.Close();
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
        return possibleWords[Random.Range(0, possibleWords.Count)];
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
