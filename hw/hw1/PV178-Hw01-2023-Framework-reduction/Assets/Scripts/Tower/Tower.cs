using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class Tower : MonoBehaviour
{
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] protected Transform _objectToPan;
    [SerializeField] protected Transform _projectileSpawn;
    [SerializeField] private GameObject _previewPrefab;


    [SerializeField] protected string _name;
    [SerializeField] protected float _range;
    [SerializeField] protected int _price;
    [SerializeField] protected float _timeBetweenShots;

    protected float timer;
    protected GameObject target;


    public HealthComponent Health => _healthComponent;
    public BoxCollider Collider => _boxCollider;
    public GameObject BuildingPreview => _previewPrefab;

    public int Price => _price;

    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
        timer = float.MaxValue;
        target = null;
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    //default target acquirement implementation
    virtual protected GameObject GetTarget()
    {
        return null;
    }

    //create projectile
    protected void CreateProjectile()
    {
        //create projectile in remove it's parent
        var projectile = Instantiate(_projectilePrefab, _projectileSpawn.transform.position, _projectileSpawn.transform.rotation);
        projectile.transform.parent = null;
    }

    //default fire implementation
    virtual protected void Fire()
    {
        //handle shots timing
        if (timer < _timeBetweenShots)
            return;
        timer = 0;

        CreateProjectile();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        //get new target (if there is any)
        target = GetTarget();
        if (target is null)
            return;

        _objectToPan.transform.LookAt(target.transform);    //"""tracking""""


        //fire if possible
        Fire();
    }


    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
