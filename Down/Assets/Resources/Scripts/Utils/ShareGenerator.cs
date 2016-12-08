using UnityEngine;
using System.Collections;
using System.IO;

public class ShareGenerator : MonoBehaviour {

    public static ShareGenerator instance = null;

    private bool isProcessing = false;
    private int score;
    
    private string gameLink = "Download the game on play store at " + "\nhttps://play.google.com/store/apps/details?id=com.cgranule.down";
    private string imageName = "myPic"; // without the extension, for iinstance, MyPic 

    void Awake() {
        if (instance == null)
            instance = this;
    }

    public void shareScore(int score)
    {
#if UNITY_ANDROID
        this.score = score;
        //if (!isProcessing)
        //   StartCoroutine(ShareScreenshot());
        Application.CaptureScreenshot("downSS.png");
        string origin = System.IO.Path.Combine(Application.persistentDataPath, "downSS.png");
        string destination = "/sdcard/ScreenCapture/" + "downSS.png"; // could be anything
        if (System.IO.File.Exists(origin))
        {
            System.IO.File.Move(origin, destination);
        }

        if (!Application.isEditor)
        {

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Hi I got Score " + score.ToString() + "!!Can you beat it ? \n\n" + gameLink);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }
#endif
    }



    

#if UNITY_ANDROID
    private IEnumerator ShareScreenshot()
    {

        isProcessing = true;
        yield return new WaitForEndOfFrame();

        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        screenTexture.Apply();

        byte[] dataToSave = Resources.Load<TextAsset>(imageName).bytes;

        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        Debug.Log(destination);
        File.WriteAllBytes(destination, dataToSave);

        if (!Application.isEditor)
        {

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Hi I got Score " + score.ToString() + "!!Can you beat it ? \n\n" + gameLink);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);

        }

        isProcessing = false;

    }
#endif
}
