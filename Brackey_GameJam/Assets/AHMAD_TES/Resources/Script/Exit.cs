using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    public int floorTarget;
    public Vector2Int coord;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hitP " + collision.gameObject.tag);
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("FINISH");
        }
            
    }

    
}
