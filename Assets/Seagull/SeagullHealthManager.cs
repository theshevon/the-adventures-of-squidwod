using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeagullHealthManager : MonoBehaviour {

    public int seagullHealth;
    public GameObject egg;
    public Material seagullMaterial;
    Color damageColour = new Color(1.0f, 0.57f, 0.57f, 1.0f);
    public TextMeshProUGUI seagullHealthValue;
    bool isDamaged;
    public int damageTaken;
    int criticalDamage;
    int stdDamage;
    AudioSource audioSrc;
    public AudioClip[] seagullHurt;
    public Image seagullHealthBar;

    void Start()
    {
        seagullHealth = 100;
        audioSrc = GetComponent<AudioSource>();
        switch (gameObject.GetComponent<GameSettings>().GetDifficulty()){
            case 0:
                criticalDamage = 10;
                stdDamage = 7;
                break;
            case 1:
                criticalDamage = 5;
                stdDamage = 3;
                break;
        }
    }

    void Update()
    {
        seagullHealthValue.SetText(seagullHealth.ToString());
        seagullHealthBar.fillAmount = seagullHealth / 100f;
    }

    public void Hit(bool isCritical)
    {
        if (isCritical && !isDamaged)
        {
            Debug.Log("critical hit");
            StartCoroutine(TakeDamage(criticalDamage));
        } else if (!isCritical && !isDamaged)
        {
            Debug.Log("hit");
            StartCoroutine(TakeDamage(stdDamage));
        }
        
    }

    IEnumerator TakeDamage(int damage)
    {
        PlayHurtSound();
        seagullHealth -= damage;
        damageTaken += damage;
        seagullMaterial.SetColor("_Color", damageColour);
        isDamaged = true;
        yield return new WaitForSeconds(0.5f);
        seagullMaterial.SetColor("_Color", Color.white);
        isDamaged = false;
    }
    
    void PlayHurtSound()
    {
        int index = Random.Range(0, seagullHurt.Length);
        Debug.Log(index);
        audioSrc.PlayOneShot(seagullHurt[index]);
    }
}
