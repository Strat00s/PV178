using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class LazyEnemy : Enemy
{
    protected float timer = 0;
    private int behaviour = 0;

    //lazy enemy damage implementation
    protected override int CalculateDamage(GameObject target)
    {
        //normal damage when castle, othwersie double
        if (target.GetComponent<Castle>() != null)
            return _damage;
        return _damage * 2;
    }

    override protected void Update()
    {
        timer += Time.deltaTime;

        //stop moving after 5s
        if (timer >= 5.0 && behaviour == 0)
        {
            behaviour = 1;
            _movementComponent.CancelMovement();
        }

        //start moving after 1s pause and reset timer
        if (timer >= 6.0 && behaviour == 1)
        {
            behaviour = 0;
            _movementComponent.MoveAlongPath();
            timer = 0;
        }
    }
}
