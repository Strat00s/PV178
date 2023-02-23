using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class RandomTower : Tower
{
    override protected void Fire()
    {
        int[] values = {0, 0, 1, 1, 1, 1, 1, 1, 2, 2};

        //pick with probability
        switch (values[Random.Range(0, values.Length)])
        {
            case 0: CreateProjectile(); CreateProjectile(); break;  //shoot twice
            case 1: CreateProjectile(); break;                      //shoot once
            case 2: break;                                          //dont shoot
        }
    }

    //custom target acquirement implementation
    override protected GameObject GetTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, _range, _enemyLayerMask); //get list of targets in "_range" on layer "Unit"

        //return if no target was found
        if (colliders.Length == 0)
            return null;

        //check if current target is still in range (or alive)
        foreach (var collider in colliders)
        {
            if (collider.gameObject == this.target)
                return this.target;
        }

        GameObject newTarget = colliders[Random.Range(0, colliders.Length)].gameObject; //return random target from the list

        if (target != newTarget && target != null)
            target.GetComponent<ObjectSelector>().DeselectObject();
        if (newTarget != null)
            newTarget.GetComponent<ObjectSelector>().SelectObject();

        return newTarget;
    }
}
