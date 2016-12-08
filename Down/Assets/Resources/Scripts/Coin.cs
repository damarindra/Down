using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public LayerMask playerMask;
    public ParticleSystem particle;
    public Transform raycaster;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //RaycastingPlayer();
	}

    void RaycastingPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(raycaster.position, Vector3.up, out hit, 2f, playerMask);
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            particle.Emit(20);
            SaveLoad.savedGame.coins += 1;
        }
    }
}
