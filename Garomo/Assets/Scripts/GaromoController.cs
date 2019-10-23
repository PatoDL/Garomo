using UnityEngine;
using System.Collections;
using Prime31;


public class GaromoController : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

    public int life = 100;

    public float recoil = 0f;

    public float rollVel = 0f;

    int recoiled = 0;

    bool enemyCollidedlastFrame = false;
    bool enemyCollision = false;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	public Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

    bool rolled = false;
    float rollTimer = 0.0f;

    private bool immunity = false;
    private float enemyCollisionTimer = 0.0f;

    public BoxCollider2D crouchCollider;
    public BoxCollider2D idleCollider;
    bool canMove = true;

    Vector3 startPos;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
        idleCollider = _controller.boxCollider;
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

        startPos = transform.position;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

        if((hit.transform.tag == "Enemy" || hit.transform.tag == "Obstacle") && !immunity)
        {
            enemyCollision = immunity = true;
            
        }

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


	void onTriggerEnterEvent( Collider2D col )
	{
        if(col.tag=="Enemy")
            enemyCollision = immunity = true;
    }


	void onTriggerExitEvent( Collider2D col )
	{
		//Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
        if(immunity)
        {
            enemyCollisionTimer += Time.deltaTime;
            if(enemyCollisionTimer>1.5f)
            {
                immunity = false;
                enemyCollisionTimer = 0.0f;
            }
        }

        if(rolled)
        {
            rollTimer += Time.deltaTime;
            if(rollTimer>1.5f)
            {
                rollTimer = 0f;
                rolled = false;
            }
        }

        if (_controller.isGrounded)
        {
            _velocity.y = 0;
            _animator.SetBool("Jumping", false);
        }
        
        if (canMove)
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
        else
        {
            normalizedHorizontalSpeed = 0;
            _animator.SetBool("Running", false);
            if(!_controller.isGrounded && recoil>25)
            {
                if (_velocity.x > 0)
                    _velocity.x = 1;
                else
                    _velocity.x = -1;

            }
        }

		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) )
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
            _animator.SetTrigger("Damage");
            enemyCollision = false;
            enemyCollidedlastFrame = true;

            if (_controller.isGrounded)
                recoil = 250;
            else
                recoil = 25;

            if (_controller.collidedLeft)
            {
                _velocity.x += recoil;
                //Debug.Log("left");
            }
            else if(_controller.collidedRight)
            {
                _velocity.x -= recoil;
                //Debug.Log("right");
            }
            canMove = false;
            Invoke("ValidateMovement",0.5f);
            //Debug.Log(_controller.collidedLeft);
           // Debug.Log(_controller.collidedRight);
        }

      //  Debug.Log(_velocity.x);

        //if(!rolled && Input.GetKey(KeyCode.LeftControl))
        //{
        //    _animator.SetTrigger("Roll");
        //    rolled = true;

        //    if (transform.localScale.x > 0f)
        //    {
        //        _velocity.x += rollVel;
        //    }
        //    else if(transform.localScale.x < 0f)
        //    {
        //        _velocity.x -= rollVel;
        //    }
        //}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            if (_controller.boxCollider != crouchCollider)
            { 
                _controller.boxCollider = crouchCollider;
                _controller.recalculateDistanceBetweenRays();
                _animator.SetBool("Crouching", true);
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                if (_controller.boxCollider != idleCollider)
                {
                    _controller.boxCollider = idleCollider;
                    _controller.recalculateDistanceBetweenRays();
                    _animator.SetBool("Jumping", true);
                    _animator.SetBool("Crouching", false);
                    _animator.SetBool("Jumping", false);
                }
            }
		}

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (_controller.boxCollider != idleCollider)
            {
                _controller.boxCollider = idleCollider;
                _controller.recalculateDistanceBetweenRays();
                _animator.SetBool("Crouching", false);
            }
        }

        _controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

    public void Restart()
    {
        transform.position = new Vector3(-380f,-1f,0f);
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
        else if(col.transform.tag == "LimitTrigger")
        {
            transform.position = startPos;
            _velocity.y = 0;
            _controller.move(_velocity * Time.deltaTime);
        }
        else if (col.transform.tag == "EndlevelTrigger")
        {
            Debug.Log("Ganaste");
        }
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
}
