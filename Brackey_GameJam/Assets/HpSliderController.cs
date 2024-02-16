using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSliderController : MonoBehaviour
{
    public Slider hpSlider;
    public Image fillHp;
    
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float currentHP;

    public Color beginColor;
    public Color endColor;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
        beginColor = fillHp.color;
    }
    
    public void OneDmg()
    {
        TakeDamage(1);
    }

    public void TakeDamage(float dmg)
    {

        LeanTween.value(currentHP, currentHP - dmg, 0.2f).setOnUpdate((float val) =>
        {
            currentHP = val;
            hpSlider.value = currentHP;
        });
    }

    public void AddHP(float heal)
    {
        LeanTween.value(currentHP, Mathf.Clamp(currentHP + heal, 0, maxHP), 0.2f).setOnUpdate((float val) =>
          {
              currentHP = val;
              hpSlider.value = currentHP;
          });
    }
    

}