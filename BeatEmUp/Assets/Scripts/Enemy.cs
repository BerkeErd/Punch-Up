﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    bool facingRight;

    public int Damage=10;

    public int maxHealth = 100;
    public int currentHealth;

    public Transform PunchPoint;
    public float punchRange = 0.5f;

    public Transform KickPoint;
    public float kickRange = 0.5f;

    private SoundManager soundmanager;
    private HealthBar healthBar;
    private GameObject HealthBarObject;

    private AudioSource AudioSource;
    public bool isDead = false;
    bool isKnockedUp;
    public bool isFrozen = false;

    public float speed;
    public float chaseDistance;
    public float stopDistanceX;
    public float stopDistanceY;
    public bool isAttacking = false;
    public bool tookDamage = false;
    public bool isWalking = false;
    
    public GameObject target;

    public bool targetClose = false;

    public LayerMask playerLayer;

    Animator animator;

    public float targetDistanceX;
    public float targetDistanceY;

    public LevelManager levelManager;


    private void Awake()
    {
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        HealthBarObject = Instantiate(Resources.Load("Prefabs/HealthBar")) as GameObject;
        HealthBarObject.transform.parent = GameObject.Find("LevelCanvas").GetComponent<Canvas>().transform;
        HealthBarObject.transform.localScale =new Vector3(1,1,1);
        healthBar = HealthBarObject.GetComponent<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);
    }
    void Start()
    {
        Damage += levelManager.Level;

        AudioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        target = GameObject.Find("Fighter");
        AudioSource = GetComponent<AudioSource>();
        soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        stopDistanceX = Random.Range(stopDistanceX - 0.3f, stopDistanceX + 0.3f);
        stopDistanceY = Random.Range(stopDistanceY - 0.1f, stopDistanceY + 0.1f);

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        tookDamage = true;
        isAttacking = false;

        float pushPower = (float)damage / 200 * 7;

        if (facingRight && !isKnockedUp)
        {
            transform.position = new Vector2(transform.position.x - pushPower, transform.position.y);
        }
        if (!facingRight && !isKnockedUp)
        {
            transform.position = new Vector2(transform.position.x + pushPower, transform.position.y);
        }

        if (!isKnockedUp)
        animator.SetTrigger("Hit");

        if(currentHealth <= 0 )
        {
            Die();
        }
        healthBar.SetHealth(currentHealth);

    }

    public void KnockUp()
    {

        if(!isKnockedUp)
        {
            animator.SetTrigger("KnockDown");
            isKnockedUp = true;
        }
        

        
    }

    public void Destroy()
    {
        Destroy(HealthBarObject);
        Destroy(gameObject);
    }


    void Die()
    {
        isDead = true;
        animator.SetBool("IsDead", isDead);
        
       
    }
    // Update is called once per frame
    void Update()
    {
        healthBar.transform.position = new Vector2(transform.position.x, transform.position.y + 3.2f);
        if (transform.position.x < target.transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (transform.position.x > target.transform.position.x && facingRight)
        {
            Flip();
        }

        if (!isFrozen)
        {
            if (isDead) // Bazen bug oluyor ölse de ölme animasyonuna girmiyor bu onu engellemek için
            {
                Die();
            }

           
            targetDistanceX = Mathf.Abs(transform.position.x - target.transform.position.x);
            targetDistanceY = Mathf.Abs(transform.position.y - target.transform.position.y);

            if (!isDead && !isKnockedUp && !tookDamage)
            {

                if (targetDistanceX < chaseDistance && targetDistanceX > stopDistanceX || (targetDistanceY < chaseDistance && targetDistanceY > stopDistanceY && !isKnockedUp && targetDistanceX < chaseDistance && targetDistanceX > stopDistanceX))
                    ChasePlayer();
                else
                    StopChasePlayer();

                if (targetDistanceX < stopDistanceX && targetDistanceY < stopDistanceY)
                    targetClose = true;
            }
            else
            {
                //Bi şey yapma
            }
        }

        
    }

    private void Attack1()
    {
        animator.SetTrigger("Attack1");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(KickPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            AudioSource.clip = soundmanager.Punch2;
            AudioSource.Play();
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerMovement>().isDead)
            {
                player.GetComponent<PlayerMovement>().TakeDamage(Damage);            
            }
        }
    }

    private void Attack2()
    {
        animator.SetTrigger("Attack2");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(KickPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            AudioSource.clip = soundmanager.Kick2;
            AudioSource.Play();
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerMovement>().isDead)
            {
                player.GetComponent<PlayerMovement>().TakeDamage(Damage*2);
            }
        }
    }

    private void Attack3()
    {
        animator.SetTrigger("Attack3");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(KickPoint.position, kickRange, playerLayer);
        if (hitEnemies.Length > 0)
        {
            AudioSource.clip = soundmanager.Punch1;
            AudioSource.Play();
        }
        foreach (Collider2D player in hitEnemies)
        {
            if (!player.GetComponent<PlayerMovement>().isDead)
            {
                player.GetComponent<PlayerMovement>().TakeDamage(Damage);

            }
        }

    }

    private IEnumerator AttackPlayer()
    {


         isAttacking = true;
         yield return new WaitForSeconds(2);

            if (targetClose && !tookDamage)
            {
                

                int AttackNo = Random.Range(0, 5);
                switch (AttackNo)
                {
                    case 0:
                        Attack1();
                  
                        break;

                    case 1:
                        Attack3();
    
                        break;

                    case 2:
                        Attack1();
                        break;

                    case 3:
                        Attack3();

                        break;

                    case 4:
                       Attack2();
                    break;

                default:
                        break;


                }
            }
            else
            {
                isAttacking = false;
            }


        tookDamage = false;
        StopAllCoroutines();
    }

    private void StopChasePlayer()
    {
        isWalking = false;
        animator.SetBool("IsWalking", isWalking);

        if(!isAttacking)
        StartCoroutine(AttackPlayer());
    }

    public void AlertObservers(string message)
    {
        if (message == "AttackEnded")
        {
            isAttacking = false;
        }

        if (message == "StandUp")
        {
            isKnockedUp = false;
        }

        if (message == "HitEnded")
        {
            tookDamage = false;
        }

    }

    private void ChasePlayer()
    {
        targetClose = false;
       
           

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        
        isWalking = true;
        animator.SetBool("IsWalking", isWalking);
    }

    private void Flip()
    {
        
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
       
    }

}
