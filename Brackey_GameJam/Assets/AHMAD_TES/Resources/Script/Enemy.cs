using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float chaseRadius = 5f; // Radius within which the enemy will start chasing the player
    public float rotationSpeed = 5f;
    public float speed = 3f; // Speed at which the enemy moves
    private Rigidbody2D rb;
    NavMeshAgent agent;
    public bool bKanan = false;
    public bool bKiri = false;
    int rott = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player(Clone)").transform;
        //agent = GetComponent<NavMeshAgent>();
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
        }
        else
        {
            // Stop chasing if the player is out of range or not in front of the enemy
            rb.velocity = Vector2.zero;
        }
        //
        if(player.position.y > transform.position.y){
            rott = 1;
        }
        else{
            rott = -1;
        }
        //
        if(bKanan == true){
            float angle = Mathf.Deg2Rad * transform.rotation.z;
            rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed * rott;
        }
        if(bKiri == true){
            float angle = Mathf.Deg2Rad * transform.rotation.z;
            rb.velocity = new Vector2(-Mathf.Cos(angle), -Mathf.Sin(angle)) * speed * rott;
        }
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
}
