using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
		public TargetLock targetLockManager; 

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("UI Objects")]
		public UI.UIManager uiManager;
		public InfoPanel infoPanel;
		public UI.SettingsManager settingsManager;

		[Header("Animation")]
		public Animator animator;
		private int _animIDBlock;
		private int _animIDParry;
		private int _animIDDodge;
		private int _animIDDash; 
		private int _animIDAttack;
		private int _animIDMoving;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

        private void Start()
        {
			_animIDMoving = Animator.StringToHash("Moving");
			_animIDAttack = Animator.StringToHash("Attack");
			_animIDBlock = Animator.StringToHash("Block");
			_animIDDodge = Animator.StringToHash("Dodge");
			_animIDDash = Animator.StringToHash("Dash");
			_animIDParry = Animator.StringToHash("Parry");
		}
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
			// Only do things if the radial menu is on
			if (swordSelectActive)
            {
				// Get the mouse position from the input
				Vector2 delta = value.Get<Vector2>();

				// Only do this if the input is not zero
				// And if the magnitude of the vector is larger than a certain
				// value so that we can verify the delta is purposeful
				if (delta != Vector2.zero && delta.sqrMagnitude >= 50)
				{
					// Calculate the angle of input using trig and convert to deg
					float angle = Mathf.Atan2(delta.y, -delta.x) * Mathf.Rad2Deg;

					// Move 0 to the space between topaz and sapphire
					angle += 203.5f;

					// Modify the angle if it isn't in the range (0, 360)
					if (angle < 0) angle += 360;
					if (angle > 360) angle -= 360;

					//Debug.Log(angle);

					// Update currentSwordType according to angle
					if (angle > 0 && angle <= 113.5)
					{
						currentSwordType = Enums.SwordType.Sapphire;
					}
					else if (angle > 113.5 && angle <= 223.5)
					{
						currentSwordType = Enums.SwordType.Ruby;
					}
					else
					{
						currentSwordType = Enums.SwordType.Topaz;
					}
				}
            }
        }
		
		public void OnSwitchSwordSpecific(InputValue value)
        {
			currentSwordType = (Enums.SwordType)(value.Get<float>() - 1);
			player.SwitchSword(currentSwordType);
        }

		/// <summary>
		/// Input method that runs when the target lock control is hit
		/// </summary>
		/// <param name="value">The value of the control</param>
		public void OnTargetLock(InputValue value)
		{
			// Toggles the target lock state to its opposite value
			targetLockManager.Active = !targetLockManager.Active;
			Debug.Log($"Target Lock Enabled: {targetLockManager.Active}");
			// Runs the LockOnEnemy method no matter what because it serves both purposes
			targetLockManager.LockOnEnemy();
		}

		/// <summary>
		/// Input method that runs when the MoveTarget input is used
		/// </summary>
		/// <param name="value">The value of the float input</param>
		public void OnMoveTarget(InputValue value)
		{
			// Checks if target lock is active
			// If not, do nothing
			if (targetLockManager.Active)
			{
				// Converts the input value to a usable float
				float moveDirection = value.Get<float>();

				// If the value is positive
				// Move the target to the right
				if (moveDirection > 0)
				{
					targetLockManager.MoveTarget(1);
				}
				// If the value is negative
				// Move the target to the left7
				else if (moveDirection < 0)
				{
					targetLockManager.MoveTarget(-1);
				}
			}
		}

		public void OnPause(InputValue value)
		{
			if (value.isPressed)
				uiManager.Pause();
		}

		public void OnUnpause(InputValue value)
		{
			if (value.isPressed)
				uiManager.Unpause();
		}

		public void OnControlsChanged()
        {
			infoPanel.ChooseControlSchemeIcons();
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

			animator.SetBool(_animIDAttack, attack);
		}

		public void ParryInput(bool newParryState)
		{
			parry = newParryState;

			animator.SetBool(_animIDParry, parry);
		}

		public void BlockInput(bool newBlockState)
        {
			block = newBlockState;

			animator.SetBool(_animIDBlock, block);
		}

		public void DodgeInput(bool newDodgeRollState)
		{
			dodge = newDodgeRollState;

			if (currentSwordType == Enums.SwordType.Sapphire)
			{
				if (player.GetPlayerFSMState() != Enums.PlayerCondition.F_Dashing)
                {
					animator.SetTrigger(_animIDDash);
					Debug.Log("Dashing");
				}
			}
			else
			{
				if (move == Vector2.zero)
				{
					animator.SetBool(_animIDMoving, false);
				}
				else
				{
					animator.SetBool(_animIDMoving, true);
				}

				animator.SetBool(_animIDDodge, dodge);
			}
		}

		public void OpenSwordSelectInput(bool newSwordSelectState)
        {
			swordSelectActive = newSwordSelectState;

			uiManager.ToggleRadialMenu(swordSelectActive);

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
