﻿using System.Collections;
using UnityEngine;

public class Summoner : Enemy
{

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public Enemy enemyToSummon;
    public float timeBetweenSummons;
    public float meleeAttackSpeed;
    public float stopDistance;

    private float _summontime;
    private Vector2 _targetPosition;
    private Animator _anim;
    private float _timer;

    // Start is called before the first frame update
    public override void Start()
    {
        // Call the start script in the base class
        base.Start();

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        _targetPosition = new Vector2(randomX, randomY);
        _anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (Vector2.Distance(transform.position, _targetPosition) > .5f)
            {
                // too far away from target position
                transform.position = Vector2.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
                _anim.SetBool("isRunning", true);
            }
            else
            {
                // reached target position
                _anim.SetBool("isRunning", false);

                if (Time.time >= _summontime)
                {
                    _summontime = Time.time + timeBetweenSummons;
                    _anim.SetTrigger("summon");
                }
            }

            // Melee Attack
            if (Vector2.Distance(transform.position, player.position) < stopDistance)
            {
                if (Time.time >= _timer)
                {
                    _timer = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    public void Summon()
    {
        if (player != null)
        {
            Instantiate(enemyToSummon, transform.position, transform.rotation);
        }
    }

    // Coroutine
    IEnumerator Attack()
    {
        player.GetComponent<Player>().TakeDamage(damage);

        // Before leap
        Vector2 originalPosition = transform.position;
        Vector2 targetPosition = player.position;

        // How much of the animation has happened
        float percent = 0;

        while (percent <= 1)
        {
            percent += Time.deltaTime * meleeAttackSpeed;
            float formula = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector2.Lerp(originalPosition, targetPosition, formula);
            yield return null;
        }
    }
}
