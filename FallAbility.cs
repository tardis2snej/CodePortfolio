using System.Threading.Tasks;
using Bimicore.BSG.StateMachinePattern;
using UnityEngine;

namespace Bimicore.BSG.ThirdPerson
{
	public class FallAbility : MovementInAirAbstractAbility, IState
	{
		public int StateRestrictorsCount { get; set; } = 0;

		private bool _isDecreasingToMaxAllowedSpeed = false;

		public async void OnEnter()
		{
			CommonEnter();
			
			view.StartAnimation (animatorManager);

			await TryDecreaseToMaxAllowedSpeed();
			
			if (inputSystem.InputHorizontalMovementDirection != 0)
			{
				ChangeVelocityWithInertia();
			}
			else
			{
				CalculateTargetVelocityDirection (Mathf.Sign (physicalProperties.targetVelocity.x));
				await DecreaseTargetVelocity (physicalProperties.decelerationInAir);
			}
		}

		public void Tick()
		{
			CommonTick();
		}

		public void FixedTick()
		{
			AddGravity (physicalProperties.fallAcceleration); 
			
			CommonFixedTick();
		}

		public void OnExit()
		{
			CommonExit();
		}

		private async Task TryDecreaseToMaxAllowedSpeed()
		{
			if (IsSpeedBiggerThanMax())
			{
				_isDecreasingToMaxAllowedSpeed = true;
				await DecreaseTargetVelocity (physicalProperties.decelerationInAir, maxHorizontalSpeed);
				_isDecreasingToMaxAllowedSpeed = false;
			}
		}

		private bool IsSpeedBiggerThanMax() => physicalProperties.targetVelocity.x > maxHorizontalSpeed;

		protected override void HandleMovementInput()
		{
			if (_isDecreasingToMaxAllowedSpeed)
			{
				RotatePerson();
				return;
			}

			base.HandleMovementInput();
		}

		protected override void CancelMovementInput()
		{
			if (_isDecreasingToMaxAllowedSpeed)
				return;
			
			base.CancelMovementInput();
		}
	}
}