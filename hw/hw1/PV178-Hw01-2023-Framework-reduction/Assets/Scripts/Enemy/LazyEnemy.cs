using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class LazyEnemy : Enemy
{
    protected float timer = 0;
    private bool move = true;

    //lazy enemy damage implementation
    protected override int calculateDamage(GameObject target)
    {
        //normal damage when castle, othwersie double
        if (target.GetComponent<Castle>() != null)
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
