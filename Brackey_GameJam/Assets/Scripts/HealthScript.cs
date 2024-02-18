using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.Play("Heal");
            //setontriggerenter => cek kalo yg collide tag nya player => findobjectbytype > hpslidercontroller > AddHP(heal)
            FindObjectOfType<HpSliderController>().AddHP(1);
            Destroy(this.gameObject);
        }
    }
}
