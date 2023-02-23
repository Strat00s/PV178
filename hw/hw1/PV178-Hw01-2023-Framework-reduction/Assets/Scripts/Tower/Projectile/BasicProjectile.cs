using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicProjectile : Projectile
{
    //projectile specific ontrigger implementation
    override protected void OnTriggerEnter(Collider other)
    {
        //hit enemy
        if ((_enemyLayerMask.value & 1 << other.gameObject.layer) != 0)
            other.gameObject.GetComponent<HealthComponent>().HealthValue -= _damage;

        Destroy(gameObject);    //destroy projectile
    }
}
