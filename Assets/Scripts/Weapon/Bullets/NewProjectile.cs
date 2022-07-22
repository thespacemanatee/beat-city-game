using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class NewProjectile : Projectile
{
    private int flipTime = 0;
    private bool canFlip = true;
    void OnTriggerEnter(Collider other)
    {
        if (flipTime >= 3)
        {
            canFlip = false;
        }
        //TODO: CHANGE PLAYER INTO OBSTACLES FOR BOUNCING PROJECTILE
        if (other.gameObject.CompareTag("Wall") && canFlip)
        {
            Flip();
            flipTime += 1;
        }
    }
}
