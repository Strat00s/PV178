using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected MovementComponent _movementComponent;
    [SerializeField] protected HealthComponent _healthComponent;
    [SerializeField] protected ParticleSystem _onDeathParticlePrefab;
    [SerializeField] protected ParticleSystem _onSuccessParticlePrefab;
    [SerializeField] protected LayerMask _attackLayerMask;


    [SerializeField] protected int _damage;
    [SerializeField] protected int _reward;
    [SerializeField] protected int _speed;

    enum deathType
    {
        NONE = 0,
        PROJECTILE,
        CRASH
    }

    protected int deathCause;


    public event Action OnDeath;

    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
        deathCause = (int)deathType.NONE;
        _movementComponent.MoveAlongPath();
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    protected virtual int calculateDamage(GameObject target)
    {
        return _damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<HealthComponent>().HealthValue -= calculateDamage(collision.gameObject);
        deathCause = (int)deathType.CRASH;
        this._healthComponent.HealthValue = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void Init(EnemyPath path)
    {
        _movementComponent.Init(path, _speed);
    }

    protected void HandleDeath()
    {
        if (deathCause == (int)deathType.PROJECTILE)
            GameObject.FindObjectOfType<Player>().Resources += _reward;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
