using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHPSlider : MonoBehaviour
{
    public Slider enemyHpSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(float enemyHP, float dmg)
    {
        float updatedHP = enemyHP - dmg;
        LeanTween.value(enemyHP, updatedHP, 0.2f).setOnUpdate((float val) =>
        {
            enemyHP = val;
            enemyHpSlider.value = enemyHP;
        }).setOnComplete(() =>
        {
            if (updatedHP <= 0)
            {
                Destroy(this.GetComponent<Enemy>().gameObject);
            }
        });
    }
    
}
