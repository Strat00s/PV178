using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class RandomTower : Tower
{
    //random tower fire implementation
    override protected void Fire()
    {
        //handle shots timing
        if (timer < _timeBetweenShots)
            return;
        timer = 0;

        int[] values = {0, 0, 1, 1, 1, 1, 1, 1, 2, 2};  //20% change of nothing, 60% change of single shot, 20% change of 2 shots
        for (int i = 0; i < values[Random.Range(0, values.Length)]; i++)
            CreateProjectile();
    }

    //random tower target acquirement implementation
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
