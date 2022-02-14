using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    // Used variables.
    private GameObject keyboard;
    private string[] characters;
    private GameManager gameManager;
    public Button backspaceButton;
    public Button confirmButton;

    // Start is called before the first frame update.
    private void Start()
    {
        InitializeCharacterList();

        keyboard = GameObject.Find("Keyboard");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Go through each keyboard row.
        for(int i = 0; i < keyboard.transform.childCount; i++)
        {
            Transform row = keyboard.transform.GetChild(i);

            // Backspace and enter buttons.
            if (i == 0)
            {
                Transform backspace = row.transform.GetChild(0);
                backspaceButton = backspace.GetComponent<Button>();
                backspaceButton.onClick.AddListener(BackspaceKeyClick);

                Transform confirm = row.transform.GetChild(1);
                confirmButton = confirm.GetComponent<Button>();
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
                    letterButton.onClick.AddListener(() => CharacterKeyClick(text));
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
    private void CharacterKeyClick(string letter)
    {
        gameManager.DisplayLetter(letter);
        
        // Backspace button gets enabled when we have at least one character.
        backspaceButton.interactable = true;

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
            backspaceButton.interactable = false;
    }

    // Used to define what a confirm key click should do.
    private void ConfirmKeyClick()
    {
        gameManager.NextRow();

        // Upon confirming, disables both backspace and confirm buttons, for the next row reset.
        backspaceButton.interactable = false;
        confirmButton.interactable = false;
    }
}
