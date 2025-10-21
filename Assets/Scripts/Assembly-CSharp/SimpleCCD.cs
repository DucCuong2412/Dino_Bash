using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleCCD : MonoBehaviour
{
	[Serializable]
	public class Node
	{
		public Transform Transform;

		public float min;

		public float max;
	}

	public int iterations = 5;

	[Range(0.01f, 1f)]
	public float damping = 1f;

	public Transform target;

	public Transform endTransform;

	public Node[] angleLimits = new Node[0];

	private Dictionary<Transform, Node> nodeCache;

	private void OnValidate()
	{
		Node[] array = angleLimits;
		foreach (Node node in array)
		{
			node.min = Mathf.Clamp(node.min, 0f, 360f);
			node.max = Mathf.Clamp(node.max, 0f, 360f);
		}
	}

	private void Start()
	{
		nodeCache = new Dictionary<Transform, Node>(angleLimits.Length);
		Node[] array = angleLimits;
		foreach (Node node in array)
		{
			if (!nodeCache.ContainsKey(node.Transform))
			{
				nodeCache.Add(node.Transform, node);
			}
		}
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			Start();
		}
		if (!(target == null) && !(endTransform == null))
		{
			for (int i = 0; i < iterations; i++)
			{
				CalculateIK();
			}
			endTransform.rotation = target.rotation;
		}
	}

	private void CalculateIK()
	{
		Transform parent = endTransform.parent;
		while (true)
		{
			RotateTowardsTarget(parent);
			if (parent == base.transform)
			{
				break;
			}
			parent = parent.parent;
		}
	}

	private void RotateTowardsTarget(Transform transform)
	{
		Vector2 vector = target.position - transform.position;
		Vector2 vector2 = endTransform.position - transform.position;
		float num = SignedAngle(vector2, vector);
		num *= Mathf.Sign(transform.root.localScale.x);
		num *= damping;
		num = 0f - (num - transform.eulerAngles.z);
		if (nodeCache.ContainsKey(transform))
		{
			Node node = nodeCache[transform];
			float num2 = ((!transform.parent) ? 0f : transform.parent.eulerAngles.z);
			num -= num2;
			num = ClampAngle(num, node.min, node.max);
			num += num2;
		}
		transform.rotation = Quaternion.Euler(0f, 0f, num);
	}

	public static float SignedAngle(Vector3 a, Vector3 b)
	{
		float num = Vector3.Angle(a, b);
		float num2 = Mathf.Sign(Vector3.Dot(Vector3.back, Vector3.Cross(a, b)));
		return num * num2;
	}

	private float ClampAngle(float angle, float min, float max)
	{
		angle = Mathf.Abs(angle % 360f + 360f) % 360f;
		return Mathf.Clamp(angle, min, max);
	}
}
