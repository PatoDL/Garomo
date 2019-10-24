using UnityEngine;
using System.Collections;
using Prime31;


public class TurtleController : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

    public int life = 100;

    public float recoil = 0f;

    public bool haveToAttack = false;

    public bool wasDamaged = false;

	[HideInInspector]
	private float normalizedHorizontalSpeed = -1;

	private CharacterController2D _controller;
	public Animator _animator;
	private Vector3 _velocity;

    public Vector3 direction;

    private GaromoChecker gc;

    public BoxCollider2D upAttackCollider;
    public BoxCollider2D downAttackCollider;

    public BoxCollider2D idleCollider;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
        idleCollider = _controller.boxCollider;
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
        _controller.collisionState.right = true;

        gc = GetComponentInChildren<GaromoChecker>();
        gc.GaromoEntrance = Attack;

        normalizedHorizontalSpeed = -1;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent( Collider2D col )
	{
        if (col.tag == "Attack")
        {
            wasDamaged = true;
            Debug.Log("ouch");
        }
		//Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		//Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion
	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
        if (normalizedHorizontalSpeed==-1)
        {
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        }
        else if (normalizedHorizontalSpeed==1)
        {
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if (_controller.collidedLeft)
            normalizedHorizontalSpeed = 1;
        else if (_controller.collidedRight)
            normalizedHorizontalSpeed = -1;

        if (wasDamaged)
        {
            life -= 1;
            _animator.SetTrigger("Damage");
            wasDamaged = false;
        }

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms

        _controller.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

    public void Restart()
    {
        transform.position = new Vector3(-380f,-1f,0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LimitTrigger")
            gameObject.SetActive(false);
    }

    int whereToAttack;

    void Attack()
    {
        if (!wasDamaged)
        {
            whereToAttack = Random.Range(0, 2);
            _animator.SetTrigger("Attack");
            if (whereToAttack == 0)
            {
                _animator.SetBool("HighAttack", true);
            }
            else
            {
                _animator.SetBool("HighAttack", false);
            }

            
        }
        runSpeed = 0f;
    }

    void ActivateAttack(int where)
    {
        if (where == 0)
        {
            upAttackCollider.gameObject.SetActive(true);
        }
        else
        {
            downAttackCollider.gameObject.SetActive(true);
        }

        Invoke("DeActivateAttacks", 0.5f);
    }

    void DeActivateAttacks()
    {
        upAttackCollider.gameObject.SetActive(false);
        downAttackCollider.gameObject.SetActive(false);
        normalizedHorizontalSpeed = -normalizedHorizontalSpeed;
        runSpeed = 2.5f;
    }
}
