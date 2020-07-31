using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class FoxBehaviour : MonoBehaviour
{
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

    [Header("Attributes")]
    private int maxLives;
    public int life;
    private Vector3 _velocity;
    public float normalizedHorizontalSpeed = -1;
    public bool wasDamaged = false;
    public float recoilTimeMax = 3.0f;
    public float recoilTime = 3.0f;
    public float recoil = 0f;
    public bool punching;

    private CharacterController2D _controller;
    public Animator _animator;
    public GameObject shadow;
    public GameObject punchCollider;

    [Header("Dead")]
    public Sprite deadFox;
    bool falling = false;
    public float fallingTime;
    public float fallingMaxTime = 3f;
    public bool punched;
    public float punchTime;
    public float punchTimeMax;

    public float runTime = 0.0f;
    public float runTimeMax;

    public GaromoChecker[] gc;

    enum State
    {
        idle, run, attack
    }

    State action = State.idle;

    public static event Action onFoxDeathEvent;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();

        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerStayEvent += onTriggerStayEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
        maxLives = life;

        _controller.boxCollider.enabled = true;
        _controller.rigidBody2D.WakeUp();

        for (int i=0;i<gc.Length;i++)
        {
            gc[i].gameObject.SetActive(true);
            gc[i].GaromoEntrance = Attack;
        }
        falling = false;
    }

    public void OnDisable()
    {
        onFoxDeathEvent();
    }

    #region Event Listeners

    Vector3 garomoPosition;

    void onControllerCollider(RaycastHit2D hit)
    {
        if (hit.normal.y == 1f)
            return;
    }


    void onTriggerEnterEvent(Collider2D col)
    {
        if (col.tag == "Attack")
        {
            if ((garomoPosition.x > transform.position.x && normalizedHorizontalSpeed == -1) ||
            (garomoPosition.x < transform.position.x && normalizedHorizontalSpeed == 1))
            {
                normalizedHorizontalSpeed = -normalizedHorizontalSpeed;
            }

            if (life <= 1)
            {
                GetComponent<SpriteRenderer>().sprite = deadFox;
                _animator.enabled = false;
                _controller.boxCollider.enabled = false;
                _controller.rigidBody2D.Sleep();
                falling = true;
                fallingTime = fallingMaxTime;
                for (int i = 0; i < gc.Length; i++)
                {
                    gc[i].gameObject.SetActive(false);
                    gc[i].GaromoEntrance = Attack;
                }
            }
            else
            {
                life -= 1;
                _animator.SetTrigger("Damaged");
                wasDamaged = true;
                canMove = false;
                recoilTime = recoilTimeMax;
                runTime = runTimeMax;
            }
        }
        else if (col.tag == "Redirectioner")
        {
            _animator.SetBool("Running", false);
            action = State.idle;
            if (wasDamaged)
            {
                _velocity = Vector3.zero;
                _controller.move(_velocity * Time.deltaTime);
            }
            else
            {
                normalizedHorizontalSpeed = -normalizedHorizontalSpeed;
            }
        }

        if (col.tag == "LimitTrigger")
            gameObject.SetActive(false);

        Debug.Log(col.name);
    }

    void onTriggerStayEvent(Collider2D col)
    {

    }

    void onTriggerExitEvent(Collider2D col)
    {

    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (punching)
            canMove = false;

        if (_controller.isGrounded)
        {
            _velocity.y = 0;
            if (!shadow.activeInHierarchy)
            {
                shadow.SetActive(true);
            }
        }
        else
        {
            if (shadow.activeInHierarchy)
            {
                shadow.SetActive(false);
            }
        }

        if (normalizedHorizontalSpeed == -1)
        {
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        
        }
        else if (normalizedHorizontalSpeed == 1)
        {
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        switch(action)
        {
            case State.idle:
                {
                    _animator.SetBool("Running", false);
                    runTime -= Time.deltaTime;
                    if(runTime <= 0.0f)
                    {
                        runTime = runTimeMax;
                        action = State.run;
                    }
                    break;
                }
            case State.run:
                {
                    if (canMove)
                    {
                        _animator.SetBool("Running", true);
                        Walk();
                    }
                    break;
                }
            case State.attack:
                {
                    if(punched)
                    {
                        action = State.idle;
                    }
                    break;
                }
        }

        if (wasDamaged)
        {
            Recoil();
        }

        if(punched)
        {
            punchTime -= Time.deltaTime;
            if(punchTime <= 0.0f)
            {
                punched = false;
                punchTime = punchTimeMax;
            }
        }

        if(falling)
        {
            fallingTime -= Time.deltaTime;
            transform.position += new Vector3(-normalizedHorizontalSpeed * runSpeed * 10, 0.0f) * Time.deltaTime;

            if (fallingTime <= 0f)
            {
                gameObject.SetActive(false);
                fallingTime = fallingMaxTime;
            }
        }

        // apply gravity before moving
        if (gravityAct)
            ApplyGravity();

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;

    }

    private void Recoil()
    {
        recoilTime -= Time.deltaTime;
        _velocity.x = -transform.localScale.x * recoil * Time.deltaTime;
        if (recoilTime <= 0f)
        {
            wasDamaged = false;
            canMove = true;
            recoilTime = recoilTimeMax;
        }
    }

    void Walk()
    {
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
    }

    

    public void Attack(GameObject garomo)
    {
        if(!wasDamaged)
        {
            garomoPosition = garomo.transform.position;
            _velocity.x = 0.0f;
            action = State.attack;
            if (!punched)
            {
                _animator.SetTrigger("Attack");
            }
        }
    }

    public void PunchOn()
    {
        punchCollider.SetActive(true);
        if ((garomoPosition.x > transform.position.x && normalizedHorizontalSpeed == -1) || (garomoPosition.x < transform.position.x && normalizedHorizontalSpeed == 1))
        {
            normalizedHorizontalSpeed = -normalizedHorizontalSpeed;
        }
    }

    public void PunchOff()
    {
        punchCollider.SetActive(false);
        punched = true;
    }

    public void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
    }
}
