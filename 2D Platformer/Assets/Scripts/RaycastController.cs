using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	public LayerMask collisionMask;
	public LayerMask interactiveMask;

	public const float skinWidth = .0015f;
	public float distanceBetweenRays = .25f;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D collider;
	public RayCastOrigins raycastOrigins;

	public virtual void Awake(){
		collider = GetComponent<BoxCollider2D> ();
	}

	public virtual void Start(){
		CalculateRaySpacing ();
	}

	public void UpdateRaycastOrigins(bool rotated = false){
		// Find boxcollider bounds.
		if (!rotated) {
			Bounds bounds = collider.bounds;
			bounds.Expand (skinWidth * -2);

			raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
			raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
			raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
			raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
		}
		if (rotated) {
			float top = collider.offset.y + (collider.size.y / 2f);
			float btm = collider.offset.y - (collider.size.y / 2f);
			float left = collider.offset.x - (collider.size.x / 2f);
			float right = collider.offset.x + (collider.size.x /2f);

			raycastOrigins.topLeft = transform.TransformPoint (new Vector3( left, top, 0f));
			raycastOrigins.topRight = transform.TransformPoint (new Vector3( right, top, 0f));
			raycastOrigins.bottomLeft = transform.TransformPoint (new Vector3( left, btm, 0f));
			raycastOrigins.bottomRight = transform.TransformPoint (new Vector3( right, btm, 0f));

		}
	}

	public void CalculateRaySpacing(){
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		float boundsWidth = bounds.size.x;
		float boundsHeight = bounds.size.y;

		horizontalRayCount = Mathf.RoundToInt (boundsHeight / distanceBetweenRays);
		verticalRayCount = Mathf.RoundToInt (boundsWidth / distanceBetweenRays);

		// Subtract 1 because there are n-1 spaces between n points.
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public struct RayCastOrigins {
		// variables for the raycastorigins
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
