using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine.SceneManagement;


public class GaromoController : MonoBehaviour
{
    public delegate void OnGaromoDeath();
    public static OnGaromoDeath GaromoDie;

    public delegate void OnLevelEnd();
    public static OnLevelEnd GaromoWin;

    public delegate void OnLevelPass(int livesAm);
    public static OnLevelPass GoToNext;

    public bool win = false;

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

    public int life = 100;
    int maxLives;

    public bool gravityAct = true;

    public float recoil = 0f;

    public float rollSpeed = 0f;

    bool enemyCollision = false;
    bool isRecoiling = false;

    public float recoilTimeMax = 3.0f;
    public float recoilTime = 3.0f;

    [HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	public Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

    float rollTimer = 0.0f;

    private bool immunity = false;
    private float enemyCollisionTimer = 0.0f;

    public BoxCollider2D crouchCollider;
    public BoxCollider2D idleCollider;
    public BoxCollider2D GroundAttackCollider;
    public BoxCollider2D AirAttackCollider;
    bool canMove = true;

    public BoxCollider2D skinnyGaromo;

    public AnimationCurve rollVelVariation;

    public bool isRolling = false;
    public float rollDistance = 0f;

    Vector3 startPos;

    GameObject lastCheckpoint;

    public GameObject teleporter;

    public bool rollJump = false;

    public bool isCrouching = false;

    public Sprite garomoFalling;

    SpriteRenderer spr;

    void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
        idleCollider = _controller.boxCollider;
        spr = GetComponent<SpriteRenderer>();
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerStayEvent += onTriggerStayEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;

        startPos = transform.position;

        lastCheckpoint = new GameObject();

        lastCheckpoint.transform.position = startPos;
        maxLives = life;

        gravityAct = true;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		if( hit.normal.y == 1f )
			return;
    }


	void onTriggerEnterEvent( Collider2D col )
	{
        if(col.tag=="Enemy" && !immunity)
            enemyCollision = immunity = true;

        if (col.transform.tag != "SewTile" && _controller.boxCollider == skinnyGaromo)
        {
            _controller.boxCollider = idleCollider;
            _controller.recalculateDistanceBetweenRays();
            skinnyGaromo.gameObject.SetActive(false);
        }

        if (col.transform.tag == "LimitTrigger")
        {
            if (life <= 1)
            {
                _animator.SetTrigger("Dead");
                canMove = false;
                _velocity = Vector3.zero;
                transform.position = startPos;
                GaromoDie();
            }
            else
            {
                life -= 1;
                transform.position = (Vector2)lastCheckpoint.transform.position;
            }
        }

        if (col.transform.tag=="Checkpoint")
        {
            if (lastCheckpoint.transform.position == startPos)
            {
                Destroy(lastCheckpoint);
                lastCheckpoint = null;
            }
            lastCheckpoint = col.transform.gameObject;
        }

        if(col.transform.tag == "NextScene")
        {
            //if(GoToNext != null)
                GoToNext(life);
            Debug.Log("pasa");
        }

        if(col.tag == "Potion")
        {
            life = maxLives;
        }

        if (col.transform.tag == "Trampoline")
        {
            _velocity.y = Mathf.Sqrt(8f * jumpHeight * -gravity);
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Running", false);
            _controller.move(_velocity * Time.deltaTime);
        }

        if (col.transform.tag == "EndlevelTrigger")
        {
            win = true;
            GaromoWin();
            lastCheckpoint.transform.position = startPos;
        }
    }

    void onTriggerStayEvent(Collider2D col)
    {
        if (col.transform.tag == "SewTile" && _controller.boxCollider != skinnyGaromo)
        {
            skinnyGaromo.gameObject.SetActive(true);
            _controller.boxCollider = skinnyGaromo;
            _controller.recalculateDistanceBetweenRays();
        }
        else if (col.tag == "Enemy" && !immunity)
            enemyCollision = immunity = true;
    }

    void onTriggerExitEvent( Collider2D col )
	{
        if (col.transform.tag == "SewTile" && _controller.boxCollider == skinnyGaromo)
        {
            _controller.boxCollider = idleCollider;
            _controller.recalculateDistanceBetweenRays();
            skinnyGaromo.gameObject.SetActive(false);
        }
    }

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
        if (_controller.isGrounded)
        {
            _velocity.y = 0;
            _animator.SetBool("Jumping", false);
        }
        
        if (canMove)
        {
            if (Time.timeScale > 0f)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                    normalizedHorizontalSpeed = 1;
                else if (Input.GetKey(KeyCode.LeftArrow))
                    normalizedHorizontalSpeed = -1;        
                else
                    normalizedHorizontalSpeed = 0;
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0;
        }

        if (!isRolling && canMove)
        {
            Walk();
        }

        if (Input.GetKeyDown(KeyCode.Z) && canMove && !isRolling && !isCrouching && !(!_controller.isGrounded && _velocity.y < 0f))
        {
            _animator.SetTrigger("Punch");
        }

		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) && canMove )
		{
            Jump();
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Running", false);
            if(isRolling)
            {
                rollJump = true;
            }
        }

        if(!_controller.isGrounded && _controller.nearFloor)
        {
            _animator.SetBool("Jumping", false);
            _animator.SetBool("Falling", true);
            _animator.SetBool("Running", false);
        }

        if(enemyCollision)
        {
            if (life <= 1f)
            {
                _animator.SetTrigger("Dead");
                _velocity = Vector3.zero;
                GaromoDie();
            }
            else
            {
                life -= 1;
                _animator.SetTrigger("Damage");
                isRecoiling = true;
            }
            enemyCollision = false;
            _velocity = Vector3.zero;
            canMove = false;
            recoilTime = recoilTimeMax;
        }

        if(isRecoiling)
        {
            recoilTime -= Time.deltaTime;
            Recoil();
            if(recoilTime<=0f)
            {
                isRecoiling = false;
                canMove = true;
                recoilTime = recoilTimeMax;
                immunity = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && canMove && /*_controller.isGrounded && */ !isRolling && !isCrouching)
        {
            isRolling = true;
            _controller.ignoreSlopeModifier = true;
            _animator.SetBool("Running", false);
            _animator.Play("Garomo_roll");
            rollTimer = rollDistance;
        }

        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            Roll();
            if (rollTimer <= 0.0f)
            {
                isRolling = false;
                _controller.ignoreSlopeModifier = false;
                rollTimer = rollDistance;
                rollJump = false;
            }
        }

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control


        // apply gravity before moving
        if(gravityAct)
            ApplyGravity();

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        //if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow) && canMove)
        //{
            //if (_controller.boxCollider != crouchCollider)
            //{
                //CrouchColliderActivation("Active");
                //_animator.SetBool("Crouching", true);
                //isCrouching = true;
            //}
            //if(Input.GetKey(KeyCode.UpArrow))
            //{
                //if (_controller.boxCollider != idleCollider)
                //{
                    //CrouchColliderActivation("Deactive");
                    //_animator.SetBool("Jumping", true);
                    //_animator.SetBool("Crouching", false);
                    //_animator.SetBool("Jumping", false);
                    //isCrouching = false;
                //}
            //}
		//}

        //if (Input.GetKeyUp(KeyCode.DownArrow))
        //{
            //if (_controller.boxCollider != idleCollider)
            //{
                //CrouchColliderActivation("Deactive");
                //_animator.SetBool("Crouching", false);
                //isCrouching = false;
            //}
        //}

        _controller.move( _velocity * Time.deltaTime );

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
	}

    public void Restart()
    {
        transform.position = startPos;
        if (transform.localScale.x < 0f)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        life = maxLives;
        canMove = true;
        isRolling = false;
        enemyCollision = false;
        immunity = false;
        win = false;
        _animator.SetTrigger("Restart");
        lastCheckpoint.transform.position = startPos;
    }

    public void CrouchColliderActivation(string action)
    {
        bool active = action == "Active";

        crouchCollider.gameObject.SetActive(active);
       
        idleCollider.enabled = !active;

        if(active)
            _controller.boxCollider = crouchCollider;
        else
            _controller.boxCollider = idleCollider;

        _controller.recalculateDistanceBetweenRays();

        Debug.Log(active);
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(4f * jumpHeight * -gravity);
    }

    void ValidateMovement()
    {
        canMove = true;
    }

    public void TeleportTo(Transform t)
    {
        transform.position = t.position;
    }

    public void ActivePunch()
    {
        GroundAttackCollider.gameObject.SetActive(true);
    }

    public void DeActivePunch()
    {
        GroundAttackCollider.gameObject.SetActive(false);
    }

    void Walk()
    {
        //if (_controller.isGrounded)
            _animator.SetBool("Running", true);

        if (normalizedHorizontalSpeed > 0f)
        {
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(normalizedHorizontalSpeed < 0f)
        {
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            _animator.SetBool("Running", false);
        }

        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
    }

    public void Roll()
    {
        float rollSpeedAux = rollSpeed;
        //if (!_controller.isGrounded)
        //{
        //    rollSpeedAux *= 1.5f;
        //}

        if (!rollJump)
        {
            _velocity.y = gravity * 10 * Time.deltaTime;
            rollSpeedAux *= 1.3f;
        }
        //if (!rollJump && !_controller.isGrounded)
        //    rollSpeedAux = 0f;

        _velocity.x += rollSpeedAux * transform.localScale.x * rollVelVariation.Evaluate((rollDistance - rollTimer) / rollDistance) * Time.deltaTime;
       
    }

    public void Recoil()
    {
        _velocity.x -= transform.localScale.x * recoil * Time.deltaTime;
    }

    public void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
    }
}
