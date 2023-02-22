using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class RandomTower : Tower
{

    override protected GameObject getTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, _range, LayerMask.GetMask("Unit")); //get list of targets in "_range" on layer "Unit"

        //return if no target was found
        if (colliders.Length == 0)
            return null;

        //check if current target is still in range (or alive)
        foreach (var collider in colliders)
        {
            if (collider.gameObject == this.target)
                return this.target;
        }

        return colliders[Random.Range(0, colliders.Length)].gameObject; //return random target from the list
    }
}
