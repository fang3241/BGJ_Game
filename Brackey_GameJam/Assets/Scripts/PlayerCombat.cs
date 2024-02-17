using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Camera _cam;
    //public float attackDegree;
    //public float slashSpeed;
    //public float slashRecoverySpeed;

    public float attackCooldown;
    public float weaponFollowSpeed;
    public float scale;
    public GameObject player;

    private float distance1, distance2;

    public Animator anim;

    public bool isAttack;
    public float a;
    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
        isAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !isAttack)
        {
            Attack();
        }
       
        followCursor();
    }

    private void Attack()
    {
        isAttack = true;
        anim.SetTrigger("Attack");
        StartCoroutine(DelayAttack(attackCooldown));
    }

    IEnumerator DelayAttack(float time)
    {
        yield return new WaitForSeconds(time);
        isAttack = false;
    }

    void followCursor()
    {
        transform.position = player.transform.position;

        if (!isAttack)
        {
            Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

            Vector2 lookPos = new Vector3(
                    mousePos.x - transform.position.x,
                    mousePos.y - transform.position.y
                    );

            float dir = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

            if (dir > 90 || dir < -90)
            {
                transform.localScale = new Vector3(scale, -scale, scale);   //kalo ikhsan jadi +-+, kalo ahmad jadi -++//
                //Debug.Log("-");
            }
            else
            {
                transform.localScale = new Vector3(scale, scale, scale);
                //Debug.Log("+");
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, dir), weaponFollowSpeed * Time.deltaTime);
        }
    }

    

}

//RecycleBin
//void smoth()
//{
//    float angle = Mathf.SmoothDampAngle(transform.rotation.z, 45, ref a, 0.1f);

//    transform.rotation = Quaternion.Euler(0, 0, angle);
//}

//IEnumerator RotateOverTime(float rotationAmount, float rotationSpeed)
//{
//    float currentRot = transform.eulerAngles.z;

//    Debug.Log(currentRot);
//    // Calculate the target rotation based on the rotation amount
//    Quaternion targetRotation = Quaternion.Euler(0, 0, currentRot + rotationAmount);

//    // Calculate the time needed for the rotation
//    float rotationTime = Mathf.Abs(rotationAmount) / rotationSpeed;

//    // Record the starting time
//    float startTime = Time.time;

//    // Loop until the desired rotation time has elapsed
//    while (Time.time - startTime < rotationTime)
//    {
//        // Calculate the current rotation progress
//        float rotationProgress = (Time.time - startTime) / rotationTime;

//        // Rotate the object gradually towards the target rotation
//        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationProgress * rotationSpeed * Time.deltaTime);
//        transform.rotation = newRotation;

//        // Wait for the next frame
//        yield return null;

//        //Debug.Log(rotationProgress);
//    }

//    // Ensure the object is rotated exactly to the target rotation
//    //transform.rotation = targetRotation;
//}

//IEnumerator Slash()
//{
//    isAttack = true;
//    bool switched = false;
//    Vector3 rotationStart = transform.localEulerAngles;
//    float currentRotation = rotationStart.z;
//    float upperBorder, lowerBorder;

//    if (currentRotation <= 90 || currentRotation >= 270)
//    {
//        upperBorder = currentRotation + (90 / 2);
//        lowerBorder = currentRotation - (90 / 2);
//    }
//    else
//    {
//        switched = true;
//        Debug.Log(currentRotation);
//        upperBorder = currentRotation - (90 / 2);
//        lowerBorder = currentRotation + (90 / 2);
//    }
//    Debug.Log("CC " + currentRotation);
//    Debug.Log("UU " + upperBorder);
//    Debug.Log("DD " + lowerBorder);
//    Debug.Log(switched);

//    float currentCounter = 0;
//    float countDuration = 0.5f;

//    while (currentCounter < currentRotation)
//    {
//        currentCounter += Time.deltaTime;
//        float perc = currentCounter / countDuration;
//        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, upperBorder), perc);
//        Debug.Log(currentCounter);
//        yield return null;
//    }

//    Debug.Log(transform.eulerAngles);
//    //countDuration = 0.8f;

//    //for (float currentCounter = 0; currentCounter < countDuration; currentCounter += Time.deltaTime)
//    //{
//    //    currentCounter += Time.deltaTime;
//    //    float perc = currentCounter / countDuration;
//    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, lowerBorder), 50 * perc);
//    //    //Debug.Log(currentCounter);
//    //    yield return null;
//    //}

//    isAttack = false;
//    yield return null;
//}

//private void AttackDirection()
//{
//    if (!isAttack)
//    {
//        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
//        mousePos.z = 1;
//        Vector3 lookPos = new Vector3(
//            mousePos.x - transform.position.x,
//            mousePos.y - transform.position.y,
//            mousePos.z
//            );

//        float dir = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;



//        //transform.right = (new Vector3(0,0,dir));
//        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,dir), Time.deltaTime * 0.1f);
//        transform.rotation = Quaternion.Euler(0, 0, dir);

//        if (dir > 90 && dir < 270)
//        {
//            transform.localScale = new Vector3(1, -1, 1);
//        }
//        else
//        {
//            transform.localScale = new Vector3(1, 1, 1);
//        }

//        Debug.Log(dir);
//    }

//    //Debug.Log("pointing");
//}