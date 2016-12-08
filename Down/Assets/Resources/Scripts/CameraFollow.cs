using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    [HideInInspector]public bool isMove = false;
    private Vector3 nextPos = Vector3.zero;
    private Vector3 originPosition;
    private Transform playerFollower;

    public float dampingMove;

	// Use this for initialization
	void Start () {
        originPosition = transform.position;
        playerFollower = GameObject.FindGameObjectWithTag("CameraFollower").transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (isMove)
        {
            //transform.position = Vector3.MoveTowards(transform.position, nextPos, 4 * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, nextPos, dampingMove * Time.deltaTime);
            
            if (transform.position == nextPos)
                isMove = false;
        }
	}

    public void setNextPos()
    {
        if(nextPos == Vector3.zero)
            nextPos = transform.position + (1.219f * -transform.up);
        else
            nextPos = nextPos + (1.219f * -transform.up);

        //nextPos = new Vector3((int)transform.position.x + .5f, (int)transform.position.y - 1, (int)transform.position.z + .5f);
        //if (nextPos.x < playerFollower.position.x)
        //    nextPos.z += 1;
        //else if (nextPos.z < playerFollower.position.z)
        //    nextPos.x += 1;
    }

    public void setPosToOrigin()
    {
        //playerFollower = GameObject.FindGameObjectWithTag("CameraFollower").transform;
        //transform.position = playerFollower.position;
        transform.position = originPosition;
        nextPos = Vector3.zero;
        isMove = false;
    }
}
