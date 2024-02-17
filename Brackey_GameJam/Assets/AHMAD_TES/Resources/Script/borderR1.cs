using UnityEngine;

public class borderR1 : MonoBehaviour
{
    //public string collisionTag = "Border";

    // Define the value you want to change in the parent
    public bool hitR;
    public bool isKanan;
    public bool isKiri;

    void Start()
    {
        // Store the original value
        //hitR = transform.parent.GetComponent<PlayerMovement>().hitR; // Replace YourParentScript and hitR with your actual script and value names
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("inc");
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Border"))
        {
            // Access the parent GameObject and change its value
            if(isKiri == true){
                transform.parent.GetComponent<Enemy>().bKanan = true; // Replace YourParentScript and hitR with your actual script and value names
            }
            if(isKanan == true){
                transform.parent.GetComponent<Enemy>().bKiri = true; // Replace YourParentScript and hitR with your actual script and value names
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("inc");
        // Check if the colliding object has the specified tag
        if (collision.gameObject.CompareTag("Border"))
        {
            // Access the parent GameObject and change its value
            if(isKiri == true){
                transform.parent.GetComponent<Enemy>().bKanan = true; // Replace YourParentScript and hitR with your actual script and value names
            }
            if(isKanan == true){
                transform.parent.GetComponent<Enemy>().bKiri = true; // Replace YourParentScript and hitR with your actual script and value names
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("outc");
        // Reset the value to its original value when the colliding object exits
        if (collision.gameObject.CompareTag("Border"))
        {
            // Access the parent GameObject and reset its value
            if(isKiri == true){
                transform.parent.GetComponent<Enemy>().bKanan = false; // Replace YourParentScript and hitR with your actual script and value names
            }
            if(isKanan == true){
                transform.parent.GetComponent<Enemy>().bKiri = false; // Replace YourParentScript and hitR with your actual script and value names
            }
        }
    }
}
