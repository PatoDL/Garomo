using System.Collections;
using System;
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

    public int maxLife;

    List<GameObject> foxList;

    int foxCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        sign.StartFight += ActivateBoss;
        attackTime = attackTimeMax;
        inScreen = false;
        BossPunchBehaviour.HitEnemy = GetDamage;
        maxLife = life;
        if (EnemyManager.instance)
            foxList = EnemyManager.instance.GetList("Fox");

        foreach(GameObject g in foxList)
        {
            FoxBehaviour.onFoxDeathEvent += DeadFoxEvent;
        }
    }

    private void DeadFoxEvent()
    {
        inScreen = true;
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
            switch (phase)
            {
                case 1:
                    animator.SetTrigger("Attack");
                    attack = false;
                    break;
                case 3:
                    animator.SetTrigger("Attack");
                    animator.SetInteger("AttackNumber", 1);
                    attack = false;
                    break;
            }
        }

        animator.SetBool("InScene", inScreen);
    }

    public void ActivateBoss()
    {
        animator.enabled = true;
        inScreen = true;
    }

    public void SpawnCollider(int hand)
    {
        Vector3 pos = bossPunchColliders3D[hand].bounds.center;
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
        life -= 1;
        if((life < maxLife / 4 && phase == 3) || (life < maxLife / 2) && phase == 2 || (life < maxLife * 3 / 4) && phase == 1)
        {
            phase += 1;
            animator.SetTrigger("AttackCancel");
            EraseCollider();
            inScreen = false;
        }
        else if(life<=0)
        {
            animator.SetTrigger("Death");
            inScreen = false;
        }
    }

    public void SpawnFox()
    {
        foxList.ToArray()[foxCount].SetActive(true);
        foxCount++;
    }
}
