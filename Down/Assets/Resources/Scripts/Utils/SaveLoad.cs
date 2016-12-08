using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveLoad {

    public static SaveLoad sInstance = new SaveLoad();
    public static Game savedGame;
    public static bool loadSavedGameComplete = false;

    private static string firstInitializeId = "firstInitialize";

    public static SaveLoad Instance {
        get { return sInstance; }
    }

    public static void SaveCoin()
    {
        PlayerPrefs.SetInt("Coin", savedGame.coins);
    }

    public static void SaveTimesPlay(int times)
    {
        PlayerPrefs.SetInt("TimesPlay", times);
    }

    public static void SaveScore()
    {
        PlayerPrefs.SetInt("HighScore", savedGame.highScore);
    }
    public static int GetScore()
    {
        return PlayerPrefs.GetInt("HighScore");
    }

    public static void SaveCharacterRecentUse(string id)
    {
        PlayerPrefs.SetString("CurrentCharacter", id);
        savedGame.currentCharacter = id;
    }

    public static void UnlockCharacter(string id)
    {
        string[] ids = PlayerPrefsX.GetStringArray("CharacterID");
        int i = 0;
        while (i < ids.Length)
        {
            if (ids[i] == id)
            {
                bool[] isUnlocked = PlayerPrefsX.GetBoolArray("CharacterUnlocked");
                isUnlocked[i] = true;
                PlayerPrefsX.SetBoolArray("CharacterUnlocked", isUnlocked);
                return;
            }
            i++;
        }
        Debug.LogWarning("Character Unlock NOT SAVED, NOT FOUND ID");
    }

    public static GameObject getCurrentCharacter() {
        GameObject go = Showcase.instance.characterPrefabs[0];
        foreach (GameObject g in Showcase.instance.characterPrefabs) {
            if (g.name == PlayerPrefs.GetString("CurrentCharacter")) {
                return g;
            }
        }
        return go;
    }

    public static void LoadSavedGame()
    {
        savedGame = new Game(Showcase.instance.characterPrefabs.Length);
        InitializeCharacterInfo();
        savedGame.coins = PlayerPrefs.GetInt("Coin");
        savedGame.timesPlay = PlayerPrefs.GetInt("TimesPlay");
        savedGame.highScore = PlayerPrefs.GetInt("HighScore");
        savedGame.currentCharacter = PlayerPrefs.GetString("CurrentCharacter");
        loadSavedGameComplete = true;
        GameManager.instance.isSoundOn = PlayerPrefsX.GetBool("Sound");

        if (!PlayerPrefsX.GetBool(firstInitializeId)) {
            PlayerPrefsX.SetBool(firstInitializeId, true);
            PlayerPrefsX.SetBool("Sound", true);
            GameManager.instance.isSoundOn = true;
        }

        //Cheat
        //savedGame.coins += 100;
    }

    public static void SaveGame() {
        SaveCoin();
        SaveScore();
        SaveCharacterRecentUse(GameManager.instance.characterChoosen.name);
    }

    public static bool CheckIsCharacterUnlocked(string id)
    {
        string[] ids = PlayerPrefsX.GetStringArray("CharacterID");
        bool[] isUnlocked = PlayerPrefsX.GetBoolArray("CharacterUnlocked");
        int i = 0;
        while (i < ids.Length)
        {
            if (ids[i] == id)
            {
                if (isUnlocked[i])
                    return true;
                else
                    return false;
            }
            i++;
        }
        return false;
    }
    
    static void InitializeCharacterInfo()
    {
        string[] currentCharacters = PlayerPrefsX.GetStringArray("CharacterID");
        string[] ids = new string[savedGame.charInfo.Length];
        //bool[] isUnlocked = PlayerPrefsX.GetBoolArray("CharacterUnlocked");
		List<bool> isUnlocked = PlayerPrefsX.GetBoolArray ("CharacterUnlocked").ToList ();

        int i = 0;
        while (i < ids.Length) {
            if (i <= currentCharacters.Length)
            {
                if (i == 0)
                {
                    savedGame.charInfo[i].isUnlocked = true;
                }
                else
                {
					if(i <= currentCharacters.Length)
					{
						savedGame.charInfo[i].isUnlocked = isUnlocked[i];
					}
					else
					{
						savedGame.charInfo[i].isUnlocked = false;
					}
                }
            }
            else
                savedGame.charInfo[i].isUnlocked = false;
            i++;
        }

        PlayerPrefsX.SetStringArray("CharacterID", savedGame.getCharacterID());
        PlayerPrefsX.SetBoolArray("CharacterUnlocked", savedGame.getCharacterUnlock());

        /*//Initialize
        if (currentCharacters.Length < ids.Length)
        {
            int i = 0;
            while (i < ids.Length)
            {
                ids[i] = savedGame.charInfo[i].id;
                i++;
            }
            PlayerPrefsX.SetStringArray("CharacterID", ids);

            bool[] isUnlocked = new bool[savedGame.charInfo.Length];
            int j = 0;
            while (j < isUnlocked.Length)
            {
                if (j == 0)
                    isUnlocked[j] = true;
                else
                    isUnlocked[j] = savedGame.charInfo[j].isUnlocked;
                j++;
            }
            PlayerPrefsX.SetBoolArray("CharacterUnlocked", isUnlocked);
        }
        else {

        }*/
    }

    public static void ToggleSound()
    {
        GameManager.instance.isSoundOn = !GameManager.instance.isSoundOn;
        PlayerPrefsX.SetBool("Sound", GameManager.instance.isSoundOn);
    }

    public static void DeleteAllGameSaved()
    {
        PlayerPrefs.DeleteAll();
    }
}
