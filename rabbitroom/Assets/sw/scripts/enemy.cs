﻿using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour
{
	public GameObject m_firePrefab = null;
	public GameObject m_shotPrefab = null;
	public GameObject startPos = null;
	public GameObject xwing = null;
	public float speed = 9.9f;
	public float fireSpeed = 3.5f;

	private Vector3 endPos = Vector3.zero;
	private bool isDead = false;

	private Transform thisTransform = null;
	private Rigidbody thisRigidBody = null;
	private Transform enemyMesh = null;

	public int enemyIndex = 0;

	void Start ()
	{
		thisTransform = this.transform;
		thisRigidBody = this.GetComponent<Rigidbody>();

		enemyMesh = thisTransform.GetChild(0);

		fireSpeed = Random.Range(1.5f, 3.5f);
		float x = (enemyIndex % 5 == 0) ? 0.0f : Random.Range(-900.00f, 900.00f);	//left-right
		float y = startPos.transform.position.y + 33.9f;							//up-down
		float z = xwing == null ? -99.0f : (xwing.transform.position.z - 999.9f);
		//x = 0.0f;	//T.ODO: remove this line when collision is working! (check rigidBody.isKinematic)
		endPos = new Vector3(x, y, z);
		this.thisTransform.position = new Vector3(x, y, thisTransform.position.z/*startPos.transform.position.z/*ca 3000*/);
		StartCoroutine(shoot(thisTransform));
	}
	
	void Update ()
	{
		//if(isDead)return;

		float maxz = (xwing != null) ? xwing.transform.position.z : 99.9f;

		//if((thisTransform.position.z - maxz) < -399.99f)
		if(thisTransform.position.z <= (endPos.z))
		{
			Destroy(this.gameObject);
			return;
		}

		float step = speed * Time.deltaTime;
		thisTransform.position = Vector3.MoveTowards(thisTransform.position, endPos, step);
	}

	private IEnumerator shoot(Transform thisTransform)
	{
		while(true)
		{
			yield return new WaitForSeconds(fireSpeed);
			for(int i=0; i<3; i++)
			{
				shoot();
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	private void shoot()
	{
		if(isDead)	//T.ODO: activate
			return;
		GameObject go = GameObject.Instantiate(m_shotPrefab, thisTransform.position, thisTransform.rotation) as GameObject;
		go.layer = 	LayerMask.NameToLayer("enemy"); 
		//go.tag = "shotfromenemy";
		GameObject.Destroy(go, 3f);
	}

	void OnCollisionEnter(Collision col)
	{
		Physics.IgnoreCollision(col.collider, this.GetComponent<Collider>());
		this.thisRigidBody.velocity = this.thisRigidBody.angularVelocity = Vector3.zero;
		if(isDead)return;
		//if (col.gameObject.tag == "shotfromenemy") return;
		isDead = true;
		//if (col.gameObject.tag == "shot")
		{
			if (col.gameObject.tag == "shot")
			{
				Destroy(col.gameObject);
			}
			endPos.y += 3333.3f;
			StopCoroutine(shoot(thisTransform));
			startFire();
			StartCoroutine(startDying(thisTransform));
		}
	}
	void startFire()
	{
		Vector3 pos =  thisTransform.position;
		pos.z -= 99.9f;
		GameObject fire = GameObject.Instantiate(m_firePrefab, pos, enemyMesh.rotation) as GameObject;
		float scale = 3.0f * 222222.2f;
		fire.transform.localScale = new Vector3(scale,scale,scale);
		fire.transform.parent = enemyMesh.GetChild(0);	//=fire.setParent(fireParent);
	}
	private IEnumerator startDying(Transform thisTransform)
	{
		float rotSpeed = 3.3f;
		for(int i=0;i<299;i++)
		{
			yield return new WaitForSeconds(0.001f);
			enemyMesh.Rotate(0.001f, 0.001f, rotSpeed);
		}
		Destroy(this.gameObject);	//T.ODO: activate
	}

	public void onDied(string isDied)
	{
		this.xwing = null;
		endPos.z = -99.0f;
	}
}
