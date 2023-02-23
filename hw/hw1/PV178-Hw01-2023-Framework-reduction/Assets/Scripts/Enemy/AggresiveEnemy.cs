using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class AggresiveEnemy : Enemy
{
    GameObject target = null;

    private void Update()
    {
        //move to target (does not have to be called every update) and skip finding new target
        if (target != null)
        {
            _movementComponent.MoveTowards(target.transform);
            return;
        }
        else
            _movementComponent.MoveAlongPath(); //follow the path again

        //search and pick first possible target in range
        foreach (var collider in Physics.OverlapSphere(this.transform.position, 10, _attackLayerMask))
        {
            //filter only turrets
            if (collider.gameObject.name != "Castle")
                target = collider.gameObject;
        }
    }
}
