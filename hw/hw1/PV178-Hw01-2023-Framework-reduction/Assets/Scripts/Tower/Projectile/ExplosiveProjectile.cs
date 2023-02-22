using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveProjectile : Projectile
{
    override protected void OnTriggerEnter(Collider other)
    {
        //hit everyone in radius
        if (other.gameObject.name == "LazyEnemy(Clone)" || other.gameObject.name == "AggresiveEnemy(Clone)")
        {
            //hit all enemies in 5 radius
            foreach (var collider in Physics.OverlapSphere(this.transform.position, 5, _enemyLayerMask))
            {
                if (collider.gameObject.name == "LazyEnemy(Clone)" || collider.gameObject.name == "AggresiveEnemy(Clone)")
                    collider.gameObject.GetComponent<HealthComponent>().HealthValue -= _damage;
            }
        }

        Destroy(gameObject);    //destroy projectile
    }
}
