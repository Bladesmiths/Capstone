using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bladesmiths.Capstone
{
    public partial class Player
    {
        // All the player's state transition methods
        /// <summary>
        /// The condition for going between the IDLE and MOVE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsMoving() => () => inputs.move != Vector2.zero;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsIdle() => () => gameObject.GetComponent<CharacterController>().velocity.magnitude <= 0;

        /// <summary>
        /// The condition for going between the MOVE and IDLE states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsCombatIdle() => () => (attack.Timer >= 0.9 / 1.5f) && !inputs.parry; // Attack Timer conditional should be compared to length of animation

        /// <summary>
        /// The condition for going between the IDLE and BLOCK state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsBlockPressed() => () => inputs.block == true;

        /// <summary>
        /// The condition for going between the BLOCK and PARRY state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsBlockReleased() => () => inputs.block == false;

        /// <summary>
        /// The condition for going between the PARRY and IDLE state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsParryFinished() => () => parryEnd == true;

        /// <summary>
        /// The condition for going between the PARRY and IDLE state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsParrySuccessful() => () => parrySuccessful == true;

        /// <summary>
        /// The condition for going between MOVE/IDLE and the ATTACK states
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAttacking() => () => inputs.attack;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsDamaged() => () => damaged && Health > 0;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsAbleToDamage() => () => takeDamage.Timer >= takeDamage.AnimDuration;

        /// <summary>
        /// The condition for going from MOVE to DODGE state
        /// </summary>
        public Func<bool> IsDodging() => () => inputs.dodge && controller.isGrounded;

        /// <summary>
        /// The condition for going from DODGE to MOVE state
        /// </summary>
        /// <returns></returns>
        // TODO: Should implement something like when dodging animation stops
        public Func<bool> IsDodgingStopped() => () => dodge.timer >= 1.1f / 1.5f;

        /// <summary>
        /// The condition for having been attacked
        /// </summary>
        /// <returns></returns>
        public Func<bool> Dead() => () => damaged && Health <= 0;

        /// <summary>
        /// Checks if the player is grounded
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsGrounded() => () =>
        {
            return gameObject.GetComponent<CharacterController>().isGrounded && jump.LandTimeoutDelta <= 0.0f;
        };

        /// <summary>
        /// Checks to see if the jump button has been pressed
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsJumping() => () => inputs.jump;

        /// <summary>
        /// The condition for going to the NULL state
        /// </summary>
        /// <returns></returns>
        public Func<bool> IsNull() => () => inState == true;

        /// <summary>
        /// The condition for going to the NULL state
        /// </summary>
        /// <returns></returns>
        public Func<bool> NotNull() => () => inState == false;
    }
}

