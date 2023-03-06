using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class BurstTower : Tower
{
    private bool burstShot = false;

    //burst tower fire implementation
    override protected void Fire()
    {
        ////handle shots timing and flip between shot types (first shot and second burst shot)
        if (!burstShot && timer < _timeBetweenShots)
                return;
        else if (burstShot && timer < 0.2)
                return;
        timer = 0;
        burstShot = !burstShot;

        CreateProjectile();
    }

    //burst tower target acquirement implementation
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

        return healthiestTarget;
    }
}
