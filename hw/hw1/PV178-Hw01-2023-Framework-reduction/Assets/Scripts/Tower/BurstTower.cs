using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class BurstTower : Tower
{
    override protected GameObject getTarget()
    {
        int highestHealth = int.MinValue;
        GameObject healthiestTarget = null;

        //iterate through colliders in "_range" on layer "Unit"
        foreach (var colllider in Physics.OverlapSphere(this.transform.position, _range, LayerMask.GetMask("Unit")))
        {
            //return current target if still in range (or alive)
            if (this.target == colllider.gameObject)
                return this.target;

            int health = colllider.GetComponent<HealthComponent>().HealthValue; //get target health
            
            //select healthiest target
            if (health > highestHealth)
            {
                highestHealth    = health;
                healthiestTarget = colllider.gameObject;
            }
        }

        return healthiestTarget;
    }
}
