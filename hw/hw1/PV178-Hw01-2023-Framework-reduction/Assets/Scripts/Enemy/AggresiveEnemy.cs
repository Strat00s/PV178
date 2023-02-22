using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class AggresiveEnemy : Enemy
{
    private void Update()
    {
        //Physics.SphereCastAll(this.transform.position, 10, )
    }
}
