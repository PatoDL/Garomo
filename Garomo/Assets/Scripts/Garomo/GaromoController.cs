using UnityEngine;
using System.Collections;
using Prime31;


public class GaromoController : MonoBehaviour
{
    public delegate void OnGaromoDeath();
    public static OnGaromoDeath GaromoDie;

	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

    public int life = 100;

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

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
        idleCollider = _controller.boxCollider;
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerStayEvent += onTriggerStayEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;

        Time.timeScale = 0f;

        startPos = transform.position;

        lastCheckpoint = new GameObject();

        lastCheckpoint.transform.position = startPos;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

       

        //if((hit.transform.tag == "Enemy" || hit.transform.tag == "Obstacle") && !immunity)
        //{
        //    enemyCollision = immunity = true;

        //}

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


	void onTriggerEnterEvent( Collider2D col )
	{
        if(col.tag=="Enemy" && !immunity)
            enemyCollision = immunity = true;

        if (col.transform.tag != "SewTile" && _controller.boxCollider == skinnyGaromo)
        {
            _controller.boxCollider = idleCollider;
            _controller.recalculateDistanceBetweenRays();
        }
        else if (col.transform.tag == "LimitTrigger")
        {
            if (life <= 0)
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
        else if(col.transform.tag=="Checkpoint")
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
            transform.position = teleporter.transform.position;
        }
    }

    void onTriggerStayEvent(Collider2D col)
    {
        if (col.transform.tag == "SewTile" && _controller.boxCollider != skinnyGaromo)
        {
            _controller.boxCollider = skinnyGaromo;
            _controller.recalculateDistanceBetweenRays();
        }
    }

    void onTriggerExitEvent( Collider2D col )
	{
        if (col.transform.tag == "SewTile" && _controller.boxCollider == skinnyGaromo)
        {
            _controller.boxCollider = idleCollider;
            _controller.recalculateDistanceBetweenRays();
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
                {
                    normalizedHorizontalSpeed = 1;
                    if (transform.localScale.x < 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                    if (_controller.isGrounded)
                        _animator.SetBool("Running", true);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    normalizedHorizontalSpeed = -1;
                    if (transform.localScale.x > 0f)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                    if (_controller.isGrounded)
                        _animator.SetBool("Running", true);
                }
                else
                {
                    normalizedHorizontalSpeed = 0;
                    _animator.SetBool("Running", false);
                }
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0;
            _animator.SetBool("Running", false);
        }

        if(Input.GetKeyDown(KeyCode.Z) && canMove && !isRolling)
        {
            _animator.SetTrigger("Punch");
        }

		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) && canMove )
		{
            Jump();
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Running", false);
        }

        if(!_controller.isGrounded && _controller.nearFloor)
        {
            _animator.SetBool("Jumping", false);
            _animator.SetBool("Falling", true);
            _animator.SetBool("Running", false);
        }

        if(enemyCollision)
        {
            if (life <= 0f)
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

        if (Input.GetKeyDown(KeyCode.C) && canMove && _controller.isGrounded && !isRolling)
        {
            isRolling = true;
            _controller.ignoreSlopeModifier = true;
            _animator.SetTrigger("Roll");
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
            }
        }

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        if (!isRolling && canMove)
        {
            var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
        }

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow) && canMove)
        {
            if (_controller.boxCollider != crouchCollider)
            {
                ActiveCrouchCollider();
                _animator.SetBool("Crouching", true);
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                if (_controller.boxCollider != idleCollider)
                {
                    DeActiveCrouchCollider();
                    _animator.SetBool("Jumping", true);
                    _animator.SetBool("Crouching", false);
                    _animator.SetBool("Jumping", false);
                }
            }
		}

        if(Input.GetKeyDown(KeyCode.G))
        {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (_controller.boxCollider != idleCollider)
            {
                DeActiveCrouchCollider();
                _animator.SetBool("Crouching", false);
            }
        }

        _controller.move( _velocity * Time.deltaTime );

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
	}

    public void Restart()
    {
        transform.position = startPos;
        life = 5;
        canMove = true;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Trampoline")
        {
            _velocity.y = Mathf.Sqrt(8f * jumpHeight * -gravity);
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Running", false);
            _controller.move(_velocity * Time.deltaTime);
        }
        
        else if (col.transform.tag == "EndlevelTrigger")
        {
            Debug.Log("Ganaste");
        }
    }

    public void ActiveCrouchCollider()
    {
        crouchCollider.gameObject.SetActive(true);
        _controller.boxCollider = crouchCollider;
        _controller.recalculateDistanceBetweenRays();
    }

    public void DeActiveCrouchCollider()
    {
        crouchCollider.gameObject.SetActive(false);
        _controller.boxCollider = idleCollider;
        _controller.recalculateDistanceBetweenRays();
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

    public void Roll()
    {
        float rollSpeedAux = rollSpeed;
        if(!_controller.isGrounded)
        {
            rollSpeedAux *= 1.5f;
        }

        _velocity.x += rollSpeedAux * transform.localScale.x * rollVelVariation.Evaluate((rollDistance - rollTimer) / rollDistance) * Time.deltaTime;
    }

    public void Recoil()
    {
        _velocity.x -= transform.localScale.x * recoil * Time.deltaTime;
    }
}
