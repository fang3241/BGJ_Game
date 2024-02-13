using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Camera _cam;
    public float attackDegree;
    public float slashSpeed;
    public float slashRecoverySpeed;
    public float attackCooldown;

    private float distance1, distance2;

    public bool isAttack;
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
            StartCoroutine(Attack());
        }
        else
        {
            AttackDirection();
        }
    }

    private void AttackDirection()
    {
        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 1;

        Vector3 lookPos = new Vector3(
            mousePos.x - transform.position.x,
            mousePos.y - transform.position.y,
            mousePos.z
            );

        float dir = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, dir);
    }

    IEnumerator Attack()
    {
        isAttack = true;
        bool switched = false;
        Vector3 rotationStart = transform.localEulerAngles;
        float currentRotation = rotationStart.z;

        float upperBorder, lowerBorder;
        if(currentRotation < 90 || currentRotation > 270)
        {
            upperBorder = currentRotation + (attackDegree / 2);
            lowerBorder = currentRotation - (attackDegree / 2);
        }
        else
        {
            switched = true;
            Debug.Log(currentRotation);
            upperBorder = currentRotation - (attackDegree / 2);
            lowerBorder = currentRotation + (attackDegree / 2);

            Debug.Log("UU " + upperBorder);
            Debug.Log("DD " + lowerBorder);
        }

        
        if (!switched)
        {
            //Move rotasi pedang keatas
            while (currentRotation < upperBorder)
            {
                currentRotation += (slashSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
                //Debug.Log("Slash Up");
            }

            //Move slash kebawah
            while (currentRotation > lowerBorder)
            {
                currentRotation -= (slashSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
                //Debug.Log("SlashDown");
            }

            //kembali ke posisi semula awal, yang sebelumnya slash kebawah
            while (currentRotation <= rotationStart.z)
            {
                currentRotation += (slashSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
                //Debug.Log("Attack Recovery");
            }

        }
        else
        {
            //pedang keatas
            while(currentRotation > upperBorder)
            {
                currentRotation -= (slashSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
                //Debug.Log("Slash Up");
            }

            //pedang kebawah
            while (currentRotation < lowerBorder)
            {
                currentRotation += (slashSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
                //Debug.Log("Slash Up");
            }

            while (currentRotation >= rotationStart.z)
            {
                currentRotation -= (slashSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                yield return null;
                //Debug.Log("Attack Recovery");
            }
        }


        //transisi ke posisi baru(arah mouse baru), biar nggak ngeblink(MASIH AGAK NGEBUG)
        float newTargetPos = transform.localEulerAngles.z;

        distance1 = Mathf.Min(currentRotation, newTargetPos) - Mathf.Max(currentRotation, newTargetPos);
        distance2 = 360 - Mathf.Abs(distance1);

        float counter = 0;
        if(Mathf.Abs(distance1) < Mathf.Abs(distance2))
        {
            while (counter < Mathf.Abs(distance1))
            {
                currentRotation += Mathf.Sign(distance1) * (slashRecoverySpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                counter += (slashRecoverySpeed * Time.deltaTime);
                yield return null;
                //Debug.Log("D" + currentRotation);
            }
        }
        else
        {
            while (counter < Mathf.Abs(distance2))
            {
                currentRotation -= Mathf.Sign(distance2) * (slashRecoverySpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentRotation);
                counter += (slashRecoverySpeed * Time.deltaTime);
                yield return null;
                //Debug.Log("U" + currentRotation);
            }
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttack = false;
    }


}
//RECYCLE BIN :D

//float distance = Mathf.Abs(currentRotation - newTargetPos);
//float minDistance = Mathf.Min((360 - distance), distance);

//if (currentRotation > newTargetPos)
//{
//    while (currentRotation > newTargetPos)
//    {
//        currentRotation -= (slashRecoverySpeed * Time.deltaTime);
//        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
//        yield return null;
//    }
//    Debug.Log("Recovery to new pos DOWN");
//}
//else
//{
//    while (currentRotation <= newTargetPos)
//    {
//        currentRotation += (slashRecoverySpeed * Time.deltaTime);
//        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
//        yield return null;
//    }
//    Debug.Log("Recovery to new pos UP");

//}
