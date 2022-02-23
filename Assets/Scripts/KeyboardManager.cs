using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    // Used variables.
    public GameObject keyboard;
    public GameManager gameManager;
    public Button backspaceButton;
    public Button confirmButton;
    public Button solveButton;
    private string[] characters;
    private Dictionary<string, Button> characterButtons;

    // Start is called before the first frame update.
    private void Start()
    {
        InitializeCharacterList();

        characterButtons = new Dictionary<string, Button>();

        // Go through each keyboard row.
        for(int i = 0; i < keyboard.transform.childCount; i++)
        {
            Transform row = keyboard.transform.GetChild(i);

            // Backspace and enter buttons.
            if (i == 0)
            {
                backspaceButton.onClick.AddListener(BackspaceKeyClick);
                confirmButton.onClick.AddListener(ConfirmKeyClick);
            }
            // Character buttons.
            else
            {
                // Setting up the text content for each keyboard character button.
                for (int j = 0; j < row.transform.childCount; j++)
                {
                    Transform letter = row.transform.GetChild(j);
                    TextMeshProUGUI letterText = letter.GetChild(0).GetComponent<TextMeshProUGUI>();
                    int characterRowIndex = i - 1;
                    string text = characters[characterRowIndex][j].ToString();
                    letterText.text = text;
                    Button letterButton = letter.GetComponent<Button>();
                    letterButton.onClick.AddListener(() => CharacterKeyClick(text, letterButton));
                    characterButtons.Add(text, letterButton);
                }
            }
        }
    }

    // Initializing the characters string array.
    private void InitializeCharacterList()
    {
        characters = new string[3];
        
        characters[0] = "QWERTYUIOP";
        characters[1] = "ASDFGHJKL";
        characters[2] = "ZXCVBNM";
    }

    // Used to define what a character key click should do.
    private void CharacterKeyClick(string letter, Button buttonElement)
    {
        gameManager.DisplayLetter(letter, buttonElement);
        
        // Backspace button gets enabled when we have at least one character.
        backspaceButton.interactable = true;

        // Automatic solve button should be disabled when we have at least one character.
        solveButton.interactable = false;

        // Confirm button gets enabled when all word letters are filled.
        if (gameManager.GetCharacterIndex() == gameManager.GetMaxLettersQuantity())
            confirmButton.interactable = true;
    }

    // Used to define what a backspace key click should do.
    private void BackspaceKeyClick()
    {
        gameManager.UndoLastCharacter();

        // Confirm button gets disabled when not all required characters are informed.
        confirmButton.interactable = false;

        // Backspace button gets disabled when there are no letters informed.
        if (gameManager.GetCharacterIndex() == 0)
        {
            backspaceButton.interactable = false;

            // If we are still on the first word row, we enable the solve button.
            if (gameManager.GetRowIndex() == 0)
                solveButton.interactable = true;
        }
    }

    // Used to define what a confirm key click should do.
    private void ConfirmKeyClick()
    {
        bool isGuessValid = gameManager.ConfirmRow();

        // Upon confirming, disables both backspace and confirm buttons, for the next row reset. When guess is invalid, backspace button will not be disabled.
        if (isGuessValid)
            backspaceButton.interactable = false;
        confirmButton.interactable = false;

        // If game finished, disables all character buttons.
        if (!gameManager.getIsGameActive())
        {
            foreach (Button button in characterButtons.Values)
                button.interactable = false;
        }
    }

    // public void AutomaticSolve()
    // {
    //     for (int i = 0; i < gameManager.wordRows.Count; i++)
    //     {
    //         string guessedWord = gameManager.NextWordGuess();
    //         foreach (char letter in guessedWord)
    //         {
    //             Button letterButton = characterButtons[letter.ToString()];
    //             CharacterKeyClick(letter.ToString(), letterButton);
    //         }
    //         ConfirmKeyClick();

    //         if (!gameManager.getIsGameActive())
    //             break;
    //     }
    // }

    public void AutomaticSolve()
    {
        DisableButtons();
        StartCoroutine(SolvePuzzle());
    }

    // Method to disable the buttons.
    private void DisableButtons()
    {
        foreach (Button button in characterButtons.Values)
            button.interactable = false;
        backspaceButton.interactable = false;
        confirmButton.interactable = false;
        solveButton.interactable = false;
    }

    // Method to solve the puzzle automatically.
    public IEnumerator SolvePuzzle()
    {
        for (int i = 0; i < gameManager.wordRows.Count; i++)
        {
            string guessedWord = gameManager.NextWordGuess();
            foreach (char letter in guessedWord)
            {
                Button letterButton = characterButtons[letter.ToString()];
                CharacterKeyClick(letter.ToString(), letterButton);
            }
            ConfirmKeyClick();

            yield return new WaitForSeconds(1f);

            if (!gameManager.getIsGameActive())
                break;
        }

        yield return null;
    }
}
