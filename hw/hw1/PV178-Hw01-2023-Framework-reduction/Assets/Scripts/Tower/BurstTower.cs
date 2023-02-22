using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class BurstTower : Tower
{
    //async function to wait without halting the entire game
    async override protected void Fire()
    {
        CreateProjectile();
        await System. Threading.Tasks.Task.Delay(200);  //waot 0.2 seconds
        CreateProjectile();
    }

    override protected GameObject GetTarget()
    {
        int highestHealth = int.MinValue;
        GameObject healthiestTarget = null;

        //iterate through colliders in "_range" on layer "Unit"
        foreach (var colllider in Physics.OverlapSphere(this.transform.position, _range, _enemyLayerMask))
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

        if (target != healthiestTarget && target != null)
            target.GetComponent<ObjectSelector>().DeselectObject();
        if (healthiestTarget != null)
            healthiestTarget.GetComponent<ObjectSelector>().SelectObject();

        return healthiestTarget;
    }
}
