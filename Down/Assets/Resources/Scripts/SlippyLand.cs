using UnityEngine;
using System.Collections;

public class SlippyLand : MonoBehaviour {

    public enum Direction {
        left,
        right
    }
    public Direction dir = Direction.left;

    public float waitTime;
    public bool isTriggered;

    private float timer = 0;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (isTriggered)
        {
            //Debug.Log(timer);
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                GameManager.instance.instanceCharacter.GetComponent<PlayerController>().TouchSlipperyLand(dir);
                timer = 0;
                isTriggered = false;
            }
        }
	}
}
