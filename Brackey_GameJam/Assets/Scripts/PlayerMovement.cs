using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float moveSpeed;
    private Vector2 _moveVec;
    public bool hitU = false; 
    public bool hitD = false; 
    public bool hitL = false; 
    public bool hitR = false; 

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveVec.x = Input.GetAxisRaw("Horizontal");
        _moveVec.y = Input.GetAxisRaw("Vertical");
        
    }

    private void FixedUpdate()
    {
        if(hitU == true){
            _moveVec.y = moveSpeed * -2;
        }
        if(hitD == true){
            _moveVec.y = moveSpeed * 2;
        }
        if(hitR == true){
            _moveVec.x = moveSpeed * -2;
        }
        if(hitL == true){
            _moveVec.x = moveSpeed * 2;
        }
        //Debug.Log("hitU: " + hitU);
        Move();
    }

    private void Move()
    {
        _rb.MovePosition(_rb.position + (_moveVec).normalized * moveSpeed * Time.fixedDeltaTime);


    }
}
