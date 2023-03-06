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


    public event Action OnDeath;

    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
        _movementComponent.MoveAlongPath();
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    //default damage implementation
    protected virtual int CalculateDamage(GameObject target)
    {
        return _damage;
    }

    //reduce damage on crash and destroy self
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<HealthComponent>().HealthValue -= CalculateDamage(collision.gameObject);
        if (collision.gameObject.GetComponent<Castle>() != null)
            Instantiate(_onSuccessParticlePrefab, this.transform.position, Quaternion.identity);
        _reward = 0;
        this._healthComponent.HealthValue = 0;
    }

    public void Init(EnemyPath path)
    {
        _movementComponent.Init(path, _speed);
    }

    virtual protected void Update()
    {
        return;
    }

    protected void HandleDeath()
    {
        //add reward only on kill
        GameObject.FindObjectOfType<Player>().Resources += _reward;
        OnDeath?.Invoke();
        Instantiate(_onDeathParticlePrefab, this.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
