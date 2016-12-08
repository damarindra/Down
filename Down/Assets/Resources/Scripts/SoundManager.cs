using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;
    private AudioSource bgMusicSource;
    public AudioSource effectSource;

    // Use this for initialization
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        bgMusicSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (!GameManager.instance.isSoundOn)
        {
            if (bgMusicSource.isPlaying)
                bgMusicSource.Stop();
        }
        else {
            if (!bgMusicSource.isPlaying)
                bgMusicSource.Play();
        }
    }

    public void RandomSingle(params AudioClip[] single)
    {
        if (GameManager.instance.isSoundOn)
        {
            effectSource.clip = single[Random.Range(0, single.Length -1)];
            effectSource.Play();
        }
    }
}
