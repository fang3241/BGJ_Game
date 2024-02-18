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
    PlayerMovement player;

    public Color beginColor;
    public Color endColor;
    bool isLerping = false;

    public GameObject bgred;
    public GameObject gameOver;
    public GameObject instantiatedPanel;
    public Transform canvasTransform;
    GameObject panelGameOver;
    public float currentLerpTime;
    Image bg;

    
    public void Setup(PlayerMovement pl, float hp)
    {
        player = pl;
        maxHP = hp;
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
            player.playerHP = currentHP;
        });

        if(currentHP <= 1){
            Debug.Log("DEADD");
            //lerp warna layar
            //show lose panel
            AudioManager.instance.StopAllandPlay("LoseBGM");
            // Start lerping the canvas alpha
            isLerping = true;
        }
    }

    public void AddHP(float heal)
    {
        LeanTween.value(currentHP, Mathf.Clamp(currentHP + heal, 0, maxHP), 0.2f).setOnUpdate((float val) =>
          {
              currentHP = val;
              hpSlider.value = currentHP;
              player.playerHP = currentHP;
          });
    }

    void Start(){
        instantiatedPanel = Instantiate(bgred, canvasTransform);
        instantiatedPanel.SetActive(false);
        panelGameOver = Instantiate(gameOver, canvasTransform);
        
        // Get the CanvasGroup component of the instantiated panel
        bg = panelGameOver.GetComponent<Image>();
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0f);
        panelGameOver.SetActive(false);
    }

    void Update(){
        if (isLerping)
        {
            // Update the current lerp time
            currentLerpTime += Time.deltaTime;
            instantiatedPanel.SetActive(false);
            panelGameOver.SetActive(true);
            // Calculate the lerp t value
            float t = currentLerpTime;

            // Lerp the canvas alpha
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, Mathf.Lerp(0f, 1f, t));
            //bg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            Debug.Log("XP");
            // Check if the lerp is complete
            if (currentLerpTime >= 1)
            {
                Debug.Log("XXP");
                // Ensure that the alpha reaches exactly 1
                //bg.alpha = targetAlpha;
                bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, Mathf.Lerp(0f, 1f, t));

                // Activate the child GameObject of the panel
                
                GameObject _player = GameObject.Find("Player(Clone)");
                _player.SetActive(false);
            }
        }
    }
    

}
