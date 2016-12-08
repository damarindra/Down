using UnityEngine;
using System.Collections;

public class TNT : MonoBehaviour {

    public float timeToExplode;
    public bool countDownExplode = false;
    private bool hasBeenExplode = false;
    public ParticleSystem explodeParticle;
    public AudioSource audioSource;
    Animator anim;

	// Use this for initialization
	void Start () {
        anim = transform.parent.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (countDownExplode && !hasBeenExplode)
        {
            timeToExplode -= Time.deltaTime;
            if (timeToExplode <= 0)
            {
                //Explode
                //Raycasting Left and Right
                Explode();
                countDownExplode = false;
                hasBeenExplode = true;
            }
        }
	}

    public void Explode()
    {
        explodeParticle.transform.SetParent(transform.parent.parent);
        explodeParticle.Emit(10);
        anim.SetTrigger("Explode");
        audioSource.Play();

        Vector3[] playerChecker = new Vector3[3];
        playerChecker[0] = new Vector3(transform.position.x, (transform.position.y + 1), transform.position.z);
        playerChecker[1] = new Vector3(transform.position.x, (transform.position.y), transform.position.z + 1);
        playerChecker[2] = new Vector3(transform.position.x + 1, (transform.position.y), transform.position.z);

        foreach (Vector3 v3 in playerChecker) {
            if (v3.Equals(GameManager.instance.playerPosition))
                GameManager.instance.instanceCharacter.GetComponent<PlayerController>().Die();
        }

        /*
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Player")
            {
                hit.collider.transform.parent.parent.gameObject.GetComponent<PlayerController>().Die();
            }
            else if (hit.collider.tag == "TNT")
            {
                hit.collider.gameObject.GetComponent<TNT>().Explode(Vector3.right);
                hit.collider.gameObject.GetComponent<TNT>().Explode(Vector3.up);
                hit.collider.gameObject.GetComponent<TNT>().Explode(Vector3.forward);
            }
        }*/
    }
}
