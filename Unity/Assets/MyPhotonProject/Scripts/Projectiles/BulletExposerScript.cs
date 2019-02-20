using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExposerScript : MonoBehaviour
{
	[SerializeField]
	private HitboxExposerScript triggerExposer;
    
	[SerializeField]
	private MeshRenderer targetMeshRenderer;
    
	[SerializeField]
	private Rigidbody targetRigidBody;
    
	[SerializeField]
	private Transform targetTransform;
    
	[SerializeField]
	private Collider targetCollider;

	private bool destroy;
	private float lifeTime;
	private float popTime;

	void Start()
	{
		destroy = false;
		lifeTime = 2.0f;
		popTime = 0.0f;
		triggerExposer.Subscribe(MyOnTriggerEnter);
	}

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
		destroy = false;
		popTime = Time.time;
	}

	public void Disable()
	{
		targetCollider.enabled = false;
        targetRigidBody.isKinematic = true;
        targetMeshRenderer.enabled = false;
		destroy = false;
	}

	public void SetParentReference(Vector3 position, Vector3 velocity, Quaternion rotation)
	{
		targetTransform.position = position;
		targetRigidBody.velocity = velocity * 250;
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

	public int GetDamage()
	{
		return 30;
	}

	void Update()
	{
		if (Time.time - popTime > lifeTime)
			destroy = true;
	}

	void OnDestroy()
	{
		triggerExposer.UnSubscribe();
	}
}
