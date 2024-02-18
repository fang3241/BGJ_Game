using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float knockback;
    public float knockTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        string thisParentTag = this.transform.parent.tag;
        
        if(thisParentTag == "EnemyWeapon")
        {
           
            if (collision.CompareTag("Player"))
            {
                Debug.Log("EnemyWeaponHit");
                Debug.Log("HitPlayer");
                HpSliderController pl = FindAnyObjectByType<HpSliderController>();
                pl.TakeDamage(1);
            }
        }
        else if(thisParentTag == "PlayerWeapon")
        {
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("PlayerWeaponHit");
                Debug.Log("HitEnemy");
                Debug.Log(collision);
                Rigidbody2D enemyRB = collision.GetComponent<Rigidbody2D>();

                //enemyRB.isKinematic = false;

                //Vector2 direction = enemyRB.transform.position - transform.position;
                //direction = direction.normalized * knockback;

                //enemyRB.AddForce(direction, ForceMode2D.Impulse);
                enemyRB.GetComponent<Enemy>().TakeDamage(1);
                //StartCoroutine(KnockTime(knockTime, enemyRB));

                //HpSliderController pl = FindAnyObjectByType<HpSliderController>();
                //pl.TakeDamage(1);
            }
            //if (collision.CompareTag("Enemy"))
            //{
            //    Rigidbody2D enemyRB = collision.transform.parent.parent.GetComponent<Rigidbody2D>();

            //    enemyRB.isKinematic = false;

            //    Vector2 direction = enemyRB.transform.position - transform.position;
            //    direction = direction.normalized * knockback;

            //    enemyRB.AddForce(direction, ForceMode2D.Impulse);
            //    enemyRB.GetComponent<Enemy>().TakeDamage(1);
            //    StartCoroutine(KnockTime(knockTime, enemyRB));
            //}
        }
    }

    //IEnumerator KnockTime(float time, Rigidbody2D rb)
    //{
    //    yield return new WaitForSeconds(time);
    //    rb.velocity = Vector2.zero;
    //    rb.isKinematic = true;
    //}
}
