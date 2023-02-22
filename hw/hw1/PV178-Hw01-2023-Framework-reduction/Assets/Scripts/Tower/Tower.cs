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
    //[SerializeField] protected Projectile _projectile;
    [SerializeField] protected float _timeBetweenShots;
    //[SerializeField] protected int _health;

    protected float timer = 0;
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


    virtual protected GameObject getTarget()
    {
        return null;
    }

    virtual protected void Update()
    {
        target = getTarget();
        if (target is null)
            return;

        Debug.Log("ID: " + target.GetInstanceID());
        Debug.Log("Range: " + (target.transform.position - this.transform.position).sqrMagnitude);
        Debug.Log("Health: " + target.GetComponent<HealthComponent>().HealthValue);
    }


    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
