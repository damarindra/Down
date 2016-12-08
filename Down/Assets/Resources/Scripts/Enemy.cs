using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    public List<Vector3> pathWalk = new List<Vector3>();

    public float timeToMove;
    private float timer;
    public int indexForMove;
    private int pluser = -1;

    private bool isMoving = false;
    private Vector3 nextMove;

    private Animator anim;

	// Use this for initialization
	void Awake () {
        anim = transform.GetChild(0).GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isMoving == false)
        {
            timer += Time.deltaTime;
            if (timer >= timeToMove)
            {
                //move
                if (indexForMove == 0)
                    pluser = 1;
                else if (indexForMove == pathWalk.Count - 1)
                    pluser = -1;
                indexForMove += pluser;
                nextMove = pathWalk[indexForMove];
                isMoving = true;
                timer = 0;
                anim.SetTrigger("Jump");
            }
        }
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextMove, 4 * Time.deltaTime);
            if(transform.position.y >= nextMove.y - 0.1f && transform.position.y <= nextMove.y + 0.1f)
            {
                transform.position = nextMove;
                CheckingForLand();
                isMoving = false;
            }
        }
	}
    
    void RaycastingForPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.GetChild(0).position.x, transform.GetChild(0).position.y - (transform.GetChild(0).GetComponent<BoxCollider>().size.y /2), transform.GetChild(0).position.z),
            Vector3.up, out hit, 1);
        if (hit.collider != null && hit.collider.tag.Equals("Player"))
            hit.collider.transform.parent.gameObject.GetComponent<PlayerController>().Die();
    }

    void CheckingForLand()
    {
        GameObject row = GameObject.Find("RowGroup : " + ((int)transform.position.y - 1).ToString());
        if (row == null)
            Destroy(gameObject);
    }
}
