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

    protected float _timer;
    protected int _deathCause;

    enum deathType
    {
        NONE = 0,
        PROJECTILE,
        CRASH
    }

    public event Action OnDeath;

    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
        _timer = 0;
        _deathCause = (int)deathType.NONE;
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < 5.0)
            _movementComponent.MoveAlongPath();
        else if (_timer < 6.0)
            _movementComponent.CancelMovement();
        else
            _timer = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Object that collided with me: " + collision.gameObject.name);
        //tower collision
        if (collision.gameObject.name == "RandomTower" || collision.gameObject.name == "BurstTower" || collision.gameObject.name == "BasicTower")
            collision.gameObject.GetComponent<HealthComponent>().HealthValue -= this.gameObject.name == "LazyEnemy" ? _damage * 2 : _damage;

        //castle collision
        if (collision.gameObject.name == "Castle")
            collision.gameObject.GetComponent<HealthComponent>().HealthValue -= _damage;

        _deathCause = (int)deathType.CRASH;
        this._healthComponent.HealthValue = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    public void Init(EnemyPath path)
    {
        // TODO: Modify this so they have appropriate speed
        _movementComponent.Init(path, _speed);
    }

    protected void HandleDeath()
    {
        // TODO: Modify this so they give appropriate reward
        if (_deathCause == (int)deathType.PROJECTILE)
            GameObject.FindObjectOfType<Player>().Resources += _reward;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
