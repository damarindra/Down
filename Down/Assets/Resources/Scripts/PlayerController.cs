using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [HideInInspector]public bool isMoving = false;
    private bool isJumpToAvoidSlippy = false;
    private Vector3 nextPos;

    public Transform placeToRaycast;

    private Animator anim;

    private float idleTimer = 5;
    private bool reverseControl = false;
    private int counterReverse = 10;

    public LayerMask obstacleLayer;

    public ParticleSystem poisonEffect;
    public ParticleSystem explodeEffect;
    public ParticleSystem jumpEffect;

    private AudioSource audioSource;

    public AudioClip[] coinSounds;

    private bool hasFirstMove = false; //for timer, work if has first move

    public LayerMask treeMask;

    enum PlayerDirection { left, right }
    PlayerDirection dir = PlayerDirection.right;

    enum PlayerState { LIVE, DIED }
    PlayerState state = PlayerState.LIVE;

	// Use this for initialization
	void Awake () {
        anim = transform.GetComponentInChildren<Animator>();
        audioSource = transform.GetComponentInChildren<AudioSource>();

        state = PlayerState.LIVE;

        nextPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.instance.gameState == GameManager.GameState.Gameplay)
        {
            CheckObstacle();
            TouchHandler();
        }
	}

    void FixedUpdate()
    {
        if (GameManager.instance.gameState == GameManager.GameState.Gameplay)
        {
            if (isMoving)
            {
                //iTween.MoveTo(gameObject, nextPos, 0.3f);
                transform.position = Vector3.MoveTowards(transform.position, nextPos, 4 * Time.deltaTime);
                if (transform.position.y <= nextPos.y + 0.05f)
                {
                    transform.position = nextPos;
                    isMoving = false;
                    hasbeenCheckSlippy = false;
                    idleTimer = 5;
                    jumpEffect.Emit(5);
                    GameManager.instance.score += 1;                    //Adding Score
                    BoardManager.instance.DestroyAndSpawnNewLand();
                    Camera.main.GetComponent<CameraFollow>().setNextPos();
                    Camera.main.GetComponent<CameraFollow>().isMove = true;
                }

            }
            else if (!isMoving && hasFirstMove)
            {
                if (nextPos.y / transform.position.y != 1)
                    transform.position = nextPos;
                isJumpToAvoidSlippy = false;
                //Logic wait until gameover
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    //END
                    Die();
                    idleTimer = 5;
                }
            }
        }
    }

    public void Move(bool isMoveRight)
    {
        if (!isMoving)
        {
            if (!reverseControl)
            {
                if (isMoveRight && !onRightEdge())
                {
                    nextPos = new Vector3(Mathf.RoundToInt(nextPos.x), Mathf.RoundToInt(nextPos.y - 1), Mathf.RoundToInt(nextPos.z + 1));
                    isMoving = true;
                    RotateAvatar(isMoveRight);
                    anim.SetTrigger("Jump");
                    audioSource.Play();
                }
                else if (!isMoveRight && !onLeftEdge())
                {
                    nextPos = new Vector3(Mathf.RoundToInt(nextPos.x + 1), Mathf.RoundToInt(nextPos.y - 1), Mathf.RoundToInt(nextPos.z));
                    isMoving = true;
                    RotateAvatar(isMoveRight);

                    anim.SetTrigger("Jump");
                    audioSource.Play();
                }
            }
            else if (reverseControl)
            {
                if (isMoveRight)
                {
                    if (!onLeftEdge())
                    {
                        nextPos = new Vector3((int)transform.position.x + 1, (int)transform.position.y - 1, (int)transform.position.z);
                        isMoving = true;
                        RotateAvatar(!isMoveRight);

                        audioSource.Play();
                        anim.SetTrigger("Jump");
                        counterReverse -= 1;
                        if (counterReverse <= 0)
                        {
                            reverseControl = false;
                            counterReverse = 10;
                            poisonEffect.Stop();
                        }
                    }
                    else
                        isMoving = false;
                }
                else if (!isMoveRight)
                {
                    if (!onRightEdge())
                    {
                        counterReverse -= 1;
                        if (counterReverse <= 0)
                        {
                            reverseControl = false;
                            poisonEffect.Stop();
                            counterReverse = 10;
                        }
                        nextPos = new Vector3((int)transform.position.x, (int)transform.position.y - 1, (int)transform.position.z + 1);
                        isMoving = true;
                        RotateAvatar(!isMoveRight);

                        audioSource.Play();
                        anim.SetTrigger("Jump");
                    }
                    else
                        isMoving = false;
                }
            }
            if (!hasFirstMove)
                hasFirstMove = true;
        }
        GameManager.instance.playerPosition = nextPos;
    }

    void RotateAvatar(bool isMoveRight)
    {
        if (dir == PlayerDirection.left && isMoveRight)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(transform.GetChild(0).rotation.x, 0, transform.GetChild(0).rotation.z);
            dir = PlayerDirection.right;
        }
        else if (dir == PlayerDirection.right && !isMoveRight)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(transform.GetChild(0).rotation.x, 90, transform.GetChild(0).rotation.z);
            dir = PlayerDirection.left;
        }
    }

    void TouchHandler()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x >= 0.5f)
            {
                RaycastHit hit;
                if (reverseControl && !onLeftEdge())
                    Physics.Raycast(transform.position, Vector3.right, out hit, 1f, treeMask);
                else
                    Physics.Raycast(transform.position, Vector3.forward, out hit, 1f, treeMask);
                if (hit.collider != null)
                {
                    if (hit.collider.tag.Equals("Tree"))
                    {
                        //Do Nothing
                    }
                }
                else
                {
                    Move(true);
                    isJumpToAvoidSlippy = true;
                }
            }
            else if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.5f)
            {
                RaycastHit hit;
                if (reverseControl && !onRightEdge())
                {
                    // if(!onRightEdge() || onLeftEdge())
                    Physics.Raycast(transform.position, Vector3.forward, out hit, 1f, treeMask);
                    //else
                    //    Physics.Raycast(transform.position, Vector3.right, out hit, 1f);
                }
                else
                    Physics.Raycast(transform.position, Vector3.right, out hit, 1f, treeMask);
                if (hit.collider != null)
                {
                    if (hit.collider.tag.Equals("Tree"))
                    {
                        //Do Nothing
                    }
                }
                else
                {
                    Move(false);
                    isJumpToAvoidSlippy = true;
                }
            }
        }
    }

    bool onRightEdge()
    {
        Transform landPos = GameObject.Find("RowGroup : " + (transform.position.y - 1).ToString()).transform;
        if (transform.position.y % 2 != 0 &&
            (transform.position.x == landPos.GetChild(0).position.x && transform.position.z == landPos.GetChild(0).position.z))
        {
            return true;
        }
        return false;
    }

    bool onLeftEdge()
    {
        Transform landPos = GameObject.Find("RowGroup : " + (transform.position.y - 1).ToString()).transform;
        if (transform.position.y % 2 != 0 &&
            (transform.position.x == landPos.GetChild(landPos.childCount - 1).position.x && transform.position.z == landPos.GetChild(landPos.childCount - 1).position.z))
        {
            return true;
        }
        return false;
    }

    public void TouchSlipperyLand(SlippyLand.Direction dir)
    {
        isMoving = true;
        if (!isJumpToAvoidSlippy)
        {
            if (dir == SlippyLand.Direction.left)
            {
                nextPos = new Vector3((int)transform.position.x + 1, (int)transform.position.y - 1, (int)transform.position.z);
            }
            else if (dir == SlippyLand.Direction.right)
            {
                nextPos = new Vector3((int)transform.position.x, (int)transform.position.y - 1, (int)transform.position.z + 1);
            }
        }
    }

    private bool hasbeenCheckSlippy = false;
    void CheckObstacle()
    {
        if (state == PlayerState.LIVE) {
            RaycastHit[] hit;
            //Physics.Raycast(placeToRaycast.position, Vector3.down, out hit, 1.2f, obstacleLayer);
            hit = Physics.RaycastAll(placeToRaycast.position, Vector3.down, 1.2f, obstacleLayer);
            foreach (RaycastHit h in hit)
            {
                if (h.collider != null)
                {
                    if (h.collider.tag.Equals("Poison"))
                    {
                        reverseControl = true;
                        poisonEffect.Play();
                        counterReverse = 10;
                    }
                    else if (h.collider.tag.Equals("TNT"))
                    {
                        h.collider.gameObject.GetComponent<TNT>().countDownExplode = true;
                    }
                    else if (h.collider.tag.Equals("SlippyLand"))
                    {
                        h.collider.gameObject.GetComponent<SlippyLand>().isTriggered = true;
                        h.collider.enabled = false;
                    }
                    else if (h.collider.tag.Equals("Coin"))
                    {
                        h.collider.gameObject.GetComponentInChildren<ParticleSystem>().Emit(25);
                        h.collider.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
                        h.collider.enabled = false;
                        SoundManager.instance.RandomSingle(coinSounds);
                        SaveLoad.savedGame.coins += 1;
                        if (SaveLoad.savedGame.coins >= 50)
                            Services.Instance.UnlockAchievement(Configuration.instance.diamond50Achievement);
                        if (SaveLoad.savedGame.coins >= 100)
                            Services.Instance.UnlockAchievement(Configuration.instance.diamond100Achievement);
                        if (SaveLoad.savedGame.coins >= 250)
                            Services.Instance.UnlockAchievement(Configuration.instance.diamond250Achievement);
                    }
                    else if (h.collider.tag.Equals("Spike"))
                    {
                        Die();
                    }
                    else if (h.collider.tag.Equals("Enemy"))
                    {
                        Die();
                    }
                }
            }
        }
    }

    public void Die()
    {
        explodeEffect.Emit(50);
        transform.GetChild(0).gameObject.SetActive(false);
        GameManager.instance.gameState = GameManager.GameState.Gameover;
        state = PlayerState.DIED;
        UICanvas.instance.anim.SetTrigger("GameoverFadeIn");
        hasFirstMove = false;
        transform.GetChild(0).gameObject.SetActive(false);
        SaveLoad.SaveGame();
        Services.Instance.ReportScore();
        if (GameManager.instance.score >= 100)
            Services.Instance.UnlockAchievement(Configuration.instance.score100Achievement);
        if (GameManager.instance.score >= 300)
            Services.Instance.UnlockAchievement(Configuration.instance.score300Achievement);
        if (GameManager.instance.score >= 500)
            Services.Instance.UnlockAchievement(Configuration.instance.score500Achievement);
        Services.Instance.UnlockAchievement(Configuration.instance.firstPlayAchievement);
        Services.Instance.IncrementAchievement(Configuration.instance.play100TimesAchievement,1);
        Services.Instance.IncrementAchievement(Configuration.instance.play250TimesAchievement,1);
        Services.Instance.IncrementAchievement(Configuration.instance.play25TimesAchievement,1);
        Services.Instance.IncrementAchievement(Configuration.instance.play50TimesAchievement,1);
        Services.Instance.FlushAchievements();
    }
}
