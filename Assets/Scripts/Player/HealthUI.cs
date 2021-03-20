using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Vector2 healthLowestAndHighest;

    Image bloodyOverlay;
    Color overlayColor, maxColor, minColor;
    float healthRatio;

    private void Start()
    {
        bloodyOverlay = GetComponent<Image>();
        overlayColor = bloodyOverlay.color;
        maxColor = overlayColor;
        maxColor.a = healthLowestAndHighest.y;
        minColor = overlayColor;
        minColor.a = healthLowestAndHighest.x;
    }

    private void Update()
    {
        healthRatio = (float)playerHealth.curHealth / (float)playerHealth.maxHealth;
        overlayColor = Color.Lerp(maxColor, minColor, healthRatio);

        bloodyOverlay.color = overlayColor;
    }
}
