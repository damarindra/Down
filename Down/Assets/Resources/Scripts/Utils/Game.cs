using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Game {

    public static Game currentGame;

    public int coins;
    public int highScore;
    public int timesPlay;
    public CharacterInfo[] charInfo;
    public string currentCharacter;

    public Game(int characterLength)
    {
        coins = 0;
        highScore = 0;
        timesPlay = 0;
        charInfo = new CharacterInfo[characterLength];
        currentCharacter = "Bird";
        //charInfo = new CharacterInfo[Showcase.instance.characterPrefabs.Length];
        int i = 0;
        while (i < Showcase.instance.characterPrefabs.Length)
        {
            charInfo[i] = new CharacterInfo(Showcase.instance.characterPrefabs[i].name, Showcase.instance.priceCharacter[i]);
            i++;
        }
    }

    public string[] getCharacterID() {
        string[] ids = new string[charInfo.Length];

        int i = 0;
        foreach (CharacterInfo info in charInfo) {
            ids[i] = info.id;
            i++;
        }

        return ids;
    }
    public bool[] getCharacterUnlock() {
        bool[] isUnlocked = new bool[charInfo.Length];

        int i = 0;
        foreach (CharacterInfo info in charInfo) {
            isUnlocked[i] = info.isUnlocked;
            i++;
        }

        return isUnlocked;
    }
}
