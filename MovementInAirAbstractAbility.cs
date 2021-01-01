using System.Threading.Tasks;
using UnityEngine;

namespace Bimicore.BSG.ThirdPerson
{
	public abstract class MovementInAirAbstractAbility : MovementAbstractAbility
	{
		[Header ("Movement in air")] 
		[SerializeField] protected float maxHorizontalSpeed = 10f;

		private bool _isInertiaEnabled = false;

		protected override void CommonEnter()
        {
            base.CommonEnter();

			SubscribeOnInput();
        }

		protected override void CommonExit()
		{
			base.CommonExit();

            UnsubscribeFromInput();

			//Stop all velocity changes in this state
			IsIncreasingVelocity = false;
			IsDecreasingVelocity = false;
			_isInertiaEnabled = false;
		}
		
		protected void AddGravity (float acceleration)
		{
			physicalProperties.targetVelocity += Vector3.up * (Physics.gravity.y * acceleration * Time.fixedDeltaTime);
		}

		// Warning: Do not call this method with no input on horizontal axis
		protected async void ChangeVelocityWithInertia()
		{
			_isInertiaEnabled = true;

			if (thirdPersonRotation.IsSameHorizontalDirection (inputSystem.InputHorizontalMovementDirection, 
			                                                   physicalProperties.targetVelocity.x))
			{
				CalculateTargetVelocityDirection (inputSystem.InputHorizontalMovementDirection);
				await TryIncreaseTargetVelocity();
			}
			else
			{
				CalculateTargetVelocityDirection (Mathf.Sign (physicalProperties.targetVelocity.x));
				await TryDecreaseTargetVelocity();

				if (_isInertiaEnabled)
					ChangeVelocityWithInertia();
			}
		}

		private async Task TryIncreaseTargetVelocity()
		{
			IsDecreasingVelocity = false;

			if (!IsIncreasingVelocity)
				await IncreaseTargetVelocity (physicalProperties.accelerationInAir, maxHorizontalSpeed);
		}

		private async Task TryDecreaseTargetVelocity()
		{
			IsIncreasingVelocity = false;

			if (!IsDecreasingVelocity)
				await DecreaseTargetVelocity (physicalProperties.decelerationInAir);
		}

		protected override void CalculateTargetVelocityDirection (float movementDirection)
		{
			physicalProperties.targetVelocityDirection = Vector3.right * movementDirection;
		}
		
		protected override void SetTargetVelocity (float speed)
		{
			physicalProperties.targetVelocity.x = physicalProperties.targetVelocityDirection.x * speed;
		}

		protected override bool CanIncreaseVelocity(float maxSpeed) => 
			Mathf.Abs (physicalProperties.targetVelocity.x) < maxSpeed;

		protected override bool CanDecreaseVelocity(float minSpeed) => 
			Mathf.Abs (physicalProperties.targetVelocity.x) > minSpeed + COMPARE_VELOCITY_TOLERANCE;

		protected override void AddSpeedToTargetVelocity (float speed)
		{
			physicalProperties.targetVelocity.x += physicalProperties.targetVelocityDirection.x * speed * Time.deltaTime;
		}
		
		protected override void SubtractSpeedFromTargetVelocity (float speed)
		{
			physicalProperties.targetVelocity.x -= physicalProperties.targetVelocityDirection.x * speed * Time.deltaTime;
		}
		
		protected override bool IsCurrentSpeedCloseToValue(float speed) => 
			Mathf.Abs(Mathf.Round (physicalProperties.targetVelocity.x)) == speed;

		protected virtual void HandleMovementInput()
		{
			IsDecreasingVelocity = false;
			_isInertiaEnabled = false;
			
			RotatePerson();
			
			ChangeVelocityWithInertia();
		}

		protected virtual void RotatePerson()
		{
			thirdPersonRotation.TryAlignHorizontalLookRotationWith (inputSystem.InputHorizontalMovementDirection);
		}

		protected virtual async void CancelMovementInput()
		{
			IsIncreasingVelocity = false;
			_isInertiaEnabled = false;
			
			CalculateTargetVelocityDirection (Mathf.Sign (physicalProperties.targetVelocity.x));
			
			if (!IsDecreasingVelocity)
				await DecreaseTargetVelocity (physicalProperties.decelerationInAir);
		}
		
		protected virtual void SubscribeOnInput()
		{
			inputSystem.movementInputHandleEventHandler += HandleMovementInput;
			inputSystem.movementInputCancelEventHandler += CancelMovementInput;
		}
		
		protected virtual void UnsubscribeFromInput()
		{
			inputSystem.movementInputHandleEventHandler -= HandleMovementInput;
			inputSystem.movementInputCancelEventHandler -= CancelMovementInput;
		}
	}
}
