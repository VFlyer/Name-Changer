using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

// Rewrite courtsey of VFlyer

public class NameChanger : MonoBehaviour {
    public KMBombModule module;
    public KMBombInfo bomb;
    public KMAudio MAudio;
    public KMSelectable[] buttons;
    public KMSelectable submitBtn;
    public TextMesh netext;

    private bool solved = false;
    private int moduleId;
    private static int counter = 1;
    private int currentLetter, currentWord, startingWord, startingLetter, chosenLetter = 0, chosenWord = 0;
    private bool auto;
    private string[] words =
    {
        "STEREOTYPE",
        "MICROPHONE",
        "PRESIDENCY",
        "ACCEPTANCE",
        "PRODUCTIVE",
        "ARTICULATE",
        "CONSTRAINT",
        "INVESTMENT",
        "CONCLUSION",
        "DISABILITY",
        "INHABITANT",
        "PSYCHOLOGY",
        "ACCOUNTANT",
        "DEFICIENCY",
        "ATTRACTIVE",
        "BASKETBALL",
        "EXPRESSION",
        "MASTERMIND",
        "ACCEPTABLE",
        "THOUGHTFUL",
        "TOURNAMENT",
        "POPULATION",
        "ASSUMPTION",
        "LITERATURE"
    };
    private string GenedWord;
    private string InitialWord;
    void Awake()
    {
        
    } 
    // Use this for initialization
    void Start () {
        moduleId = counter++;

        for (int x = 0; x < buttons.Length; x++)
        {
            int y = x;
            buttons[x].OnInteract += delegate {
                MAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[y].transform);
                if (!solved)
                {
                    switch (y)
                    {
                        case 0:
                            LeftPress();
                            break;
                        case 1:
                            RightPress();
                            break;
                        case 2:
                            UpPress();
                            break;
                        case 3:
                            DownPress();
                            break;
                    }
                }
                return false;
            };

        }
        // Commented out due to inefficiency
        /*
        buttons[0].OnInteract += delegate
        {
            MAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[0].transform);
            if (!solved)
                leftPress();
            return false;
        };
        buttons[1].OnInteract += delegate
        {
            MAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[1].transform);
            if (!solved)
                rightPress();
            return false;
        };
        buttons[2].OnInteract += delegate
        {
            MAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[2].transform);
            if (!solved)
                upPress();
            return false;
        };
        buttons[3].OnInteract += delegate
        {
            MAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[3].transform);
            if (!solved)
                downPress();
            return false;
        };
        */
        submitBtn.OnInteract += delegate {
            MAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitBtn.transform);
            if (!solved)
                HandleSubmit();
            return false;
        };

        module.OnActivate += Activation;
	}
    void GenerateInitialSet()
    {
        startingWord = Rnd.Range(0, words.Length); // Generate a starting word index from 0,23 inclusive

        currentWord = startingWord;
        InitialWord = words[currentWord];
        Debug.LogFormat("[Name Changer #{0}] Starting on the word: {1}, at position {2} on the table in reading order.", moduleId, InitialWord, currentWord + 1);

        startingLetter = Rnd.Range(0, 10);

        currentLetter = startingLetter;
        Debug.LogFormat("[Name Changer #{0}] Starting letter from the given word: {1} (the {2} letter)", moduleId, InitialWord.ElementAt(currentLetter), new[] { 0, 1, 2 }.Contains(startingLetter) ? new[] { "1st", "2nd", "3rd" }[startingLetter] : ((startingLetter + 1).ToString() + "th"));
        netext.text = "" + InitialWord.ElementAtOrDefault(currentLetter);
    }

    void GenerateSolution()
    {
        chosenLetter = startingWord + bomb.GetIndicators().Count();
        chosenLetter %= 10;
        chosenLetter = (chosenLetter == 0) ? 1 : chosenLetter;
        Debug.LogFormat("[Name Changer #{0}] The letter to choose to delete is the {1} letter.", moduleId, new[] { 0, 1, 2 }.Contains(chosenLetter) ? new[] { "1st", "2nd", "3rd" }[chosenLetter] : ((chosenLetter + 1).ToString() + "th"));

        chosenWord = startingWord + bomb.GetPortCount();
        chosenWord %= 24;
        chosenWord = (chosenWord == 0) ? 1 : chosenWord;
        Debug.LogFormat("[Name Changer #{0}] The target word to choose is {1}! Which is in position {2} on the table in reading order.", moduleId, words[chosenWord], chosenWord + 1);

        

    }

	void Activation()
    {
        GenerateInitialSet();
        GenerateSolution();
        
    }
    void ChangeLetter()
    {
        if (GenedWord == null)
        {
            netext.text = "" + InitialWord.ElementAt(currentLetter);
        }
        else
        {
            netext.text = "" + GenedWord.ElementAt(currentLetter);
        }
    }
    void NewWord()
    {
        GenedWord = words[currentWord];
        netext.text = "" + GenedWord.ElementAt(currentLetter);
    }
    void LeftPress()
    {
        currentLetter--;
        OutOfBoundsLetter();
        ChangeLetter();
    }
    void RightPress()
    {
        currentLetter++;
        OutOfBoundsLetter();
        ChangeLetter();
    }
    void UpPress()
    {
        currentWord--;
        OutOfBoundsWord();
        NewWord();
    }
    void DownPress()
    {
        currentWord++;
        OutOfBoundsWord();
        NewWord();
    }
    // Update is called once per frame
    void HandleSubmit()
    {
        if (auto)
        {
            Correct();
        }
        else
        {
            string highlightedWord = words.ElementAtOrDefault(currentWord);

            if (highlightedWord == null) return;

            Debug.LogFormat("[Name Changer #{0}] Attempting to submit on the word: {1}", moduleId, highlightedWord);

            char s = highlightedWord.ElementAtOrDefault(currentLetter);
            
            Debug.LogFormat("[Name Changer #{0}] Attempting to delete the following letter from the word: {1} ({2} letter from the current word.)", moduleId, s, new[] { 0, 1, 2 }.Contains(currentLetter) ? new[] { "1st", "2nd", "3rd" }[currentLetter] : ((currentLetter + 1).ToString() + "th"));
            if (chosenLetter == currentLetter && chosenWord == currentWord)
            {
                Correct();
            }
            else
            {
                Incorrect();
            }
        }
    }
    void Correct()
    {
        solved = true;
        module.HandlePass();
        Debug.LogFormat("[Name Changer #{0}] The letter deleted is correct!", moduleId);
        netext.text = auto ? "Forced\nSolved" : "Good Job!";
        netext.transform.localPosition= new Vector3(0, 0.0151f, 0);
        netext.fontSize = 65;
        buttons[0].transform.Translate(x: 0, y: (float)-0.01, z: 0);
        buttons[1].transform.Translate(x: 0, y: (float)-0.01, z: 0);
        buttons[2].transform.Translate(x: 0, y: (float)-0.01, z: 0);
        buttons[3].transform.Translate(x: 0, y: (float)-0.01, z: 0);
        submitBtn.transform.Translate(x: 0, y: (float)-0.01, z: 0);
    }
    void Incorrect()
    {
        Debug.LogFormat("[Name Changer #{0}] The letter deleted is incorrect!", moduleId);
        module.HandleStrike();
    }
    void OutOfBoundsLetter()
    {
        if (currentLetter < 0)
        {
            currentLetter = 0;
        }
        else if (currentLetter > 9)
        {
            currentLetter = 9;
        }
    }
    void OutOfBoundsWord()
    {
        if (currentWord < 0)
        {
            currentWord = 0;
        }
        else if (currentWord > words.Length - 1)
        {
            currentWord = words.Length - 1;
        }
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = " \"!{0} left/right/up/down #\" to move left, right, down or up within the given letters, \"!{0} submit\" to submit the selected letter. \"!{0} reset/restart\" to go back to the initial letter and word. Command is case insensitive.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        // Old TP Handling, case sensistive against "!# L L", "!# L R", etc. Breaks with "!# l r", "!# u d", etc...
        /*
        command = command.ToLower();
        
        string[] parameters = command.Split(' ');

        if (parameters.Length < 2)
        {
            yield return null;
            yield return "sendtochaterror Please specify what number you would like to press!";
            yield break;
        }
        else if (parameters.Length > 2)
        {
            yield return null;
            yield return "sendtochaterror Too many arguements!";
            yield break;
        }
        if (parameters[0] == "left" || parameters[0] == "l")
        {
            yield return null;
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                buttons[0].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "right" || parameters[0] == "r")
        {
            yield return null;
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                buttons[1].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "up" || parameters[0] == "u")
        {
            yield return null;
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                buttons[3].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "down" || parameters[0] == "d")
        {
            yield return null;
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                buttons[4].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "submit")
        {
            yield return null;
            buttons[2].OnInteract();
        }
        */
        if (Regex.IsMatch(command, @"^(l(eft)?|r(ight)?|u(p)?|d(own)?)\s\d+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            string[] splittedParts = command.ToLowerInvariant().Split();
            Dictionary<char, KMSelectable> directionsToButtons = new Dictionary<char, KMSelectable>() {
                {'l',buttons[0] },
                {'r',buttons[1] },
                {'u',buttons[2] },
                {'d',buttons[3] },
            };
            
            int timesToPress;
            if (!directionsToButtons.ContainsKey(splittedParts[0].ElementAtOrDefault(0)))
            {
                yield return string.Format("sendtochaterror There are no buttons to press in conjunction with a specified direction.");
                yield break;
            }
            KMSelectable curSelected = directionsToButtons[splittedParts[0][0]];
            if (!int.TryParse(splittedParts[1], out timesToPress) || timesToPress <= 0 || timesToPress > 24)
            {
                yield return string.Format("sendtochaterror The module does not want to accept \"{0}\" as a valid number of times to press.",splittedParts[1]);
                yield break;
            }
            for (int x = 0; x < timesToPress; x++)
            {
                yield return null;
                curSelected.OnInteract();
            }
        }
        else if (Regex.IsMatch(command, @"^submit$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[2].OnInteract();
        }
        else if (Regex.IsMatch(command, @"^res(et|tart)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            currentLetter = startingLetter;
            currentWord = startingWord;
            NewWord();
            ChangeLetter();
            yield return "sendtochat I have now resetted Name Changer (#{1}) to the initial letter and word.";
            yield break;

        }
        else
        {
            yield return string.Format("sendtochaterror I do not know of a command {0} in the module, check your command for typos.",command);
            yield break;
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        auto = true;
        /*
        currentWord = (chosenWord == 0) ? 1 : chosenWord;
        Debug.LogFormat("<Name Changer {0}> Debug: The word is at position: {1}", moduleId, currentWord);
        currentLetter = (chosenLetter == 0) ? 1 : chosenLetter;
        */
        while (currentWord != chosenWord)
        {
            yield return null;
            if (currentWord > chosenWord)
                buttons[2].OnInteract();
            else
                buttons[3].OnInteract();
        }
        while (currentLetter != chosenLetter)
        {
            yield return null;
            if (currentLetter > chosenLetter)
                buttons[0].OnInteract();
            else
                buttons[1].OnInteract();
        }
        submitBtn.OnInteract();
    }
}
