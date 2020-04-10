using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class NameChanger : MonoBehaviour {
    public KMBombModule module;
    public KMBombInfo bomb;
    public KMAudio audio;
    public KMSelectable[] buttons;
    public TextMesh netext;

    private bool solved = false;
    private int moduleId;
    private static int counter = 1;
    private int letter = 0;
    private int chosenLetter = 0;
    private int chosenWord = 0;
    private int word;
    private MeshRenderer button;
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
    void Awake()
    {
        moduleId = counter++;
        int a = 0;
        int b = 1;
        int c = 2;
        int d = 3;
        int e = 4;
        buttons[a].OnInteract += delegate
        {
            leftPress(); return false;
        };
        buttons[b].OnInteract += delegate
        {
            rightPress(); return false;
        };
        buttons[c].OnInteract += delegate
        {
            something(letter); return false;
        };
        buttons[d].OnInteract += delegate
        {
            upPress(); return false;
        };
        buttons[e].OnInteract += delegate
        {
            downPress(); return false;
        };
    } 
	// Use this for initialization
    void wordGen()
    {
        word = Rnd.Range(0, words.Length);
        GenedWord = words[word];
        Debug.LogFormat("[Name Changer {0}] Word chosen: {1}, which is at position {2}", moduleId, GenedWord, word+1);
        
    }
    void letterGen()
    {
        letter = Rnd.Range(0, 10);
        Debug.LogFormat("[Name Changer {0}] Letter chosen: {1}", moduleId, GenedWord.ElementAt(letter));
        netext.text = "" + GenedWord.ElementAt(letter);
    }
    void newWord()
    {
        GenedWord = words[word];
        netext.text = "" + GenedWord.ElementAt(letter);
    }
    void Start () {
        module.OnActivate += activation;
	}
	void activation()
    {
        wordGen();
        letterGen();
        choseLetter();
        choseWord();
    }
    void changeLetter()
    {
        netext.text = "" + GenedWord.ElementAt(letter);
    }
    void leftPress()
    {
        letter--;
        outOfBoundsLetter();
        changeLetter();
    }
    void rightPress()
    {
        letter++;
        outOfBoundsLetter();
        changeLetter();
    }
    void upPress()
    {
        word--;
        outOfBoundsWord();
        newWord();
        choseLetter();
    }
    void downPress()
    {
        word++;
        outOfBoundsWord();
        newWord();
        choseLetter();
    }
    // Update is called once per frame
    void something(int let)
    {
        char s = GenedWord.ElementAt(chosenLetter);
        string t = words.ElementAt(chosenWord);
        Debug.LogFormat("[Name Changer #{0}] Letter deleted: {1}", moduleId, s);
        if (s == netext.text.ElementAt(0) && t == GenedWord)
        {
            correct();
            netext.text = "Good Job!";
            netext.transform.Translate(x: 0.04f, y: 0.04f, z: 0);
            netext.fontSize = 65;
            buttons[0].transform.Translate(x: 0, y: (float)-0.01, z: 0);
            buttons[1].transform.Translate(x: 0, y: (float)-0.01, z: 0);
            buttons[2].transform.Translate(x: 0, y: (float)-0.01, z: 0);
            buttons[2].GetComponentInChildren<TextMesh>().text = "";
            buttons[3].transform.Translate(x: 0, y: (float)-0.01, z: 0);
            buttons[4].transform.Translate(x: 0, y: (float)-0.01, z: 0);
        }
        else
        {
            incorrect();
        }
    }
    void correct()
    {
        solved = true;
        module.HandlePass();
        Debug.LogFormat("[Name Changer #{0}] The letter deleted is correct!", moduleId);
    }
    void incorrect()
    {
        module.HandleStrike();
        Debug.LogFormat("[Name Changer #{0}] The letter deleted is incorrect!", moduleId);
    }
    void choseLetter()
    {
        chosenLetter = 0;
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == GenedWord)
            {
                chosenLetter = i;
                break;
            }
        }
        chosenLetter += bomb.GetIndicators().Count();
        chosenLetter %= 10;
        chosenLetter = (chosenLetter == 0) ? 9 : chosenLetter;
        if (GenedWord != words.ElementAt(chosenWord))
        {
            Debug.LogFormat("[Name Changer #{0} Wrong word at position {1}!", moduleId, word+1);
        }
        else
        {
            Debug.LogFormat("[Name Changer #{0}] The letter to choose is {1}! Which is position {2}", moduleId, GenedWord[chosenLetter], chosenLetter + 1);
        }
    }
    void choseWord()
    {
        chosenWord = 0;
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == GenedWord)
            {
                chosenWord = i;
                break;
            }
        }
        chosenWord += bomb.GetPortCount();
        chosenWord %= 24;
        chosenWord = (chosenWord == 0) ? 10 : chosenWord;
        Debug.LogFormat("[Name Changer #{0}] The word to choose is {1}! Which is position {2}", moduleId, words[chosenWord], chosenWord + 1);
    }
    void outOfBoundsLetter()
    {
        if (letter < 0)
        {
            letter++;
        }
        else if (letter > 9)
        {
            letter--;
        }
    }
    void outOfBoundsWord()
    {
        if (word < 0)
        {
            word++;
        }
        else if (word > words.Length)
        {
            word--;
        }
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} l # [Presses the left button # of times.] | !{0} r # [Presses the right button # of times.] | !{0} u # [Presses the up button # of times] | !{0} d # [Presses the left button # of times] | !{0} submit [Presses the submit button.]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        yield return null;
        if (parameters[0] == "L" || parameters[0] == "l")
        {
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                leftPress();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "R" || parameters[0] == "r")
        {
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                rightPress();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "U" || parameters[0] == "u")
        {
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                upPress();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "D" || parameters[0] == "d")
        {
            for (int i = 0; i < int.Parse(parameters[1]); i++)
            {
                downPress();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else if (parameters[0] == "submit")
        {
            something(letter);
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        netext.text = "" + GenedWord.ElementAt(chosenLetter);
        yield return new WaitForSeconds(0.5f);
        something(netext.text.ElementAt(0));
        yield return null;
    }
}
