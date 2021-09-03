using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Refrences")]
    public SliderBar staminaBar;

    [Header("Health")]
    public float health;
    public float maxHealth;
    public bool alive;

    [Header("Stamina System")]
    public float stamina;
    public float maxStamina = 100f;
    public float staminaRechargeTime = 2f;
    public float staminaRegenRate = 3f;
    private Coroutine regenCoroutine;
    public float sprintStamina = 10f;
    public float slideStamina = 30f;

    private void Start()
    {
        health = maxHealth;
        alive = true;
        stamina = maxStamina;
        staminaBar.SetMax(maxStamina);
        staminaBar.SetValue(stamina);
    }

    private void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        health = 0;
        alive = false;
    }

    public bool TrySprint()
    {
        return UseStamina(sprintStamina * Time.deltaTime);
    }
    public bool TrySlide()
    {
        return UseStamina(slideStamina);
    }

    private bool UseStamina(float amount)
    {
        bool enoughStamina;

        if (stamina - amount >= 0)
        {
            enoughStamina = true;
            stamina -= amount;
            staminaBar.SetValue(stamina);

            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            regenCoroutine = StartCoroutine(RegenStamina(staminaRechargeTime, staminaRegenRate));
        }
        else
        {
            enoughStamina = false;
            staminaBar.Pulse(0.2f);
        }

        return enoughStamina;
    }

    private IEnumerator RegenStamina(float wait, float rate)
    {
        yield return new WaitForSeconds(wait);

        while (stamina < maxStamina)
        {
            stamina += maxStamina * rate * Time.deltaTime;
            staminaBar.SetValue(stamina);
            yield return null;
        }

        stamina = maxStamina;
        regenCoroutine = null;
    }

}
