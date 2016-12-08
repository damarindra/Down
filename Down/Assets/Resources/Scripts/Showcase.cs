using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Showcase : MonoBehaviour {

    public static Showcase instance = null;

    public GameObject[] characterPrefabs;
    public int[] priceCharacter;
    public Transform characterSelectionParent;
    private Vector3 characterSelectionOriginPos;

    private Vector3 lastTouchPos;

    private Transform cornerPos;
    private Vector3 cornerOriginPos;
    private float distanceParentCorner;
    private List<Transform> characterTransformArray = new List<Transform>();

    private List<GameObject> instanceCharacter = new List<GameObject>();
    private Material[,] trueMaterial;
    private Color[,] trueColor;
    private Color[,] bwColor;

    public Text nameText;
    public GameObject buttonOke;
    public GameObject buttonBuy;
    private Text textPrice;

    public Text diamondText;

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start() {
        characterSelectionOriginPos = characterSelectionParent.position;
        trueMaterial = new Material[characterPrefabs.Length, 10];
        trueColor = new Color[characterPrefabs.Length, 10];
        bwColor = new Color[characterPrefabs.Length, 10];

        textPrice = buttonBuy.transform.GetChild(0).GetComponent<Text>();

        InstantiateAllCharacter();
    }

    public void InstantiateAllCharacter()
    {
        float x = 0;

        int i = 0;
        while (i < characterPrefabs.Length)
        {
            GameObject instance = Instantiate(characterPrefabs[i],
                new Vector3(characterSelectionParent.position.x - x, characterSelectionParent.position.y, characterSelectionParent.position.z), Quaternion.Euler(20, 315, 340)) as GameObject;
            instance.transform.SetParent(characterSelectionParent);
            instance.name = characterPrefabs[i].name;
            instance.GetComponent<PlayerController>().enabled = false;
            characterTransformArray.Add(instance.transform);
            x += 1.5f;
            i++;
            if (i == characterPrefabs.Length)
            {
                cornerPos = new GameObject("cornerRight").transform;
                cornerPos.SetParent(characterSelectionParent);
                cornerOriginPos = instance.transform.position;
                cornerPos.position = cornerOriginPos;
                distanceParentCorner = cornerOriginPos.x - characterSelectionParent.position.x;
            }
            instanceCharacter.Add(instance);
        }
        InitializeAllMaterialAndColor();
    }

    void InitializeAllMaterialAndColor()
    {
        int i = 0;
        foreach (GameObject go in instanceCharacter)
        {
            int j = 0;
            Material[] mat = go.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().materials;//.GetComponentsInChildren<Material>();
            while (j < mat.Length)
            {
                trueMaterial[i, j] = mat[j];
                trueColor[i, j] = mat[j].color;
                float meanColor = (trueColor[i,j].r + trueColor[i, j].g + trueColor[i, j].b) / 3;
                bwColor[i, j] = new Color(meanColor, meanColor, meanColor);
                j++;
            }
            i++;
        }
        SetColorColorOrBW();
    }

    public void SetColorColorOrBW()
    {
        bool[] isUnlocked = PlayerPrefsX.GetBoolArray("CharacterUnlocked");
        int i = 0;
        while (i < isUnlocked.Length)
        {
            int j = 0;
            //Debug.Log(isUnlocked[i]);
            //Debug.Log(i);
            while (j < 10)
            {
                if (trueMaterial[i, j] != null)
                {
                    if (!isUnlocked[i])
                        trueMaterial[i, j].color = bwColor[i, j];
                    else if (isUnlocked[i])
                        trueMaterial[i, j].color = trueColor[i, j];
                }
                j++;
            }
            i++;
        }
    }

    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.CharacterSelection)
        {
            //Logic Char Selection
            if (Input.GetMouseButtonDown(0))
            {
                lastTouchPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                float distanceX = lastTouchPos.x - Input.mousePosition.x;
                if (distanceX < 0)
                {
                    if (characterSelectionParent.position.x <= characterSelectionOriginPos.x)
                    {
                        characterSelectionParent.position = characterSelectionOriginPos;
                        distanceX = 0;
                    }
                }
                else if (distanceX > 0)
                {
                    if (cornerPos.position.x >= characterSelectionOriginPos.x)
                    {
                        characterSelectionParent.position = new Vector3(characterSelectionOriginPos.x - distanceParentCorner, characterSelectionParent.position.y, characterSelectionParent.position.z);
                        distanceX = 0;
                    }
                }
                characterSelectionParent.position = new Vector3(characterSelectionParent.position.x + (distanceX * 2 * Time.deltaTime), characterSelectionParent.position.y, characterSelectionParent.position.z);
                lastTouchPos = Input.mousePosition;
            }
            SelectCharacter();
        }

    }

    void SelectCharacter()
    {
        int i = 0;
        foreach (Transform tr in characterTransformArray)
        {
            if (tr.position.x > GameManager.instance.characterSelectionCamera.transform.position.x - 0.75f && tr.position.x <= GameManager.instance.characterSelectionCamera.transform.position.x + 0.75f)
            {
                //GameManager.instance.characterChoosen = tr.gameObject;
                GameManager.instance.characterChoosen = characterPrefabs[i];
                tr.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                nameText.text = tr.gameObject.name;
                nameText.transform.GetChild(0).GetComponent<Text>().text = tr.gameObject.name;
                diamondText.text = SaveLoad.savedGame.coins.ToString();
                diamondText.transform.GetChild(0).GetComponent<Text>().text = SaveLoad.savedGame.coins.ToString();
                //Debug.Log(SaveLoad.savedGame.coins);
                if (SaveLoad.CheckIsCharacterUnlocked(tr.gameObject.name))
                {
                    buttonOke.SetActive(true);
                    buttonBuy.SetActive(false);
                }
                else {
                    buttonOke.SetActive(false);
                    buttonBuy.SetActive(true);
                    textPrice.text = priceCharacter[i].ToString();
                }
            }
            else
                tr.localScale = Vector3.one;
            i++;
        }
    }
}
