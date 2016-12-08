using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterInfo {

    public string id;
    public bool isUnlocked = false;
    public int price;

    public CharacterInfo(string id, int price)
    {
        this.id = id;
        this.price = price;
    }
}
