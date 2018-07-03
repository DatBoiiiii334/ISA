using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

//public enum State { Idle, Aggro, Patrol}

public class Chase : MonoBehaviour {

    public enum State { Dead, Aggro, Patrol, Idle };
    public State enumState;

	public Transform player;
	Animator anim;

    public float startHealth = 300f;
	private float health;
	public Image healthBar;
	public float lifeTime = 5f;

    //State enumState = State.Patrol;

   // string state = "patrol";
	public GameObject[] waypoints; // way points/amount of waypoints
	public int currentWP = 0;

	public float rotSpeed = 0.2f; //rotation speed
	public float speed = 1.5f; // move speed
	float accuracyWP = 2.0f; // accuracy when following way points

	public GameObject screenFlash; //Blink red when taking damage 
	// 3 different hurt sounds for when getting hit
	public AudioSource Hurt01; 
	public AudioSource Hurt02;
	public AudioSource Hurt03;
	public int painSound;
	public int attackTrigger;
	public bool canAttack;
    public bool isDead;

    //public Transform direction;
    Vector3 direction;


	[SerializeField]
	Transform _destination;
	private NavMeshAgent agent;


	// Use this for initialization
	void Start () {
		health = startHealth;
		anim = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
        Vector3 direction = player.position - this.transform.position;
        direction.y = 0;
        //float angle = Vector3.Angle (direction,this.transform.forward);

        currentWP = Random.Range(0, waypoints.Length);

        enumState = State.Patrol;

    }

    // Update is called once per frame
    void Update () {
        
        switch (enumState)
        {
            case State.Patrol:
                Patrol();
                // print("Patrol");
                break;
            case State.Aggro:
                Aggro();
                // print("Aggro");
                break;
            case State.Dead:
                Dead();
                break;
            case State.Idle:
                Idle();
                break;
            default:
                break;
        }

        //Incase you are out of his range
        if (Vector3.Distance(transform.position, player.position) > 30f)
        {
            enumState = State.Patrol;
        }

        if (currentWP == null)
        {
            enumState = State.Idle;
        }
        

        }// End Update loop

    void Dead()
    {
        isDead = true;
        direction.y = 0;
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;
        float angle = Vector3.Angle(direction, this.transform.forward);
        anim.SetBool("isDead", true);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isWalking", false);
        canAttack = false;
        Destroy(gameObject, lifeTime);
        //agent.SetDestination(agent.destination);
       
    }

    void Idle()
    {
        anim.SetBool("isDead", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isIdle", true);   
    }

    void Patrol()
    {
        //Start patrolling cuz no player around
        //if (State == Patrol && waypoints.Length > 0)
        if (waypoints.Length > 0)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isWalking", true);
            if (Vector3.Distance(waypoints[currentWP].transform.position, transform.position) < accuracyWP)
            {
                //Use this if you want them to follow each WP randomly
                currentWP = Random.Range(0, waypoints.Length);
                //Use this if you want them to follow each WP orderly
                //if (currentWP >= waypoints.Length) 
                //{
                //	currentWP = 0;
                //}
            }
            //Go to WP direction
            agent.SetDestination(waypoints[currentWP].transform.position);
        }
    }



    void Aggro()
    {
        agent.SetDestination(player.position);
        if (canAttack == true)
        {
            anim.SetBool("isAttacking", true);
            anim.SetBool("isWalking", false);
            StartCoroutine(EnemyDamage());
        }
        else if (canAttack == false)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalking", true);
            agent.SetDestination(player.position);
        }
    }


	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			canAttack = true;
            if(isDead == false)
            {
                enumState = State.Aggro;
            }
		}

	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") 
		{
			canAttack = false;
            
		}
	}

	IEnumerator EnemyDamage(){
		//print ("ThePain");
		yield return new WaitForSeconds (0.15f);
		screenFlash.SetActive (true);
		GlobalHealth.currentHealth -= 0.5f;
		Hurt03.Play ();
		yield return new WaitForSeconds (0.05f);
		screenFlash.SetActive (false);
		yield return new WaitForSeconds (3);
	}

	private void SetDestination()
	{
		if(_destination != null)
		{
            //Vector3 targetVector = _destination.transform.position;
            //_navMeshAgent.SetDestination(targetVector);
            agent.SetDestination(waypoints[currentWP].transform.position);

        }
	}
		
	public void TakeDamege (float amount)
	{
		health -= amount;
        enumState = State.Aggro;
        //state = "Agro";
        //print("Agro");
        healthBar.fillAmount = health / startHealth;

		if (health <= 0f) 
		{
            enumState = State.Dead;
		}
	}
}

