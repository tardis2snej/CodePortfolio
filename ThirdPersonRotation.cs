using UnityEngine;

namespace Bimicore.BSG.ThirdPerson
{
	public class ThirdPersonRotation : MonoBehaviour
	{
		public bool IsSameHorizontalDirection (float firstDirection, float secondDirection)
		{
			if (firstDirection == 0 || secondDirection == 0)
				return true;

			return Mathf.Sign (firstDirection) == Mathf.Sign (secondDirection);
		}
		
		public void TryAlignHorizontalLookRotationWith (float horizontalDirection)
		{
			if (horizontalDirection == 0)
				return;
			
			if (IsFacingHorizontalMovementDirection (horizontalDirection, transform.eulerAngles))
				return;

			transform.rotation = GetReversedHorizontalRotation (horizontalDirection);
		}

		// Use rotation initVelocityAngle in range [-179; 180]. Positive will indicate right direction, negative - left.
		private bool IsFacingHorizontalMovementDirection (float horizontalMovementDirection, Vector3 rotation) =>
			Mathf.Sign (horizontalMovementDirection) == Mathf.Sign (rotation.y > 180 ? rotation.y - 360 : rotation.y);

		private Quaternion GetReversedHorizontalRotation (float horizontalDirection)
		{
			Vector3 rotation = transform.eulerAngles;
			rotation.y += 180 * horizontalDirection;
			rotation.y = ClampAngleInRange_Minus179_Plus180 (rotation.y);

			return Quaternion.Euler (rotation);
		}

		/// Returns the same initVelocityAngle as given, but in range [-179; 180]
		private float ClampAngleInRange_Minus179_Plus180 (float initialAngle)
		{
			float angle = initialAngle %= 360;

			if (initialAngle > 180)
				return angle % 180 - 180;

			if (initialAngle <= -180)
				return angle % 180 + 180;

			return angle;
		}
	}
}