using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform listenerTf;

	public Vector3 posOffset;
	public Vector3 lookOffset;

	public float followSpeed;


	private void LateUpdate()
	{

		//if (PlayerMovement.Local is PlayerMovement p && p != null)
		//{
		//	transform.position = p.transform.position + posOffset;
		//	transform.LookAt(p.transform.position + lookOffset);
		//	listenerTf.position = p.transform.position + lookOffset;
		//}

		if (listenerTf)
		{

			if (Vector3.Distance(transform.position , listenerTf.position + lookOffset + posOffset) < 5)
			{

				listenerTf = null;
			}
			else { transform.position = Vector3.MoveTowards(transform.position ,listenerTf.position + lookOffset + posOffset,Time.deltaTime * followSpeed); }
		}

	}
}
