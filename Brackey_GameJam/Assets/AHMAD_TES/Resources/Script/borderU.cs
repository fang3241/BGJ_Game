using UnityEngine;

public class borderU : MonoBehaviour
{
    //public string collisionTag = "Floor";

    // Define the value you want to change in the parent
    public bool hitU;

    void Start()
    {
        // Store the original value
        //hitU = transform.parent.GetComponent<PlayerMovement>().hitU; // Replace YourParentScript and hitU with your actual script and value names
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("inc");
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Access the parent GameObject and change its value
            transform.parent.GetComponent<PlayerMovement>().hitU = true; // Replace YourParentScript and hitU with your actual script and value names
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("outc");
        // Reset the value to its original value when the colliding object exits
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Access the parent GameObject and reset its value
            transform.parent.GetComponent<PlayerMovement>().hitU = false; // Replace YourParentScript and hitU with your actual script and value names
        }
    }
}
