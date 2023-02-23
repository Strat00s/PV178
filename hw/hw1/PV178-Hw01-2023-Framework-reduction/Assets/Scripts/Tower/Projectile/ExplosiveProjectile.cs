using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveProjectile : Projectile
{
    //projectile specific ontrigger implementation
    override protected void OnTriggerEnter(Collider other)
    {
        //hit all enemies in 5 radius
        foreach (var collider in Physics.OverlapSphere(this.transform.position, 5, _enemyLayerMask))
        {
            if ((_enemyLayerMask.value & 1 << other.gameObject.layer) != 0)
                collider.gameObject.GetComponent<HealthComponent>().HealthValue -= _damage;
        }

        Destroy(gameObject);    //destroy projectile
    }
}
