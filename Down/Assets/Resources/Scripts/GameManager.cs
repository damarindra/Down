using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public int score = 0;
    public int coin = 0;

    public Camera mainCamera;
    public Camera characterSelectionCamera;

    [HideInInspector]
    public Game gameProgress;


    [HideInInspector] public GameObject characterChoosen;
    [HideInInspector] public GameObject instanceCharacter;
    [HideInInspector]
    public Vector3 playerPosition;

    [HideInInspector]
    public bool isSoundOn = true;

    [HideInInspector]
    public int interstitialCounter;

    public bool deleteGame;

    public enum GameState {
        Mainmenu,
        CharacterSelection,
        Gameplay,
        Gameover
    }

    public GameState gameState = GameState.Mainmenu;

    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        mainCamera.gameObject.SetActive(true);
        characterSelectionCamera.gameObject.SetActive(false);
        if (deleteGame)
            SaveLoad.DeleteAllGameSaved();
        SaveLoad.LoadSavedGame();

        interstitialCounter = Configuration.instance.interstitialShowPerGame;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}
