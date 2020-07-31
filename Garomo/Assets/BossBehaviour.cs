using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public bool attack = false;
    public Animator animator;

    bool inScreen = true;
    public float attackTime;
    public float attackTimeMax;

    public int life;

    public EnemyManager em;

    public SignBehaviour sign;

    public int phase = 1;

    public GameObject bossPunchCollider2D;

    public BoxCollider[] bossPunchColliders3D;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        sign.StartFight += ActivateBoss;
        attackTime = attackTimeMax;
        inScreen = false;
        BossPunchBehaviour.HitEnemy = GetDamage; 
    }

    private void OnDestroy()
    {
        sign.StartFight -= ActivateBoss;
    }

    // Update is called once per frame
    void Update()
    {
        if(inScreen)
        {
            attackTime -= Time.deltaTime;
            if(attackTime <= 0.0f)
            {
                attack = true;
                attackTime = attackTimeMax;
            }
        }

        if(attack)
        {
            animator.SetTrigger("Attack");
            attack = false;
        }
    }

    public void ActivateBoss()
    {
        animator.enabled = true;
        inScreen = true;
    }

    public void SpawnCollider(int hand)
    {
        Vector3 pos = bossPunchColliders3D[hand].transform.position;
        pos.z = 0.0f;
        bossPunchCollider2D.transform.position = pos;
        bossPunchCollider2D.SetActive(true);
    }

    public void EraseCollider()
    {
        bossPunchCollider2D.SetActive(false);
    }

    public void GetDamage()
    {
        if(life < 0.0f)
        {

        }
        else
        {
            life -= 2;
        }
    }
}
