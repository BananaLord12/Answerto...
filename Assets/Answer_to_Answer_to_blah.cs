using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using rnd = UnityEngine.Random;
using KModkit;

public class Answer_to_Answer_to_blah : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMSelectable[] NumberPad;
    public KMSelectable Clear;
    public KMSelectable Submit;
    public TextMesh Display;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved = false;
    int index = 0;
    double Index = 0;
    double dbat;
    double aabat;
    int currentAnswer = 0;
    int Solution = 0;
    int strikes;
    int recentStrikes;
    char firstletter;
    char lastletter;
    string allletters;
    double solution = 0;
    bool ispurgatoryon = false;
    List<string> solvedmodulesnames;
    int minutes;
    string light_indicator;
    public MeshRenderer lightrenderer;
    public MeshRenderer[] buttonrendered;


    private int[] MeaningfulNumbers = new int[30] { 42, 12, 365, 69, 420, 10, 2, 603, 294,
                                                    3141, 6028, 0, 6022, 666, 777, 1337, 200,
                                                    709, 981, 839, 4011, 836, 33, 49, 1200, 1459,
                                                    1523, 1499, 8944, 9091 };
    //public char[] AlphabetLetters = new char[26] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    string AlphabetLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public Material[] LED_Colours;
    private string[] Colour_names = new string[7] { "red","green","blue","cyan","yellow","magenta", "black" };
    private List<int> Usedpositions = new List<int>();
    private string[] colourpositions = new string[6];

    void Awake()
    {
        moduleId = moduleIdCounter++;
        Clear.OnInteract += delegate () { ClearButton(); return false; };
        Submit.OnInteract += delegate () { SubmitButton(); return false; };
        foreach(KMSelectable numberbutton in NumberPad)
        {
            KMSelectable PressedNumber = numberbutton;
            PressedNumber.OnInteract += delegate () { Handler(PressedNumber); return false; };
        }
    }


    // Use this for initialization
    void Start ()
    {
        Display.text = "";
        var random = rnd.Range(0, 7);
        light_indicator = Colour_names[random];
        lightrenderer.material = LED_Colours[random];
        for(int i = 0; i < 6; i++)
        {
            while (Usedpositions.Contains(random))
            {
                random = rnd.Range(0, 7);
            }
            buttonrendered[i].material = LED_Colours[random];
            Usedpositions.Add(random);
            colourpositions[i] = Colour_names[random];
        }
        Debug.LogFormat("[The Answer to ... #{0}] The Colours of the buttons are {1}, {2}, {3}, {4}, {5}, {6}", moduleId, colourpositions[0], colourpositions[1], colourpositions[2], colourpositions[3], colourpositions[4], colourpositions[5]);
        Usedpositions.Clear();
        GetRule();
	}
	
	// Update is called once per frame
	void Update ()
    {
        var minutes = Bomb.GetTime();
        minutes = Convert.ToInt32(minutes / 60);
        solvedmodulesnames = Bomb.GetSolvedModuleNames();
        strikes = Bomb.GetStrikes();
        if(strikes != recentStrikes && !moduleSolved)
        {
            checkStrikes();
            GetRule();
        }
    }

    void checkStrikes()
    {
        recentStrikes = 0;
        if (Bomb.GetStrikes() > 0)
        {
            if (Bomb.GetStrikes() == 1)
            {
                recentStrikes = strikes;
                GetRule();
            }
            else if (Bomb.GetStrikes() == 2)
            {
                recentStrikes = strikes;
                GetRule();
            }
        }
        else
        {
            recentStrikes = 0;
        }
    }

    void GetRule()
    {
        int bateries = Bomb.GetBatteryCount();
        int AAbat = Bomb.GetBatteryCount(Battery.AA);
        int Dbat = Bomb.GetBatteryCount(Battery.D);
        int battholder = Bomb.GetBatteryHolderCount();
        int lit = Bomb.GetOnIndicators().Count();
        int unlit = Bomb.GetOffIndicators().Count();
        int lastnumber = Bomb.GetSerialNumberNumbers().Last();
        int firstnumber = Bomb.GetSerialNumberNumbers().First();
        int numbers = Bomb.GetSerialNumberNumbers().Count();
        int SumNumbers = Bomb.GetSerialNumberNumbers().Sum();
        firstletter = Bomb.GetSerialNumberLetters().First();
        lastletter = Bomb.GetSerialNumberLetters().Last();
        allletters = Bomb.GetSerialNumberLetters().ToString();
        int allmodules = Bomb.GetModuleNames().Count();
        double indicators = Bomb.GetIndicators().Count();
        int porttypes = Bomb.CountUniquePorts();
        int portplates = Bomb.GetPortPlateCount();
        int ports = Bomb.GetPortCount();

        index = (bateries * battholder) + (lit * unlit) + (porttypes * portplates);
        Debug.LogFormat("[The Answer to ... #{0}] The Original Index is {1}", moduleId, index);
        if (light_indicator == "red")
        {
            if(Bomb.IsPortPresent(Port.Serial) && Bomb.IsPortPresent(Port.Parallel))
            {
                index = (index * 89) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is red AND there is a serial and a parallel port present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else if (Bomb.IsPortPresent(Port.Serial))
            {
                index = (index + 6) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is red AND there is a serial port present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else if (Bomb.IsPortPresent(Port.Parallel))
            {
                index = (index - 26) % 30;
                if(index < 0)
                {
                    index += 30;
                }
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is red AND parallel port present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else
            {
                index = (index + 1) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is red AND neither there is a serial or a parallel port present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
        }
        else if (light_indicator == "green")
        {
            if (Bomb.IsIndicatorPresent(Indicator.TRN))
            {
                index = (index + bateries) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is green AND there is a TRN indicator present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else if (Bomb.IsIndicatorPresent(Indicator.NSA))
            {
                if (Bomb.IsIndicatorOn(Indicator.NSA))
                {
                    index = (index + battholder) % 30;
                    Debug.LogFormat("[The Answer to ... #{0}] Light indicator is green AND there is a lit NSA indicator present", moduleId);
                    Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
                }
            }
            else if (Bomb.IsIndicatorPresent(Indicator.CAR))
            {
                if (Bomb.IsIndicatorOff(Indicator.CAR))
                {
                    index = (index + index) % 30;
                    Debug.LogFormat("[The Answer to ... #{0}] Light indicator is green AND there is an unlit CAR indicator present", moduleId);
                    Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
                }
            }
            else
            {
                if(index % 2 == 0)
                {
                    index = index / 2;
                    index = index % 30;
                }
                else
                {
                    index = index % 30;
                }
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is green AND there is no TRN/lit NSA/unlit CAR indicators present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
        }
        else if (light_indicator == "blue")
        {
            if(AAbat > Dbat)
            {
                index = (index + AAbat) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is blue AND there is more AA batteries than D batteries present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else if(Dbat > AAbat)
            {
                index = (index + Dbat) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is blue AND there is more D batteries than AA batteries present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else if(AAbat == Dbat)
            {
                index = (index + (bateries * battholder)) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is blue AND there is an equal amount of AA batteries and D batteries present", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
            else
            {
                index = (index * 2) % 30;
                Debug.LogFormat("[The Answer to ... #{0}] Light indicator is blue AND there is no batteries", moduleId);
                Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
            }
        }
        else if (light_indicator == "cyan")
        {
            index = (index * Dbat) % 30;
            Debug.LogFormat("[The Answer to ... #{0}] Light indicator is cyan AND the index is multiplied by D batteries", moduleId);
            Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
        }
        else if (light_indicator == "magenta")
        {
            index = (index * AAbat) % 30;
            Debug.LogFormat("[The Answer to ... #{0}] Light indicator is magenta AND the index is multiplied by amount of AA batteries", moduleId);
            Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
        }
        else if (light_indicator == "yellow")
        {
            int Indicators = Convert.ToInt32(indicators);
            index = (index * Indicators) % 30;
            Debug.LogFormat("[The Answer to ... #{0}] Light indicator is yellow AND the index is multiplied by amount of indicators", moduleId);
            Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
        }
        else if (light_indicator == "black")
        {
            index = index % 30;
            Debug.LogFormat("[The Answer to ... #{0}] Light indicator is black and the index is moduloed by 30", moduleId);
            Debug.LogFormat("[The Answer to ... #{0}] The Index is {1}", moduleId, index);
        }


        currentAnswer = MeaningfulNumbers[index];
        Debug.LogFormat("[The Answer to ... #{0}] The Rule to be used is {1}", moduleId, currentAnswer);
        switch (index)
        {
            case 0:
                Solution = 42;
                if(colourpositions[3] == "blue")
                {
                     if (recentStrikes == 0)
                     {
                        if (Bomb.IsIndicatorPresent(Indicator.IND))
                        {
                             if (Bomb.IsIndicatorOn(Indicator.IND))
                             {
                              solution = Convert.ToDouble(Solution);
                              Solution = Convert.ToInt32(Math.Pow(Solution, 5));
                              Solution = Solution % 30;
                              Debug.LogFormat("[The Answer to ... #{0}] The Solution with 0 strikes is {1}", moduleId, Solution);
                            }
                        }
                     }
                    else if (recentStrikes == 1)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.FRK))
                        {
                            if (Bomb.IsIndicatorOff(Indicator.FRK))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 9));
                                Solution = Solution % 30;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution with 1 strike is {1}", moduleId, Solution);
                            }
                        }
                    }
                    else if (recentStrikes >= 2)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.BOB))
                        {
                            if (Bomb.IsIndicatorOn(Indicator.BOB))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 5));
                                Solution = Solution % 30;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution with 2 or more strikes is {1}", moduleId, Solution);
                            }
                        }
                    }
                }
                if (colourpositions[1] == "red")
                {
                    if (strikes == 0)
                    {
                        if (lastnumber % 2 == 0)
                        {
                            Solution = lastnumber * numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 0 with 0 strikes AND last digit is even is {1}", moduleId, Solution);
                        }
                        else
                        {
                            Solution -= numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 0 with 0 strikes AND last digit is odd is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        Solution = firstnumber * lastnumber;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 0 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution += SumNumbers;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 0 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 0 is {1}", moduleId, Solution);
                break;
            case 1:
                var month = DateTime.Now.Month;
                Solution = 12 + month;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 1 is {1}", moduleId, Solution);
                break;
            case 2:
                var month1 = DateTime.Now.Month;
                var Day = DateTime.Now.Day;
                Solution = 365 + month1 + Day;
                var Modulesid = Bomb.GetModuleNames();
                foreach (var module in Modulesid)
                {
                    if (module == "Calendar")
                    {
                        Solution = (Solution * 2) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 2 and Calendar present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 2 is {1}", moduleId, Solution);
                break;
            case 3:
                var num = Bomb.GetSerialNumberNumbers().First();
                var num1 = Bomb.GetSerialNumberNumbers().Last();
                Solution = 69 + (num1 * num);
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 3 is {1}", moduleId, Solution);
                break;
            case 4:
                Solution += 420 + 42;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 4 is {1}", moduleId, Solution);
                break;
            case 5:
                var Num = Bomb.GetSerialNumberNumbers().Sum();
                Solution = 10 + Num - 9;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 5 is {1}", moduleId, Solution);
                break;
            case 6:
                var modulesid = Bomb.GetModuleNames();
                Solution = 2 + 39;
                foreach (var module in modulesid)
                {
                    if(module == "Prime Checker")
                    {
                        Solution = Solution - 17;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 6 AND Prime checker present is {1}", moduleId, Solution);
                        break;
                    }
                    if(module == "Prime Encryption")
                    {
                        Solution = Solution + 3301;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 6 AND Prime Encryption present is {1}", moduleId, Solution);
                        break;
                    }
                }
                if(Solution < 0)
                {
                    Solution += 10000;
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 6 is {1}", moduleId, Solution);
                break;
            case 7:
                var ModulesID = Bomb.GetModuleNames();
                var Num1 = Bomb.GetSerialNumberNumbers().Sum();
                var letters = Bomb.GetSerialNumberLetters().Count();
                Solution = 603 - 503;
                foreach(var module in ModulesID)
                {
                    if(module == "Osu!")
                    {
                        Solution = Solution + (Num1 - letters);
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 7 and Osu! is present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 7 is {1}", moduleId, Solution);
                break;
            case 8:
                Solution = 294 + 38;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 8 is {1}", moduleId, Solution);
                break;
            case 9:
                var modulesid1 = Bomb.GetModuleNames();
                Solution = 3141 + 22;
                foreach(var module in modulesid1)
                {
                    if(module == "Pie")
                    {
                        Solution = 3613;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 9 AND pi module present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 9 is {1}", moduleId, Solution);
                break;
            case 10:
                Solution = 6028;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 10 is {1}", moduleId, Solution);
                break;
            case 11:
                var modulesid2 = Bomb.GetModuleNames();
                Solution = 0 + 43;
                foreach (var module in modulesid2)
                {
                    if(module == "The Matrix")
                    {
                        Solution = (Solution * 6) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 11 AND The Matrix present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 11 is {1}", moduleId, Solution);
                break;
            case 12:
                Solution = 6022;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 12 is {1}", moduleId, Solution);
                break;
            case 13:
                Solution = 666;
                var id = Bomb.GetSolvedModuleNames();
                var modules = Bomb.GetModuleNames();
                foreach(var module in modules)
                {
                    if (module == "Purgatory")
                    {
                        Solution = Solution + 777;
                        ispurgatoryon = true;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 13 AND Purgatory present is {1}", moduleId, Solution);
                        break;
                    }
                }
                if(ispurgatoryon == false)
                {
                    foreach (var ID in id)
                    {
                        if (ID == "Creation")
                        {
                            Solution = (Solution + 666 * 2) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 13 and Creation present is {1}", moduleId, Solution);
                            break;
                        }
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 13 is {1}", moduleId, Solution);
                break;
            case 14:
                Solution = 777;
                var id1 = Bomb.GetModuleNames();
                var modules1 = Bomb.GetModuleNames();
                foreach (var module in modules1)
                {
                    if (module == "Purgatory")
                    {
                        Solution = Solution + 666;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 14 and Purgatory present is {1}", moduleId, Solution);
                    }
                }
                if(ispurgatoryon == false)
                {
                    foreach (var ID in id1)
                    {
                        if (ID == "The Necronomicon")
                        {
                            Solution = (Solution + 777*2) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 14 and Necronomicon present is {1}", moduleId, Solution);
                        }
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 14 is {1}", moduleId, Solution);
                break;
            case 15:
                var ModulesNames = Bomb.GetModuleNames();
                Solution = 1337;
                foreach(var Module in ModulesNames)
                {
                    if(Module == "Colour Code"||Module == "Ultimate Cipher" ||Module == "Rainbow Arrows")
                    {
                        var sum = Bomb.GetSerialNumberNumbers().Sum();
                        Solution = Solution + sum;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 15 AND either Colour code/Ultimate Cipher/ Rainbow Arrows are present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Solution = 1337 + 31;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 15 is {1}", moduleId, Solution);
                break;
            case 16:
                Solution = 200;
                if (colourpositions[1] == "magenta")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + ports) % 30;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 16 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + (portplates * porttypes)) % 30;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 16 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution - portplates) % 30;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 16 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                if (colourpositions[2] == "yellow")
                {
                    if (strikes == 0)
                    {
                        if (Bomb.GetModuleNames().Count() < 10)
                        {
                            Solution = (Solution + allmodules) % 30;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 16 with 0 strikes is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        if (Bomb.GetModuleNames().Count() < 12)
                        {
                            Solution = (Solution + 12) % 30;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 16 with 1 strikes is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes >= 2)
                    {
                        if (Bomb.GetModuleNames().Count() < 15)
                        {
                            Solution = (Solution + allmodules) % 30;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 16 with 2 or more strikes is {1}", moduleId, Solution);
                        }
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 16 is {1}", moduleId, Solution);
                break;
            case 17:
                Solution = 709;
                if (colourpositions[0] == "green")
                {
                    if (strikes == 0)
                    {
                        int indexoffletter = AlphabetLetters.IndexOf(firstletter);
                        Debug.LogFormat("[The Answer to ... #{0}] The First letter used for index 17 is {1}", moduleId, indexoffletter);
                        indexoffletter += 1;
                        Solution = (Solution + indexoffletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 17 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        int indexoflletter = AlphabetLetters.IndexOf(lastletter);
                        Debug.LogFormat("[The Answer to ... #{0}] Last letter used for index 17 is {1}", moduleId, indexoflletter);
                        indexoflletter += 1;
                        Solution = (Solution + indexoflletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 17 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        foreach (var letter in allletters)
                        {
                            int indexofletter = AlphabetLetters.IndexOf(letter);
                            indexofletter += 1;
                            Solution += indexofletter;
                        }
                        Solution = Solution % 30;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 17 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                if (colourpositions[3] == "cyan")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + (AAbat * Dbat));
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 17 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + bateries);
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 17 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution + (bateries * battholder)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 17 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 17 is {1}", moduleId, Solution);
                break;
            case 18:
                Solution = 981;
                if (colourpositions[5] == "green")
                {
                    if (strikes == 0)
                    {
                        int indexoffletter = AlphabetLetters.IndexOf(firstletter);
                        Debug.LogFormat("[The Answer to ... #{0}] The First letter used for index 18 is {1}", moduleId, indexoffletter);
                        indexoffletter += 1;
                        Solution = (Solution + indexoffletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 18 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        int indexoflletter = AlphabetLetters.IndexOf(lastletter);
                        Debug.LogFormat("[The Answer to ... #{0}] Last letter used for index 18 is {1}", moduleId, indexoflletter);
                        indexoflletter += 1;
                        Solution = (Solution + indexoflletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 18 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        foreach (var letter in allletters)
                        {
                            int indexofletter = AlphabetLetters.IndexOf(letter);
                            indexofletter += 1;
                            Solution += indexofletter;
                        }
                        Solution = Solution % 30;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 18 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                if (colourpositions[4] == "magenta")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + ports) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 18 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + (portplates * porttypes)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 18 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution - portplates) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 18 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 18 is {1}", moduleId, Solution);
                break;
            case 19:
                Solution = 839;
                if (colourpositions[2] == "yellow")
                {
                    if (strikes == 0)
                    {
                        if (Bomb.GetModuleNames().Count() < 10)
                        {
                            Solution = (Solution + allmodules) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 19 with less than 10 modules on the bomb is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        if (Bomb.GetModuleNames().Count() < 12)
                        {
                            Solution = (Solution + 12) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 19 with less than 12 modules on the bomb is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes >= 2)
                    {
                        if (Bomb.GetModuleNames().Count() < 15)
                        {
                            Solution = (Solution + solvedmodulesnames.Count()) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 19 with less than 15 modules on the bomb is {1}", moduleId, Solution);
                        }
                    }
                }
                if (colourpositions[0] == "cyan")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + (AAbat * Dbat)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 19 with 0 strikes on the bomb is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + bateries) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 19 with 1 strikes on the bomb is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution + (bateries * battholder)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 19 with 2 strikes on the bomb is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 19 is {1}", moduleId, Solution);
                break;
            case 20:
                var modulesid3 = Bomb.GetModuleNames();
                Solution = 4011;
                foreach (var module in modulesid3)
                {
                    if(module == "Fruits")
                    {
                        Solution = (Solution * 33) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 20 with Fruits present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 20 is {1}", moduleId, Solution);
                break;
            case 21:
                Solution = 836;
                if (colourpositions[0] == "blue")
                {
                    if (strikes == 0)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.IND))
                        {
                            if (Bomb.IsIndicatorOn(Indicator.IND))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 5));
                                Solution = Solution % 10000;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 0 strikes AND Lit IND on the bomb is {1}", moduleId, Solution);
                            }
                        }
                    }
                    else if (strikes == 1)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.FRK))
                        {
                            if (Bomb.IsIndicatorOff(Indicator.FRK))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 9));
                                Solution = Solution % 10000;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 1 strikes AND unlit FRK on the bomb is {1}", moduleId, Solution);
                            }
                        }
                    }
                    else if (strikes >= 2)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.BOB))
                        {
                            if (Bomb.IsIndicatorOn(Indicator.BOB))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 5));
                                Solution = Solution % 30;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 2 or more strikes AND lit Bob on the bomb is {1}", moduleId, Solution);
                            }
                        }
                    }
                }
                if (colourpositions[5] == "red")
                {
                    if (strikes == 0)
                    {
                        if (lastnumber % 2 == 0)
                        {
                            Solution = lastnumber * numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 0 strikes AND last number is even on the bomb is {1}", moduleId, Solution);
                        }
                        else
                        {
                            Solution -= numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 0 strikes AND last number is odd on the bomb is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        Solution = firstnumber * lastnumber;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution += SumNumbers;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 21 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 21 is {1}", moduleId, Solution);
                break;
            case 22:
                Solution = 33;
                if (colourpositions[2] == "magenta")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + ports) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 22 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + (portplates * porttypes)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 22 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution - portplates) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 22 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                else if (colourpositions[3] == "red")
                {
                    if (strikes == 0)
                    {
                        if (lastnumber % 2 == 0)
                        {
                            Solution = lastnumber * numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 22 with 0 strikes AND last number is even on the bomb is {1}", moduleId, Solution);
                        }
                        else
                        {
                            Solution -= numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution index 22 with 0 strikes AND last number is odd on the bomb is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        Solution = firstnumber * lastnumber;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 22 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution += SumNumbers;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution index 22 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 22 is {1}", moduleId, Solution);
                break;
            case 23:
                var modulesid4 = Bomb.GetModuleNames();
                Solution = 49;
                foreach (var module in modulesid4)
                {
                    if (module == "Mafia")
                    {
                        Solution = 49 + 11;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 23 and Mafia present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 23 is {1}", moduleId, Solution);
                break;
            case 24:
                Solution = 1200;
                if (colourpositions[4] == "yellow")
                {
                    if (strikes == 0)
                    {
                        if (Bomb.GetModuleNames().Count() < 10)
                        {
                            Solution = (Solution + allmodules) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 AND less than 10 modules on the bomb is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        if (Bomb.GetModuleNames().Count() < 12)
                        {
                            Solution = (Solution + 12) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 AND less than 12 modules on the bomb is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes >= 2)
                    {
                        if (Bomb.GetModuleNames().Count() < 15)
                        {
                            Solution = (Solution + solvedmodulesnames.Count()) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 AND less than 15 modules on the bomb is {1}", moduleId, Solution);
                        }
                    }
                }
                if (colourpositions[0] == "green")
                {
                    if (strikes == 0)
                    {
                        int indexoffletter = AlphabetLetters.IndexOf(firstletter);
                        indexoffletter += 1;
                        Solution = (Solution + indexoffletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        int indexoflletter = AlphabetLetters.IndexOf(lastletter);
                        indexoflletter += 1;
                        Solution = (Solution + indexoflletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 with 1 strike is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        foreach (var letter in allletters)
                        {
                            int indexofletter = AlphabetLetters.IndexOf(letter);
                            indexofletter += 1;
                            Solution += indexofletter;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 with 2 or more strikes is {1}", moduleId, Solution);
                        }
                        Solution = Solution % 10000;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 24 is {1}", moduleId, Solution);
                break;
            case 25:
                Solution = 1459;
                if (colourpositions[0] == "blue")
                {
                    if (strikes == 0)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.IND))
                        {
                            if (Bomb.IsIndicatorOn(Indicator.IND))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 5));
                                Solution = Solution % 10000;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 0 strikes AND lit IND present is {1}", moduleId, Solution);
                            }
                        }
                    }
                    else if (strikes == 1)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.FRK))
                        {
                            if (Bomb.IsIndicatorOff(Indicator.FRK))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 9));
                                Solution = Solution % 10000;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 1 strikeAND unlit FRK present is {1}", moduleId, Solution);
                            }
                        }
                    }
                    else if (strikes >= 2)
                    {
                        if (Bomb.IsIndicatorPresent(Indicator.BOB))
                        {
                            if (Bomb.IsIndicatorOn(Indicator.BOB))
                            {
                                solution = Convert.ToDouble(Solution);
                                Solution = Convert.ToInt32(Math.Pow(Solution, 5));
                                Solution = Solution % 10000;
                                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 2 or more strikes AND for lit BOB is {1}", moduleId, Solution);
                            }
                        }
                    }
                }
                else if (colourpositions[3] == "red")
                {
                    if (strikes == 0)
                    {
                        if (lastnumber % 2 == 0)
                        {
                            Solution = lastnumber * numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 0 strikes AND last number is even is {1}", moduleId, Solution);
                        }
                        else
                        {
                            Solution -= numbers;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 0 strkes AND last number is odd is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        Solution = firstnumber * lastnumber;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 1 strike is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution += SumNumbers;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 with 2 strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 25 is {1}", moduleId, Solution);
                break;
            case 26:
                Solution = 1523;
                if (colourpositions[5] == "cyan")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + (AAbat * Dbat)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + bateries) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 with 1 strike is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution + (bateries * battholder)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                else if (colourpositions[4] == "yellow")
                {
                    if (strikes == 0)
                    {
                        if (Bomb.GetModuleNames().Count() < 10)
                        {
                            Solution = (Solution + allmodules) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 with 0 strikes is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes == 1)
                    {
                        if (Bomb.GetModuleNames().Count() < 12)
                        {
                            Solution = (Solution + 12) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 with 1 strike is {1}", moduleId, Solution);
                        }
                    }
                    else if (strikes >= 2)
                    {
                        if (Bomb.GetModuleNames().Count() < 15)
                        {
                            Solution = (Solution + solvedmodulesnames.Count()) % 10000;
                            Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 with 2 strikes is {1}", moduleId, Solution);
                        }
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 26 is {1}", moduleId, Solution);
                break;
            case 27:
                Solution = 1499;
                if (colourpositions[1] == "green")
                {
                    if (strikes == 0)
                    {
                        int indexoffletter = AlphabetLetters.IndexOf(firstletter);
                        indexoffletter += 1;
                        Solution = (Solution + indexoffletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        int indexoflletter = AlphabetLetters.IndexOf(lastletter);
                        indexoflletter += 1;
                        Solution = (Solution + indexoflletter) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        foreach (var letter in allletters)
                        {
                            int indexofletter = AlphabetLetters.IndexOf(letter);
                            indexofletter += 1;
                            Solution += indexofletter;
                        }
                        Solution = Solution % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                else if (colourpositions[0] == "magenta")
                {
                    if (strikes == 0)
                    {
                        Solution = (Solution + ports) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 with 0 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes == 1)
                    {
                        Solution = (Solution + (portplates * porttypes)) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 with 1 strikes is {1}", moduleId, Solution);
                    }
                    else if (strikes >= 2)
                    {
                        Solution = (Solution - portplates) % 10000;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 with 2 or more strikes is {1}", moduleId, Solution);
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 27 is {1}", moduleId, Solution);
                break;
            case 28:
                Solution = 8944;
                var modulesid5 = Bomb.GetModuleNames();
                foreach(var module in modulesid5)
                {
                    if(module == "The Cube")
                    {
                        Solution = 6832;
                        Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 28 with the Cube module present is {1}", moduleId, Solution);
                        break;
                    }
                }
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 28 is {1}", moduleId, Solution);
                break;
            case 29:
                Solution = 9091;
                Debug.LogFormat("[The Answer to ... #{0}] The Solution for index 29 is {1}", moduleId, Solution);
                break;
        }          
    }


    void SubmitButton()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Submit.AddInteractionPunch();
        if(moduleSolved == true)
        {
            return;
        }
        if(Display.text == Solution.ToString())
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            strikes++;
            Display.text = "";
        }
    }

    void ClearButton()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Display.text = "";
    }

    void Handler(KMSelectable PressedNumber)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        PressedNumber.AddInteractionPunch();
        if(moduleSolved == true || Display.text.Length == 4)
        {
            return;
        }
        Display.text += PressedNumber.GetComponentInChildren<TextMesh>().text;
    }

}
