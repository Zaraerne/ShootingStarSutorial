﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    private void Awake()
    {
        if(moveDirction != Vector2.left)
        {
            transform.rotation = Quaternion.FromToRotation(Vector2.left, moveDirction);
        }
    }
}
