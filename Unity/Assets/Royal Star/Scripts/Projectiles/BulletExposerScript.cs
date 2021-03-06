﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExposerScript : MonoBehaviour
{
	[SerializeField] private int damage;
    [SerializeField] private float lifeTime;
    [SerializeField] private HitboxExposerScript triggerExposer;
	[SerializeField] private MeshRenderer targetMeshRenderer;
	[SerializeField] private Rigidbody targetRigidBody;
	[SerializeField] private Transform targetTransform;
	[SerializeField] private Collider targetCollider;
    [SerializeField] public ParticleSystem particules;
	private bool destroy = false;
	private float popTime = 0.0f;
	private Vector3 lastPosition;

	void MyOnTriggerEnter(Collider other)
	{
		destroy = true;
	}

	public bool GetDestroy()
	{
		return destroy;
	}

	public void Enable()
	{
		targetRigidBody.velocity = Vector3.zero;
        targetCollider.enabled = true;
        targetRigidBody.isKinematic = false;
        targetMeshRenderer.enabled = true;
		triggerExposer.Subscribe(MyOnTriggerEnter);
		lastPosition = transform.position;

        if (particules != null)
        {
            //particules.gameObject.SetActive(false);
        }
        
        destroy = false;
		popTime = Time.time;
	}

	public void Disable()
	{
		targetCollider.enabled = false;
        targetRigidBody.isKinematic = true;
        targetMeshRenderer.enabled = false;
		triggerExposer.UnSubscribe();
		lastPosition = Vector3.zero;
        
        if (particules != null)
        {
            particules.gameObject.SetActive(false);
        }
		destroy = false;
	}

	public void SetParentReference(Vector3 position, Vector3 velocity, Quaternion rotation)
	{
		targetTransform.position = position;
		targetRigidBody.velocity = velocity;
		targetTransform.rotation = rotation;
		targetTransform.Rotate(new Vector3(-90, -90, -90));
	}

	public void SetPosition(Vector3 position)
	{
		targetTransform.position = position;
	}

	public void SetVelocity(Vector3 velocity)
	{
		targetRigidBody.velocity = velocity;
	}

	public void SetParentRotation(Quaternion rotation)
	{
		targetTransform.rotation = rotation;
		targetTransform.Rotate(new Vector3(-90, -90, -90));
	}

	private void PredictCollision()
	{
		RaycastHit hit;
	
		if (Physics.Raycast(transform.forward, transform.forward, out hit, CalculateDeltaDistance()))
        {
            if(hit.transform.CompareTag("Player"))
            {
                destroy = true;
                hit.transform.gameObject.GetComponent<ShipExposer>()?.TakeDamage(damage);
            }
            if (hit.transform.CompareTag("Bot"))
            {
                destroy = true;
                hit.transform.gameObject.GetComponent<EnemyExposer>()?.TakeDamage(damage);
            }
        }
	}

	private void SetLastPosition()
	{
		if (targetCollider.enabled)
			lastPosition = transform.position;
	}

	private float CalculateDeltaDistance()
	{
		return lastPosition != Vector3.zero ? (transform.position - lastPosition).magnitude : 0;
	}

	public int GetDamage()
	{
		return damage;
	}

	void Update()
	{
		if (Time.time - popTime > lifeTime)
			destroy = true;
	}

	void FixedUpdate()
	{
		SetLastPosition();
		PredictCollision();
	}

	void OnDestroy()
	{
		triggerExposer.UnSubscribe();
	}
}
