using UnityEngine;
using System.Collections;
using Prime31;
using System.Collections.Generic;


public class TurtleController : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

    public float attackTimeMax;
    public float attackTimer;
    public bool attacked = false;

    public int life = 100;

    public float recoil = 0f;

    public float recoilTime;
    public float recoilTimeMax;

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

    public Sprite deadTurtle;
    bool falling = false;

    public float fallingTime;
    public float fallingMaxTime = 3f;

    public AnimationCurve deadAnim;

    Vector3 startPos;

    public static List<GameObject> turtles = new List<GameObject>();

    public static void RestartTurtles()
    {
        foreach(GameObject g in turtles)
        {
            g.SetActive(true);
            g.GetComponent<TurtleController>().Restart();
        }
    }

    public void Restart()
    {
        transform.position = startPos;
        life = 3;
        _animator.enabled = true;
        _controller.boxCollider.enabled = true;
        _controller.rigidBody2D.WakeUp();
        falling = false;
        wasDamaged = false;
    }

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
        recoilTime = recoilTimeMax;
        gc = GetComponentInChildren<GaromoChecker>();
        gc.GaromoEntrance = Attack;
        startPos = transform.position;
        turtles.Add(gameObject);

        normalizedHorizontalSpeed = -1;

        attackTimer = attackTimeMax;
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
            if (life <= 1)
            {
                GetComponent<SpriteRenderer>().sprite = deadTurtle;
                _animator.enabled = false;
                _controller.boxCollider.enabled = false;
                _controller.rigidBody2D.Sleep();
                falling = true;
                fallingTime = fallingMaxTime;
            }
            else
            {
                life -= 1;
                if ((col.transform.position.x > transform.position.x && normalizedHorizontalSpeed == 1) || 
                    (col.transform.position.x < transform.position.x && normalizedHorizontalSpeed == -1))
                {
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    
                }
                _animator.SetTrigger("Damage");
                wasDamaged = true;
                recoilTime = recoilTimeMax;
            }
        }
        else if(col.tag=="Redirectioner")
        {
            normalizedHorizontalSpeed= -normalizedHorizontalSpeed;
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

        if (falling)
        {
            fallingTime -= Time.deltaTime;
            transform.position += new Vector3(- normalizedHorizontalSpeed * (fallingMaxTime - fallingTime)*runSpeed, deadAnim.Evaluate((fallingMaxTime - fallingTime) / fallingMaxTime)*runSpeed)*Time.deltaTime;

            if(fallingTime<=0f)
            {
                gameObject.SetActive(false);
            }
        }

        if(wasDamaged)
        {
            recoilTime -= Time.deltaTime;
            Recoil();
            if(recoilTime<=0)
            {
                wasDamaged = false;
                recoilTime = recoilTimeMax;
                normalizedHorizontalSpeed = -normalizedHorizontalSpeed;
            }
        }

        if (!falling && !wasDamaged)
        {
            // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
            var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

            // apply gravity before moving
            _velocity.y += gravity * Time.deltaTime;

            _controller.move(_velocity * Time.deltaTime);
        }

        if(attacked)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                attacked = false;
                attackTimer = attackTimeMax;
            }
        }

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LimitTrigger")
            gameObject.SetActive(false);
    }

    int whereToAttack;

    void Attack()
    {
        if (!wasDamaged && !attacked)
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
        attacked = true;
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

    void Recoil()
    {
        _velocity.x -= transform.localScale.x * recoil * Time.deltaTime;
    }
}
