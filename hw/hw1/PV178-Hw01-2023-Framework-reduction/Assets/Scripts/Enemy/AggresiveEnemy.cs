using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class AggresiveEnemy : Enemy
{
    GameObject target     = null;
    private int behaviour = 0;

    override protected void Update()
    {
        //move to target untill it is destroyed and skip finding new target
        if (target != null)
        {
            if (behaviour == 0)
            {
                behaviour = 1;
                _movementComponent.MoveTowards(target.transform);
            }
            return;
        }
        else
        {
            if (behaviour == 1)
            {
                behaviour = 0;
                _movementComponent.MoveAlongPath();
            }
        }
        //search and pick first possible target in range
        foreach (var collider in Physics.OverlapSphere(this.transform.position, 10, _attackLayerMask))
        {
            //filter only towers
            if (collider.GetComponent<Tower>() != null) {
                target = collider.gameObject;
                return;    
            }
        }
    }
}
