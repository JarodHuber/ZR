using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector]
    public bool Paused = false;
    [HideInInspector]
    public int curHealth = 5;

    [Tooltip("player's maximum amount of health")]
    public int maxHealth = 5;
    [Tooltip("delay before healing starts")]
    public float healDelay = 2;
    [Tooltip("amount of time healing to full takes")]
    public float healLength = 1;
    [Tooltip("lenght of time before you can be hit again")]
    public float invincibleLength = .2f;
    [Tooltip("List of sounds played when the player gets hit")]
    public List<AudioClip> damageSounds = new List<AudioClip>();

    Timer timerToHeal, timerForHeal;

    bool lowestHealthLock = false;
    int lowestHealth;

    AudioSource audioSource;

    private void Awake()
    {
        timerToHeal = new Timer(healDelay);
        timerForHeal = new Timer(healLength);
        audioSource = GetComponent<AudioSource>();
        curHealth = maxHealth;
    }

    private void Update()
    {
        if (Paused) return;

        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);

        if (curHealth != maxHealth)
        {
            if (curHealth > 0)
            {
                if (timerToHeal.Check(false))
                {
                    if (!lowestHealthLock)
                    {
                        lowestHealth = curHealth;
                        lowestHealthLock = true;
                    }

                    timerForHeal.CountByTime();
                    curHealth = (int)Mathf.Lerp(lowestHealth, maxHealth, timerForHeal.PercentComplete());
                }
            }
            else
                Die();
        }
    }

    /// <summary>
    /// make the player take damage
    /// </summary>
    /// <param name="dmg">amount of damage for the player to take</param>
    public void TakeDamage(int damageSound = 0, int dmg = 1)
    {
        if(!audioSource.isPlaying)
        {
            curHealth = Mathf.Clamp(curHealth-dmg, 0, maxHealth);
            timerToHeal.Reset();
            timerForHeal.Reset();
            lowestHealthLock = false;

            audioSource.clip = damageSounds[damageSound];
            audioSource.Play();
        }
    }

    /// <summary>
    /// Take damage from the grenade
    /// </summary>
    public void GrenadeDamage()
    {
        curHealth = 1;
        timerToHeal.Reset();
        timerForHeal.Reset();
        lowestHealthLock = false;
    }

    /// <summary>
    /// kill the player
    /// </summary>
    public void Die()
    {
        SceneManager.LoadScene("GameOver");
    }
}
