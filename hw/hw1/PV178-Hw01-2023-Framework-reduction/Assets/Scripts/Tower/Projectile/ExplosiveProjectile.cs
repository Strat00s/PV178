using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveProjectile : Projectile
{
    override protected void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.gameObject.name);

        //hit everyone in radius
        if (other.gameObject.name == "LazyEnemy(Clone)" || other.gameObject.name == "AggresiveEnemy(Clone)")
        {
            foreach (var collider in Physics.OverlapSphere(this.transform.position, 5, _enemyLayerMask))
                other.gameObject.GetComponent<HealthComponent>().HealthValue -= _damage;
        }

        Destroy(gameObject);    //destroy projectile
    }
}
