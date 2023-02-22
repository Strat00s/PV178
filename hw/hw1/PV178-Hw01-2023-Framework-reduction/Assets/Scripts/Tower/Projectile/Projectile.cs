using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] protected ParticleSystem _onHitParticleSystem;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _speed;
    [SerializeField] protected float _lifetime;

    protected float timer;

    private void Start()
    {
        timer = 0;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {

    }

    private void Update()
    {
        this.transform.Translate(Vector3.forward * Time.deltaTime * _speed * 4);    //move projectile in a straight line

        //destroy projectile when lifetime ends
        timer += Time.deltaTime;
        if (timer >= _lifetime)
            Destroy(gameObject);
    }
}
