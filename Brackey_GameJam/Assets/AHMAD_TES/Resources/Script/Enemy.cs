using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(EnemyHPSlider))]
public class Enemy : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float chaseRadius = 15f; // Radius within which the enemy will start chasing the player
    public float atkRaduis = 3f;
    public float rotationSpeed = 5f;
    public float speed = 3f; // Speed at which the enemy moves
    private Rigidbody2D rb;
    NavMeshAgent agent;
    public bool bKanan = false;
    public bool bKiri = false;
    int rott = 1;

    public bool isAttack;
    public float attackCooldown;
    public Animator anim;

    public float enemyHP;
    public EnemyHPSlider hpSlider;
    private float maxHP;

    public float alertToIdle;
    public float alertCD;
    public bool isIdle = true;
    public float waitCD;
    public float waitRange;
    Vector2Int targetWalk;
    bool hitBorder = false  ;

    private void Awake()
    {
        hpSlider = GetComponent<EnemyHPSlider>();
        maxHP = enemyHP;
        hpSlider.enemyHpSlider.maxValue = maxHP;
        hpSlider.enemyHpSlider.value = maxHP;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //agent = GetComponent<NavMeshAgent>();
        
    }
    
    public void TakeDamage(float dmg)
    {
        hpSlider.TakeDamage(enemyHP, dmg);
        enemyHP--;
        
        Debug.Log(enemyHP);
        
    }

    void Update()
    {
        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if the player is within the chase radius and in front of the enemy
        if (distanceToPlayer <= chaseRadius && IsPlayerInFront())
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move towards the player
            rb.velocity = direction * speed;
            //Debug.Log(agent);
            //agent.SetDestination(player.position);
            if(distanceToPlayer <= atkRaduis){
                //attack
                isAttack = true;
                anim.SetTrigger("Attack");
                StartCoroutine(DelayAttack(attackCooldown));
            }
        }
        else
        {
            // Stop chasing if the player is out of range or not in front of the enemy
            //rb.velocity = Vector2.zero;
            //
            if(alertCD <= 0){
                isIdle = true;
                alertCD = Random.Range(0, alertToIdle);
            }
            if(isIdle == true){
                //wait -> roam -> wait -> roam
                if(waitCD >= 0){
                    waitCD -= Time.deltaTime;
                    //rb.velocity = Vector2.zero; 
                    if(bKanan == true){
                        //float angle = Mathf.Deg2Rad * transform.rotation.z;
                        //rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed * rott;
                        //rb.velocity = Vector2.zero;
                        Vector3 currentRotation = transform.rotation.eulerAngles;

                        // Add 180 degrees to the current rotation
                        currentRotation.z += 180f;

                        // Apply the new rotation to the GameObject
                        transform.rotation = Quaternion.Euler(currentRotation);
                        alertCD = 0;
                    }
                    if(bKiri == true){
                        //float angle = Mathf.Deg2Rad * transform.rotation.z;
                        //rb.velocity = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle)) * speed * rott;
                        //rb.velocity = Vector2.zero;
                        Vector3 currentRotation = transform.rotation.eulerAngles;

                        // Add 180 degrees to the current rotation
                        currentRotation.z += 180f;

                        // Apply the new rotation to the GameObject
                        transform.rotation = Quaternion.Euler(currentRotation);
                        alertCD = 0;
                    }
                }
                else{
                    // func() raytrace to coord, if not hit wall, walk to there
                    isIdle = false;
                    //targetWalk = cektarget();

                    float Aangle = 0;
                    int xr = Random.Range(0,4);
                    if(xr == 0){
                        Aangle = 0f;
                    }
                    if(xr == 1){
                        Aangle = 90f;
                    }
                    if(xr == 2){
                        Aangle = 180f;
                    }
                    if(xr == 3){
                        Aangle = 270f;
                    }
                    Vector3 currentRotation = transform.rotation.eulerAngles;
                    // Add 180 degrees to the current rotation
                    currentRotation.z += Aangle;
                    // Apply the new rotation to the GameObject
                    transform.rotation = Quaternion.Euler(currentRotation);
                    Vector2 direction = new Vector2(Mathf.Cos(Aangle * Mathf.Deg2Rad), Mathf.Sin(Aangle * Mathf.Deg2Rad));
                    rb.velocity = transform.right * speed;
                    float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;


                    Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    if(hitBorder == true){
                        //angle = Mathf.Deg2Rad * transform.rotation.z;
                        //rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed * rott;
                        rb.velocity = rb.velocity * -1;
                        hitBorder = false;
                    }

                    // Convert the angle to a direction vector
                    //Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                    // Add velocity to the Rigidbody2D in the randomized direction
                    //rb.velocity = direction * speed;
                    alertCD = Random.Range(0, alertToIdle);

                    /*
                    Vector2 direction = (targetWalk - new Vector2(transform.position.x, transform.position.y)).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    rb.velocity = direction * speed;
                    */
                    
                }
            }
            else{
                //if(Vector2.Distance(targetWalk, new Vector2(transform.position.x, transform.position.y)) <= 1f){//[]
                    waitCD = Random.Range(0, waitRange);
                    isIdle = true;
                //}
            }
        }
        //
        if(player.position.y > transform.position.y){
            rott = 1;
        }
        else{
            rott = -1;
        }
        //
        if(hitBorder == true){
            float angle = Mathf.Deg2Rad * transform.rotation.z;
            //rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed * rott;
            rb.velocity = rb.velocity * -1;
            hitBorder = false;
        }
        //if(bKiri == true){
            //float angle = Mathf.Deg2Rad * transform.rotation.z;
            //rb.velocity = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle)) * speed * rott;
        //}
        
    }

    // Check if the player is in front of the enemy
    bool IsPlayerInFront()
    {
        bool aman = true;
        List<string> hitTags = new List<string>();
        //
        Vector2 directionToPlayer = player.position - transform.position;
        float angle = Vector2.Angle(transform.right, directionToPlayer);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, (Vector2)player.position - (Vector2)transform.position, Vector2.Distance(transform.position, player.position));
        Debug.DrawRay(transform.position, (Vector2)player.position - (Vector2)transform.position, Color.red);
        // You can adjust the angle threshold to change the cone in which the enemy detects the player
        if (angle < 90f || angle > 270)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if(hit.collider != null){
                    Debug.Log("Raycast hit " + hit.collider);
                    hitTags.Add(hit.collider.tag);
                    if(hit.collider.CompareTag("Border")){
                        return false;
                        aman = false;
                        break;
                    }
                }
                else{
                    return false;
                }
            }
            if(aman == true){
                return true;
            }
            else{
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    IEnumerator DelayAttack(float time)
    {
        yield return new WaitForSeconds(time);
        isAttack = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("inc");
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Border"))
        {
            hitBorder = true;
        }
    }

    /*
    Vector2Int cektarget() {
        bool aman = true;
        List<string> hitTags = new List<string>();
        Vector2Int _t = Vector2Int.zero; // Initialize _t
        Vector2Int target = new Vector2Int(Random.Range(0, 40 * 3), Random.Range(0, 40 * 3));
        Vector2 directionToPlayer = targetWalk - new Vector2(transform.position.x, transform.position.y);
        float angle = Vector2.Angle(transform.right, directionToPlayer);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, (Vector2)target - (Vector2)transform.position, Vector2.Distance(transform.position, target));
        Debug.DrawRay(transform.position, (Vector2)target - (Vector2)transform.position, Color.red);
    
        foreach (RaycastHit2D hit in hits) {
            if (hit.collider != null) {
                Debug.Log("Raycast hit " + hit.collider);
                hitTags.Add(hit.collider.tag);
                if (hit.collider.CompareTag("Border")) {
                    aman = false;
                    break; // Exit the loop, no need to continue
                }
            }
        }

        if (!aman) {
            _t = cektarget(); // Recursive call
        } else {
            _t = target;
        }

        return _t;
    }
    */

}
