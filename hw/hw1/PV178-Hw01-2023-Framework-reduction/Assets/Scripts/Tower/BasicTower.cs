using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class BasicTower : Tower
{
    override protected GameObject GetTarget()
    {
        float nearestDistance = float.MaxValue;
        GameObject nearestTarget = null;

        //iterate through colliders in "_range" on layer "Unit"
        foreach (var collider in Physics.OverlapSphere(this.transform.position, _range, _enemyLayerMask))
        {
            //return old target if it is still in range (or alive)
            if (this.target == collider.gameObject)
                return this.target;

            float distance = (collider.transform.position - this.transform.position).sqrMagnitude;    //get squared distance

            //select closest target
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget   = collider.gameObject;
            }
        }

        if (target != nearestTarget && target != null)
            target.GetComponent<ObjectSelector>().DeselectObject();
        if (nearestTarget != null)
            nearestTarget.GetComponent<ObjectSelector>().SelectObject();

        return nearestTarget;
    }
}
