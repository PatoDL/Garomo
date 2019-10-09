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
    int recoiled = 0;

    bool enemyCollidedlastFrame = false;
    bool enemyCollision = false;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	public Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

    public BoxCollider2D crouchCollider;

    public BoxCollider2D idleCollider;

    bool canMove = true;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
        idleCollider = _controller.boxCollider;
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

        if(hit.transform.tag == "Enemy" && !enemyCollidedlastFrame)
        {
            enemyCollision = true;
        }
        else if(enemyCollidedlastFrame)
        {
            enemyCollidedlastFrame = false;
        }


        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


	void onTriggerEnterEvent( Collider2D col )
	{
        //Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
    }


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
        if (!IsInvoking())
        {
            if (_controller.isGrounded)
            {
                _velocity.y = 0;
                _animator.SetBool("Jumping", false);
                _animator.SetBool("Falling", false);
            }
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
        }

		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) )
		{
            //Invoke("Jump", 0.1f);
            Jump();
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Running", false);
        }

        if(!_controller.isGrounded && /*_velocity.y > 0 &&*/ _controller.nearFloor)
        {
            _animator.SetBool("Falling", true);
        }

        if(enemyCollision)
        {
            _animator.SetTrigger("Damage");
            enemyCollision = false;
            enemyCollidedlastFrame = true;
            if(_controller.collidedLeft)
            {
                _velocity.x += recoil;
            }
            else if(_controller.collidedRight)
            {
                _velocity.x -= recoil;
            }
            Debug.Log("recoil");
            canMove = false;
            Invoke("ValidateMovement",0.5f);
        }

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            _velocity.y *= 3f;
            //_controller.ignoreOneWayPlatformsThisFrame = true;
            if (_controller.boxCollider != crouchCollider)
            { 
                _controller.boxCollider = crouchCollider;
                _controller.recalculateDistanceBetweenRays();
                _animator.SetBool("Crouching", true);
            }
		}

        if(_controller.isGrounded && Input.GetKeyUp(KeyCode.DownArrow))
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
            _controller.move(_velocity * Time.deltaTime);
        }
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(4f * jumpHeight * -gravity);
        Debug.Log("jump");
    }

    void ValidateMovement()
    {
        canMove = true;
    }
}
