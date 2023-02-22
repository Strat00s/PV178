using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicProjectile : Projectile
{
    override protected void OnTriggerEnter(Collider other)
    {
        //hit all enemies
        if (other.gameObject.name == "LazyEnemy(Clone)" || other.gameObject.name == "AggresiveEnemy(Clone)")
            other.gameObject.GetComponent<HealthComponent>().HealthValue -= _damage;

        Destroy(gameObject);    //destroy projectile
    }
}
