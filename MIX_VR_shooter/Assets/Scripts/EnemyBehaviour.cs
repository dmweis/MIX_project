using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour {

    public GameObject fireballPrefab;
    private Transform fireballSpawn;
    private float health, maxHealth;
    private Image healthBar;
    private GameObject player;
    private Animator animator;
    public GameObject teleportEffect;
    public int shootEveryXsec;
    public int damageWhenHit;
    public int totalHealth;
    public string tagOfPlayerShot;
    bool alive;

    // Animation states
    const int STATE_IDLE = 0;
    const int STATE_SHOOT = 1;
    const int STATE_DIE = 3;
    
    private int CurrentState { get; set; }

    private void Awake()
    {
        healthBar = transform.FindChild("EnemyCanvas").FindChild("HealthBG").FindChild("Health").GetComponent<Image>();
        fireballSpawn = transform.FindChild("FireballSpawn");
        health = totalHealth;
        maxHealth = totalHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        CurrentState = STATE_IDLE;
        alive = true;
    }

    // Use this for initialization
    void Start () {
        animator = this.GetComponent<Animator>();
        Instantiate(teleportEffect, this.transform.position, this.transform.rotation);
        StartCoroutine(Shooting());
    }
	
	// Update is called once per frame
	void Update () {

        // Make monster always look at player
        transform.LookAt(player.transform.position);

        // If enemy has no health left start dying
        if (health < 1 & alive == true)
        {
            StartCoroutine(Die());
        }
        else
        {
            // else default state is idle
            changeAnimationState(STATE_IDLE);
        }
    }

    IEnumerator Die()
    {
        // Stop coroutine, play die animation and destroy monster in 5 sec
        alive = false;
        changeAnimationState(STATE_DIE);
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }

    IEnumerator Shooting()
    {
        while (alive)
        {
            // Play throwing animation every x sec while alive
            changeAnimationState(STATE_SHOOT);
            yield return new WaitForSeconds(shootEveryXsec);
            
        }

    }

    void changeAnimationState(int state)
    {
        // If unchanged return
        if(CurrentState == state)
        {
            return;
        }
        // If monster is dead just return
        if(CurrentState == STATE_DIE)
        {
            return;
        }

        // Change state
        CurrentState = state;

        switch (state)
        {
            case STATE_IDLE:
                animator.SetInteger("State", STATE_IDLE);
                break;
            case STATE_SHOOT:
                animator.SetInteger("State", STATE_SHOOT);
                break;
            case STATE_DIE:
                animator.SetInteger("State", STATE_DIE);
                break;
        }
    }

    void Shoot()
    {
        Instantiate(fireballPrefab, fireballSpawn.position, fireballSpawn.rotation);
    }

    private void Move(Vector3 vector3)
    {
        StartCoroutine(TeleportEffect(vector3));
    }

    IEnumerator TeleportEffect(Vector3 vector)
    {
        // Make teleport effect appear on monster
        Instantiate(teleportEffect, this.transform.position, this.transform.rotation);
        yield return new WaitForSeconds(1);

        // Make teleport effect appear on next location that enemy is moving to
        Instantiate(teleportEffect, vector, this.transform.rotation);
        yield return new WaitForSeconds(2);

        // Teleport enemy (teleport effect will be destroyed in 2 sec)
        transform.position = vector;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagOfPlayerShot)
        {
            health -= damageWhenHit;
            // Update healthbar
            healthBar.fillAmount = health / maxHealth;
        }
    }
}
