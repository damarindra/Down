using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UICanvas : MonoBehaviour {

    public static UICanvas instance = null;

    public Text scoreText;
    public GameObject gameplayPanel;
    public GameObject gameoverPanel;
    public GameObject touchHelpPanel;
    public GameObject characterSelectionPanel;

    [HideInInspector]public Animator anim;

    public Button soundButton;
    public Sprite soundOnSpr;
    public Sprite soundOffSpr;

    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        anim = GetComponent<Animator>();
    }

    void Start() {
        if (GameManager.instance.isSoundOn)
            soundButton.image.sprite = soundOnSpr;
        else
            soundButton.image.sprite = soundOffSpr;
    }

    // Update is called once per frame
    void Update() {
        UpdateScore();
        if (touchHelpPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
                anim.SetTrigger("TouchOut");
        }

    }

    void UpdateScore()
    {
        scoreText.text = GameManager.instance.score.ToString();
        scoreText.transform.GetChild(0).GetComponent<Text>().text = GameManager.instance.score.ToString();
    }

    public void StartButton()
    {
        GameManager.instance.gameState = GameManager.GameState.Gameplay;
    }

    public void AgainButton()
    {
        GameManager.instance.interstitialCounter -= 1;
        if (GameManager.instance.interstitialCounter <= 0)
        {
            Services.Instance.InterstitialShow();
            GameManager.instance.interstitialCounter = Configuration.instance.interstitialShowPerGame;
        }
        GameManager.instance.gameState = GameManager.GameState.Gameplay;
        GameManager.instance.instanceCharacter.SetActive(false);
        BoardManager.instance.Restart();
        anim.SetTrigger("LoadingIn");
        anim.SetTrigger("GameoverFadeOut");
        Debug.Log(GameManager.instance.interstitialCounter);
    }

    public void ExitButton()
    {

    }

    public void SoundButton()
    {
        SaveLoad.ToggleSound();
        if (GameManager.instance.isSoundOn)
            soundButton.image.sprite = soundOnSpr;
        else
            soundButton.image.sprite = soundOffSpr;
    }
    public void ShareButton()
    {
        Services.Instance.Share(GameManager.instance.score);
        //ShareGenerator.instance.shareScore(GameManager.instance.score);
    }
    public void CharacterButton()
    {
        anim.SetTrigger("LoadingIn");
        anim.SetTrigger("GameoverFadeOut");
        StartCoroutine(PanelSwitcher(characterSelectionPanel, 0.3f));
        StartCoroutine(PanelSwitcher(gameplayPanel, 0.3f));
        Invoke("ChangeToCharacterScreen", 1f);

    }
    public void CharacterBackButton()
    {
        anim.SetTrigger("LoadingIn");
        GameManager.instance.gameState = GameManager.GameState.Gameplay;
        StartCoroutine(PanelSwitcher(characterSelectionPanel, 0.5f));
        StartCoroutine(PanelSwitcher(gameplayPanel, 0.3f));
        Invoke("SwitchCamera", .3f);
        BoardManager.instance.Restart();
    }
    public void AchievementButton()
    {
        Services.Instance.ShowAchievementsUI();
    }
    public void LeaderboardButton()
    {
        Services.Instance.LeaderboardShow();
    }

    void ChangeToCharacterScreen()
    {
        GameManager.instance.gameState = GameManager.GameState.CharacterSelection;
        SwitchCamera();
    }

    public void ChooseCharacter()
    {
        anim.SetTrigger("LoadingIn");
        SaveLoad.SaveCharacterRecentUse(GameManager.instance.characterChoosen.name);
        GameManager.instance.gameState = GameManager.GameState.Gameplay;
        StartCoroutine(PanelSwitcher(characterSelectionPanel, 0.5f));
        StartCoroutine(PanelSwitcher(gameplayPanel, 0.3f));
        Invoke("SwitchCamera", .3f);
        BoardManager.instance.Restart();
    }

    public void BuyCharacter() {
        if (SaveLoad.savedGame.coins >= Int32.Parse(Showcase.instance.buttonBuy.GetComponentInChildren<Text>().text)) {
            SaveLoad.UnlockCharacter(Showcase.instance.nameText.text);
            Showcase.instance.SetColorColorOrBW();
            Services.Instance.UnlockAchievement(Configuration.instance.firstBuyAchievement);
        }
    }

    void SwitchCamera()
    {
        GameManager.instance.characterSelectionCamera.gameObject.SetActive(!GameManager.instance.characterSelectionCamera.gameObject.activeSelf);
        GameManager.instance.mainCamera.gameObject.SetActive(!GameManager.instance.mainCamera.gameObject.activeSelf);
    }

    IEnumerator PanelSwitcher(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(!go.activeSelf);
    }
}
