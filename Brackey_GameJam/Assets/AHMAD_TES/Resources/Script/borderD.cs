using UnityEngine;

public class borderD : MonoBehaviour
{
    //public string collisionTag = "Border";

    // Define the value you want to change in the parent
    public bool hitD;

    void Start()
    {
        // Store the original value
        //hitD = transform.parent.GetComponent<PlayerMovement>().hitD; // Replace YourParentScript and hitD with your actual script and value names
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("inc");
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Border"))
        {
            // Access the parent GameObject and change its value
            transform.parent.GetComponent<PlayerMovement>().hitD = true; // Replace YourParentScript and hitD with your actual script and value names
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            transform.parent.GetComponent<PlayerMovement>().hitD = false;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("inc");
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Border"))
        {
            // Access the parent GameObject and change its value
            transform.parent.GetComponent<PlayerMovement>().hitD = true; // Replace YourParentScript and hitD with your actual script and value names
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("outc");
        // Reset the value to its original value when the colliding object exits
        if (collision.gameObject.CompareTag("Border"))
        {
            // Access the parent GameObject and reset its value
            transform.parent.GetComponent<PlayerMovement>().hitD = false; // Replace YourParentScript and hitD with your actual script and value names
        }
    }
}