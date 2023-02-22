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

    protected float timer = float.MaxValue;
    protected GameObject target = null;


    public HealthComponent Health => _healthComponent;
    public BoxCollider Collider => _boxCollider;
    public GameObject BuildingPreview => _previewPrefab;

    public int Price => _price;

    private void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }


    virtual protected GameObject GetTarget()
    {
        return null;
    }

    protected void CreateProjectile()
    {
        //create projectile and move it infront of the barrel
        var projectile = Instantiate(_projectilePrefab, _objectToPan.transform.position + (Vector3.up * 1), _objectToPan.transform.rotation);
        projectile.transform.Translate(Vector3.forward * 2);
        projectile.transform.parent = null;
    }

    virtual protected void Fire()
    {
        CreateProjectile();
    }

    virtual protected void Update()
    {
        //get new target (if there is any)
        target = GetTarget();
        if (target is null)
            return;

        _objectToPan.transform.LookAt(target.transform);    //tracking

        //shoot at correct intervals
        timer += Time.deltaTime;
        if (timer >= _timeBetweenShots)
        {
            timer = 0;
            Fire();
        }
    
    }


    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
