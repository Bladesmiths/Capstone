using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

namespace Bladesmiths.Capstone
{
	public class PlayerInputsScript : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Player player; 
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool attack = false;
		public bool parry = false;
		public bool block = false;
		public bool dodge = false;
		public bool swordSelectActive = false;
		public Enums.SwordType currentSwordType;
		public GameObject playerLookCamera; 

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

        #region On Input Methods
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook && !swordSelectActive)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
		}

		public void OnParry(InputValue value)
		{
			ParryInput(value.isPressed);
		}

		public void OnBlock(InputValue value)
        {
			BlockInput(value.isPressed);
        }

		public void OnDodge(InputValue value)
		{
			DodgeInput(value.isPressed);
		}

		public void OnOpenSwordSelector(InputValue value)
		{
			OpenSwordSelectInput(value.isPressed);
		}

		public void OnSwitchSword(InputValue value)
        {
			if (swordSelectActive)
            {
				currentSwordType += (int)value.Get<float>();

				if (currentSwordType < Enums.SwordType.Quartz)
                {
					currentSwordType = Enums.SwordType.Sapphire;
                }
				else if (currentSwordType > Enums.SwordType.Sapphire)
                {
					currentSwordType = Enums.SwordType.Quartz;
                }
			}
        }
		
		public void OnSwitchSwordSpecific(InputValue value)
        {
			player.SwitchSword((Enums.SwordType)(value.Get<float>() - 1));
        }
        #endregion

        #region Input Helper Methods
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}

		public void ParryInput(bool newParryState)
		{
			parry = newParryState;
		}

		public void BlockInput(bool newBlockState)
        {
			block = newBlockState;
        }

		public void DodgeInput(bool newDodgeRollState)
		{
			dodge = newDodgeRollState;
		}

		public void OpenSwordSelectInput(bool newSwordSelectState)
        {
			swordSelectActive = newSwordSelectState;
			
			if (swordSelectActive)
            {
				currentSwordType = player.CurrentSword.SwordType;
            }
			else
            {
				player.SwitchSword(currentSwordType); 
            }

			playerLookCamera.GetComponent<CustomCinemachineInputProvider>().InputEnabled = !swordSelectActive;
        }
        #endregion


#if !UNITY_IOS || !UNITY_ANDROID

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif
	}

}
