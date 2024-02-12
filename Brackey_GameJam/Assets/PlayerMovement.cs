using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float moveSpeed;
    private Vector2 _moveVec;

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
        Move();
    }

    private void Move()
    {
        _rb.MovePosition(_rb.position + (_moveVec).normalized * moveSpeed * Time.fixedDeltaTime);


    }
}
