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

    public GameObject[] bossPunchColliders3D;

    public int maxLife;

    List<GameObject> foxList;
    List<GameObject> powerUpList;

    int foxCount = 0;

    public delegate void OnBossDead();
    public static OnBossDead Die;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        sign.StartFight += ActivateBoss;
        attackTime = attackTimeMax;
        BossPunchBehaviour.HitEnemy = GetDamage;
        maxLife = life;
        if (EnemyManager.instance)
            foxList = EnemyManager.instance.GetList("Fox");

        if (ItemManager.instance)
            powerUpList = ItemManager.instance.GetListOfTag("PowerUp");

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
                case 2:
                    animator.SetTrigger("Attack");
                    animator.SetInteger("AttackNumber", 0);
                    attack = false;
                    break;
                case 3:
                case 4:
                    animator.SetTrigger("Attack");
                    animator.SetInteger("AttackNumber", 1);
                    attack = false;
                    break;
            }
        }

        animator.SetBool("InScene", inScreen);
    }

    public void PlayBossAudio()
    {
        GameManager.Instance.PlaySound("Boss");
    }

    public void ActivateBoss()
    {
        animator.enabled = true;
        animator.Play("Boss_Start", -1, 0);
        for(int i=0;i<transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
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
            GameManager.Instance.PlaySound("Boss_dead");
            EraseCollider();
            inScreen = false;
        }
    }

    internal void Restart()
    {
        phase = 1;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        animator.enabled = false;
        life = maxLife;
        foreach (GameObject g in foxList)
        {
            g.GetComponent<FoxBehaviour>().Restart();
        }
        sign.gameObject.SetActive(true);
        sign.GetComponent<Animator>().Play("animacion cartel", -1, 0);
        EraseCollider();
        foxCount = 0;
    }

    public void OnDead()
    {
        Die();
    }

    public void SpawnFox()
    {
        foxList.ToArray()[foxCount].SetActive(true);
        if(powerUpList != null)
            powerUpList.ToArray()[foxCount].SetActive(true);
        foxCount++;
    }
}
