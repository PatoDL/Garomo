using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GaromoController : MonoBehaviour
{
    public delegate void OnGaromoDeath();
    public static OnGaromoDeath GaromoDie;

    public delegate void OnLevelEnd();
    public static OnLevelEnd GaromoWin;

    public delegate void OnLevelPass(int livesAm);
    public static OnLevelPass GoToNext;

	[Header("Movement")]
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
    public bool canMove = true;
    public AnimationCurve runVelocityModifier;
    public float runModifierMultiplier;
    public bool gravityAct = true;

    [Header("Player Attributes")]
    public int life = 100;
    public bool win = false;
    public Sprite garomoFalling;
    public BoxCollider2D skinnyGaromo;
    int maxLives;
    private Vector3 _velocity;
    SpriteRenderer spr;
    GameObject lastCheckpoint;

    [Header("Enemy")]
    public bool enemyCollision = false;
    public bool isRecoiling = false;
    public float recoilTimeMax = 3.0f;
    public float recoilTime = 3.0f;
    public float recoil = 0f;
    private bool immunity = false;

    [Header("Roll")]
    public float rollTimer = 0.0f;
    public float rollSpeed = 0f;
    public bool rollJump = false;
    public bool isRolling = false;
    public float rollDistance = 0f;
    public AnimationCurve rollVelVariation;

    [Header("Crouch")]
    public BoxCollider2D crouchCollider;
    public BoxCollider2D idleCollider;

    [Header("Punch")]
    public BoxCollider2D GroundAttackCollider;
    public BoxCollider2D AirAttackCollider;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    private CharacterController2Di _controller;
    public Animator _animator;


    void Awake()
	{
        if (!UIController.Instance.garomoController)
        {
            UIController.Instance.garomoController = this as GaromoController;
        }

        _animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2Di>();
        idleCollider = _controller.boxCollider;
        spr = GetComponent<SpriteRenderer>();
		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerStayEvent += onTriggerStayEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
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
        if (col.tag == "Enemy" && !immunity)
        {
            enemyCollision = immunity = true;
            runModifierMultiplier = 0f;
            isRolling = canMove = false;

            if (GameManager.Instance.soundOn)
                AkSoundEngine.PostEvent("Garomo_hurt", gameObject);

            if(col.GetComponentInParent<TurtleController>() && GameManager.Instance.soundOn)
            {
                AkSoundEngine.PostEvent("Turtle_Punch_Hit", gameObject);
            }
        }

        if (col.transform.tag != "SewTile" && _controller.boxCollider == skinnyGaromo)
        {
            _controller.boxCollider = idleCollider;
            _controller.recalculateDistanceBetweenRays();
            skinnyGaromo.gameObject.SetActive(false);
        }

        if (col.transform.tag == "LimitTrigger")
        {
            GetDamage(true);
        }

        if(col.transform.tag == "NextScene")
        {
            GoToNext(life);
        }

        if(col.tag == "Potion")
        {
            life = maxLives;
            if (GameManager.Instance.soundOn)
                AkSoundEngine.PostEvent("Life_Potion", gameObject);
        }

        if (col.transform.tag == "Trampoline")
        {
            if (GameManager.Instance.soundOn)
                AkSoundEngine.PostEvent("Trampoline", gameObject);
            Jump(true);
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Running", false);
           _controller.move(_velocity * Time.deltaTime);
        }

        if (col.transform.tag == "EndlevelTrigger")
        {
            win = true;
            GaromoWin();
            CheckPointManager.instance.RestartLevel();
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

    bool isRestarting = false;

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
                    runModifierMultiplier += Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    normalizedHorizontalSpeed = -1;
                    runModifierMultiplier += Time.deltaTime;
                }
                else
                {
                    //if (runModifierMultiplier > 0f)
                    //    runModifierMultiplier -= Time.deltaTime;
                    //else
                        normalizedHorizontalSpeed = 0;
                }
                if (runModifierMultiplier > 0.5f)
                    runModifierMultiplier = 0.5f;
            }
        }

        if (!isRolling && canMove)
        {
            Walk();
        }


        _animator.SetFloat("Yvel", _velocity.y);


        if (Input.GetKeyDown(KeyCode.Z) && canMove && !isRolling)
        {
            _animator.SetTrigger("Punch");
        }

		// we can only jump whilst grounded
		if( _controller.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) && canMove )
		{
            Jump(false);
            if (GameManager.Instance.soundOn)
            {
                if (isRolling)
                    AkSoundEngine.PostEvent("Garomo_roll_jump", gameObject);
                else
                    AkSoundEngine.PostEvent("Garomo_Jump", gameObject);
            }
        }

        if(enemyCollision)
        {
            GetDamage(false);
            _animator.SetTrigger("Damage");
            isRecoiling = true;
            enemyCollision = false;
            _velocity = Vector3.zero;
            canMove = false;
            recoilTime = recoilTimeMax;
        }

        if(isRecoiling)
        {
            Recoil();
        }

        if (Input.GetKeyDown(KeyCode.C) && canMove && !isRolling && _controller.isGrounded)
        {
            isRolling = true;
            //_controller.ignoreSlopeModifier = true;
            _animator.SetBool("Running", false);
            _animator.Play("Garomo_roll");
            rollTimer = rollDistance;
            _velocity = Vector3.zero;
            if(GameManager.Instance.soundOn)
                AkSoundEngine.PostEvent("Garomo_roll", gameObject);
        }

        if (isRolling)
        {
            Roll();
        }

        // apply gravity before moving
        if(gravityAct)
            ApplyGravity();

        if (!_controller.collisionState.wasGroundedLastFrame && _controller.isGrounded && GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Garomo_Land", gameObject);

        if(isRestarting)
        {
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            life = maxLives;
            canMove = true;
            isRolling = false;
            enemyCollision = false;
            immunity = false;
            win = false;
            _velocity = Vector3.zero;
            transform.position = (Vector2)CheckPointManager.instance.GetLastCheckPoint().transform.position;

            isRestarting = false;
        }

        _controller.move( _velocity * Time.deltaTime );

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
	}

    public void Restart()
    {
        isRestarting = true;
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
    }

    public void Jump(bool trampoline)
    {
        //_velocity.y = jumpHeight * multiplier * -gravity;

        float finalJump = jumpHeight;

        if (trampoline)
            finalJump *= 1.5f;

        _velocity.y = Mathf.Sqrt(2f * finalJump * -gravity);
        _animator.SetBool("Jumping", true);
        _animator.SetBool("Running", false);
        
        if (isRolling)
        {
            rollJump = true;
        }

    }

    public void TeleportTo(Transform t)
    {
        transform.position = t.position;
    }

    public void ActivePunch(int air)
    {
        if(air == 0)
            GroundAttackCollider.gameObject.SetActive(true);
        else
            AirAttackCollider.gameObject.SetActive(true);

        if (GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Garomo_Punch", gameObject);
    }

    public void DeActivePunch(int air)
    {
        if (air == 0)
            GroundAttackCollider.gameObject.SetActive(false);
        else
            AirAttackCollider.gameObject.SetActive(false);
    }

    void Walk()
    {
        if (_controller.isGrounded)
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
        //_velocity.x = normalizedHorizontalSpeed * runSpeed /** runVelocityModifier.Evaluate(runModifierMultiplier)*/;
    }

    public void Roll()
    {
        rollTimer -= Time.deltaTime;
        float rollSpeedAux = rollSpeed;

        if (!rollJump && !_controller.isGrounded)
        {
            _velocity.y = gravity * 10 * Time.deltaTime;
            rollSpeedAux *= 1.3f;
        }
        else if(rollJump)
        {
            rollSpeedAux *=  1.5f;
        }

        _velocity.x = rollSpeedAux * transform.localScale.x * rollVelVariation.Evaluate((rollDistance - rollTimer) / rollDistance) * Time.deltaTime;

        if (rollTimer <= 0.0f)
        {
            isRolling = false;
            //_controller.ignoreSlopeModifier = false;
            rollTimer = rollDistance;
            rollJump = false;
        }
    }

    public void Recoil()
    {
        recoilTime -= Time.deltaTime;
        _velocity.x = transform.localScale.x * recoil * Time.deltaTime;
        if (recoilTime <= 0f)
        {
            isRecoiling = false;
            canMove = true;
            recoilTime = recoilTimeMax;
            immunity = false;
        }
    }

    void GetDamage(bool returnToCheck)
    {
        if(life <= 1)
        {
            GaromoDie();
            returnToCheck = true;
        }
        else
        {
            life -= 1;
        }

        if(returnToCheck)
        {
            transform.position = (Vector2)CheckPointManager.instance.GetLastCheckPoint().transform.position;
        }

        //canMove = false;
        _velocity = Vector3.zero;
    }

    public void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
    }
}
