using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

    public enum SpikeType {
        UpDown,
        Rear
    }

    public SpikeType type;

	// Use this for initialization
	void Start () {
        SpikeBehaviour();
	}

    void Update() {
        //RaycastingPlayer();
    }

    void SpikeBehaviour()
    {
        if (type == SpikeType.UpDown)
            iTween.MoveBy(gameObject, iTween.Hash("y", 1, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .7f));
        else
        {
            iTween.MoveBy(gameObject, iTween.Hash("x", 1, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .7f));
            if(transform.parent.childCount != 1)
                iTween.MoveBy(transform.parent.GetChild(1).gameObject, iTween.Hash("z", 1, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .7f));
        }
    }

    void RaycastingPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z), Vector3.up, out hit, .8f);
        if (hit.collider != null && hit.collider.tag.Equals("Player"))
            hit.collider.transform.parent.gameObject.GetComponent<PlayerController>().Die();
    }
}
