using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupPongCanvas : LevelCanvasBehaviour
{
    public Image leftPowerupImage;
    public Image rightPowerupImage;

    private PowerupPongPowerupManager powerupManager;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        powerupManager = FindObjectOfType<PowerupPongPowerupManager>();

        // Reset powerup images
        clearPowerupImage(true);
        clearPowerupImage(false);

        // Assign score colors
        leftScoreText.color = Color.green;
        rightScoreText.color = Color.red;
    }

    public void clearPowerupImage(bool onLeft)
    {
        if (onLeft)
        {
            leftPowerupImage.color = Color.clear;
        }
        else
        {
            rightPowerupImage.color = Color.clear;
        }
    }

    public void setPowerupImage(PowerupPongPowerupManager.PowerupType power, bool onLeft)
    {
        if (onLeft)
        {
            leftPowerupImage.color = Color.white;
            leftPowerupImage.sprite = powerupManager.getSprite(power);
        } 
        else
        {
            rightPowerupImage.color = Color.white;
            rightPowerupImage.sprite = powerupManager.getSprite(power);
        }
        
    }
}
