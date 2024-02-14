using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public float knockback;
    public float knockTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        Rigidbody2D enemyRB = collision.GetComponent<Rigidbody2D>();

        enemyRB.isKinematic = false;

        Vector2 direction = enemyRB.transform.position - transform.position;
        direction = direction.normalized * knockback;

        enemyRB.AddForce(direction, ForceMode2D.Impulse);
        StartCoroutine(KnockTime(knockTime, enemyRB));
    }

    IEnumerator KnockTime(float time, Rigidbody2D rb)
    {
        yield return new WaitForSeconds(time);
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }
}
