using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    private GameObject keyboard;
    private string[] characters;
    private GameManager gameManager;

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
                Button backspaceButton = backspace.GetComponent<Button>();
                backspaceButton.onClick.AddListener(BackspaceKeyClick);

                Transform confirm = row.transform.GetChild(1);
                Button confirmButton = confirm.GetComponent<Button>();
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

    private void CharacterKeyClick(string letter)
    {
        gameManager.DisplayLetter(letter);
    }

    private void BackspaceKeyClick()
    {
        Debug.Log("Backspace clicked");
    }

    private void ConfirmKeyClick()
    {
        Debug.Log("Confirm clicked");
    }
}
