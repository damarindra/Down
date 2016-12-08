using UnityEngine;
using System.Collections;

public class Splash : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Services.Instance.GPGSSetup();
        Services.Instance.BannerSetup();
        Services.Instance.InterstitialSetup();


        Services.Instance.BannerShow();

        StartCoroutine(load());
    }

    IEnumerator load() {
        AsyncOperation async = Application.LoadLevelAsync("Main");
        yield return async;
        Debug.Log("Loading complete");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
