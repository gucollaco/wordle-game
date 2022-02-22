using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Used variables.
    public string randomWord;
    public GameObject invalidWordPanel;
    public TextMeshProUGUI invalidWordText;
    public GameObject gameEndPanel;
    public TextMeshProUGUI gameEndText;
    public GameObject words;
    public List<Transform> wordRows;
    private int characterIndex;
    private int rowIndex;
    private int maxLettersQuantity;
    private bool isGameActive;
    private List<string> possibleGuesses;
    private List<string> possibleAnswers;
    private List<Button> currentGuessButtons;
    private string currentGuess;

    private List<string> possibleGuessesClone;
    private List<string> possibleAnswersUpdated;
    private List<string> guessesToConsider;
    private float mininumAverage;
    private string chosenGuess;

    // Start is called before the first frame update.
    private void Start()
    {
        InitializeLists();
        randomWord = GetRandomWord();
        ResetVariables();

        possibleGuessesClone = new List<string>(possibleGuesses);
        possibleAnswersUpdated = new List<string>(possibleAnswers);
        guessesToConsider = new List<string>();
        
        mininumAverage = 1000000;
        chosenGuess = "";
    }

    // Initializes the used lists.
    private void InitializeLists()
    {
        currentGuessButtons = new List<Button>();
        InitializePossibleAnswers();
        InitializePossibleGuesses();
        InitializeWordRows();
    }

    // Initializes the possible answers list.
    private void InitializePossibleAnswers()
    {
        possibleAnswers = new List<string>();
        string path = "Assets/Resources/answersList.txt";
        ReadWords(path, possibleAnswers);
    }

    // Initializes the possible guesses list, adding the possibleAnswers elements into it.
    private void InitializePossibleGuesses()
    {
        possibleGuesses = new List<string>();
        string path = "Assets/Resources/wordsList.txt";
        ReadWords(path, possibleGuesses);
        possibleGuesses = possibleGuesses.Concat(possibleAnswers).ToList();
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

    // Confirms the current row, and goes to the next one. Returns false when guess is invalid, and true when guess is valid.
    public bool ConfirmRow()
    {
        // Checks if the guess is a possible guess.
        if (!possibleGuesses.Contains(currentGuess))
        {
            invalidWordText.text = "Invalid word :(";
            invalidWordPanel.SetActive(true);
            return false;
        }
        // In this case, we can compare the guessed word with the hidden word.
        else
        {
            bool hasWon = CompareTarget();

            characterIndex = 0;
            rowIndex++;
            currentGuess = string.Empty;
            currentGuessButtons.Clear();

            // Checks if user won.
            if (hasWon)
                GameEndWon();
            // Checks if user lost.
            else if (rowIndex == wordRows.Count)
                GameEndLost();

            return true;
        }
    }

    // Method to close the popup (dialog panel).
    public void ClosePopup()
    {
        invalidWordPanel.SetActive(false);
        invalidWordText.text = string.Empty;
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
                    // Checking how many of this character is present on the random word.
                    char currentLetter = currentGuess[i];
                    int letterOccurences = randomWord.Count(letter => (letter == currentLetter));

                    // Checking how many of this character has already been checked.
                    int occurencesConsidered = 0;
                    for (int j = 0; j < i; j++)
                        if (currentGuess[j] == currentLetter)
                            occurencesConsidered++;
                    
                    // If there are still occurences to consider, color it.
                    if (occurencesConsidered < letterOccurences)
                    {
                        // Setting letter box as yellow.
                        letterImage.color = Color.yellow;

                        // Only sets keyboard as yellow in case it has not been green already.
                        if (keyboardButtonImage.color != Color.green)
                            keyboardButtonImage.color = Color.yellow;
                    }
                    // Else, paint it grey.
                    else
                    {
                        letterImage.color = Color.grey;
                    }
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
            list.Add(word.ToUpper());

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
        return possibleAnswers[Random.Range(0, possibleAnswers.Count)];
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

    // Method to solve wordle by itself.
    public string NextWordGuess()
    {
        mininumAverage = 1000000;
        chosenGuess = "";
        Dictionary<string, List<string>> evaluationToWords = new Dictionary<string, List<string>>();

        // If not the first iteration, consider all possible guesses
        if (rowIndex > 0)
            guessesToConsider = new List<string>(possibleGuesses);
        // If it is the first iteration, let's consider some reduced possible guess.
        else
        {
            guessesToConsider = new List<string>();
            string[] someGoodWords = { "ARISE", "CRANE", "TEARS", "CARET" };
            foreach (string word in someGoodWords)
                guessesToConsider.Add(word);
        }

        foreach (string guessToConsider in guessesToConsider)
        {
            Dictionary<string, List<string>> temporaryEvaluationToWords = new Dictionary<string, List<string>>();

            foreach (string possibleAnswer in possibleAnswersUpdated)
            {
                string evaluation = GetEvaluation(possibleAnswer, guessToConsider);

                // If we already have a word with this evaluation key, we add this value to the list
                if (temporaryEvaluationToWords.ContainsKey(evaluation))
                {
                    temporaryEvaluationToWords[evaluation].Add(possibleAnswer);
                }
                // If this is the first word with this evaluation.
                else
                {
                    List<string> newKeyList = new List<string>();
                    newKeyList.Add(possibleAnswer);
                    temporaryEvaluationToWords.Add(evaluation, newKeyList);
                }
            }

            // Getting the average of the possible words count, when using this guess, compared to the possible answers.
            int totalPossibilitiesSum = 0;
            foreach (List<string> value in temporaryEvaluationToWords.Values)
                totalPossibilitiesSum += value.Count;
            float averageRemainingWords = totalPossibilitiesSum / temporaryEvaluationToWords.Count;
            
            if (averageRemainingWords < mininumAverage)
            {
                mininumAverage = averageRemainingWords;
                chosenGuess = guessToConsider;

                evaluationToWords = temporaryEvaluationToWords;
            }
        }

        possibleAnswersUpdated = evaluationToWords[GetEvaluation(randomWord, chosenGuess)];

        if (possibleAnswersUpdated.Count == 1)
            return possibleAnswersUpdated[0];
        return chosenGuess;
    }
    

    // Method to solve wordle by itself.
    public void Solve()
    {
        List<string> possibleGuessesClone = new List<string>(possibleGuesses);
        List<string> possibleAnswersUpdated = new List<string>(possibleAnswers);
        List<string> guessesToConsider = new List<string>();
        
        for (int i = 0; i < wordRows.Count; i++)
        {
            float mininumAverage = 1000000;
            string chosenGuess = "";
            Dictionary<string, List<string>> evaluationToWords = new Dictionary<string, List<string>>();

            // If not the first iteration, consider all possible guesses
            if (i > 0)
            {
                guessesToConsider = possibleGuessesClone;
            }
            // If it is the first iteration, let's consider a fixed guess.
            else
            {
                guessesToConsider = new List<string>();
                guessesToConsider.Add("ARISE");
            }

            foreach (string guessToConsider in guessesToConsider)
            {
                Dictionary<string, List<string>> temporaryEvaluationToWords = new Dictionary<string, List<string>>();

                foreach (string possibleAnswer in possibleAnswersUpdated)
                {
                    string evaluation = GetEvaluation(possibleAnswer, guessToConsider);

                    // If we already have a word with this evaluation key, we add this value to the list
                    if (temporaryEvaluationToWords.ContainsKey(evaluation))
                    {
                        temporaryEvaluationToWords[evaluation].Add(possibleAnswer);
                    }
                    // If this is the first word with this evaluation.
                    else
                    {
                        List<string> newKeyList = new List<string>();
                        newKeyList.Add(possibleAnswer);
                        temporaryEvaluationToWords.Add(evaluation, newKeyList);
                    }
                }

                // Getting the average of the possible words count, when using this guess, compared to the possible answers.
                int totalPossibilitiesSum = 0;
                foreach (List<string> value in temporaryEvaluationToWords.Values)
                    totalPossibilitiesSum += value.Count;
                float averageRemainingWords = totalPossibilitiesSum / temporaryEvaluationToWords.Count;
                
                // int max = 0;
                // foreach (List<string> value in temporaryEvaluationToWords.Values)
                //     if (max < value.Count)
                //         max = value.Count;
                // float averageRemainingWords = max;
                
                if (averageRemainingWords < mininumAverage)
                {
                    mininumAverage = averageRemainingWords;
                    chosenGuess = guessToConsider;

                    evaluationToWords = temporaryEvaluationToWords;
                }
            }

            possibleAnswersUpdated = evaluationToWords[GetEvaluation(randomWord, chosenGuess)];

            if (possibleAnswersUpdated.Count == 1)
                return ;
            Debug.Log("Chosen guess");
            Debug.Log(chosenGuess);
            Debug.Log(GetEvaluation(randomWord, chosenGuess));
            Debug.Log(possibleAnswersUpdated[0]);
        }
    }

    // Evaluates two words
    private string GetEvaluation(string answer, string guess)
    {
        int[] evaluation = { 0, 0, 0, 0, 0 };

        for (int i = 0; i < answer.Length; i++)
        {
            if (answer[i] == guess[i])
                evaluation[i] = 2;
        }

        for (int i = 0; i < answer.Length; i++)
        {
            if (answer.Contains(guess[i]) && evaluation[i] == 0)
                evaluation[i] = 1;
        }

        return string.Join(string.Empty, evaluation);
    }
}
