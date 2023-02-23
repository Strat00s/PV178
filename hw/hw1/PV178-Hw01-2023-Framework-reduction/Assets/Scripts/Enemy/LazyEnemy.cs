using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class LazyEnemy : Enemy
{
    protected float timer = 0;
    private bool move = true;

    //custom damage implementation
    protected override int calculateDamage(GameObject target)
    {
        if (target.name == "Castle")
            return _damage;
        return _damage * 2;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        //stop moving after 5s
        if (timer >= 5.0 && move)
        {
            move = false;
            _movementComponent.CancelMovement();
        }

        //start moving after 1s pause and reset timer
        if (timer >= 6.0)
        {
            move = true;
            _movementComponent.MoveAlongPath();
            timer = 0;
        }
    }
}
