using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform listenerTf;

	public Vector3 posOffset;
	public Vector3 lookOffset;

	public float followSpeed;

	public void SetTransformToFollow(Transform _follow)
	{
		posOffset = Vector3.zero;
		listenerTf = _follow;
	}

	public void SetPositionOffset(Vector3 _posOffset)
	{
		posOffset = _posOffset;

	}

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

			if (Vector3.Distance(transform.position, listenerTf.position + lookOffset + posOffset) < Mathf.Abs(posOffset.z * 0.5f))
			{

				listenerTf = null;
			}
			else { transform.position = Vector3.MoveTowards(transform.position, listenerTf.position + lookOffset + posOffset, Time.deltaTime * followSpeed); }
		}
		else
		{
			if (posOffset != Vector3.zero)
			{
				Vector3 newposition = transform.position + posOffset;
				newposition = new Vector3(Mathf.Clamp(newposition.x, 0, GameConstants.BOUNDARY_X), newposition.y, Mathf.Clamp(newposition.z, 0, GameConstants.BOUNDARY_Z));

				transform.position = Vector3.MoveTowards(transform.position, newposition, Time.deltaTime * followSpeed);

			}
		}

	}
}
