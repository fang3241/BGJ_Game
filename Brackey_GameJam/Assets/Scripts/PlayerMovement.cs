using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer[] mySpriteRenderer;

    public float moveSpeed;
    private Vector2 _moveVec;
    public bool hitU = false; 
    public bool hitD = false; 
    public bool hitL = false; 
    public bool hitR = false; 
    public Animator animator;

    public float playerHP;
    public float doorCD = 2f;
    public bool isDoorCD;

    // Start is called before the first frame update
    void Start()
    {
        isDoorCD = false;
        _rb = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        HpSliderController hp = FindAnyObjectByType<HpSliderController>();
        hp.Setup(this, playerHP);
    }

    public void StartCD()
    {
        StartCoroutine(Cooldown());
    }

    public IEnumerator Cooldown()
    {
        isDoorCD = true;
        Debug.Log("COOLD");
        yield return new WaitForSeconds(doorCD);
        isDoorCD = false;
    }

    // Update is called once per frame
    void Update()
    {
        _moveVec.x = Input.GetAxisRaw("Horizontal");
        _moveVec.y = Input.GetAxisRaw("Vertical");
        // if the A key was pressed this frame
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Flip the sprite based on mouse position relative to the GameObject
        if (mousePosition.x < transform.position.x)
        {
            // Mouse is on the left side of the GameObject, flip the sprite
            mySpriteRenderer[0].flipX = true;
        }
        else
        {
            // Mouse is on the right side of the GameObject, unflip the sprite
            mySpriteRenderer[0].flipX = false;
        }

        //
        if(_moveVec != Vector2.zero){
            animator.speed = 1f;
        }
        else{
            animator.speed = 0f;
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
        }
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
