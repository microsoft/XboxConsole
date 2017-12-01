//------------------------------------------------------------------------------
// <copyright file="XboxGamepadState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Internal.GamesTest.Xbox.Input
{
    using System;

    /// <summary>
    /// Represents the state of an XboxGamepad.
    /// </summary>
    public class XboxGamepadState
    {
        private float leftThumbstickX;
        private float leftThumbstickY;
        private float rightThumbstickX;
        private float rightThumbstickY;
        private float leftTrigger;
        private float rightTrigger;

        /// <summary>
        /// Gets or sets the buttons of the GamepadState.
        /// </summary>
        public XboxGamepadButtons Buttons { get; set; }

        /// <summary>
        /// Gets or sets the left thumbsticks X value.
        /// </summary>
        /// <remarks>Valid range is [-1.0, 1.0].</remarks>
        public float LeftThumbstickX 
        {
            get
            {
                return this.leftThumbstickX;
            }

            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Valid range for LeftThumbstickX value is [-1.0, 1.0].");
                }

                this.leftThumbstickX = value;
            }
        }

        /// <summary>
        /// Gets or sets the left thumbsticks Y value.
        /// </summary>
        /// <remarks>Valid range is [-1.0, 1.0].</remarks>
        public float LeftThumbstickY
        {
            get
            {
                return this.leftThumbstickY;
            }

            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Valid range for LeftThumbstickY value is [-1.0, 1.0].");
                }

                this.leftThumbstickY = value;
            }
        }

        /// <summary>
        /// Gets or sets the right thumbsticks X value.
        /// </summary>
        /// <remarks>Valid range is [-1.0, 1.0].</remarks>
        public float RightThumbstickX
        {
            get
            {
                return this.rightThumbstickX;
            }

            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Valid range for RightThumbstickX value is [-1.0, 1.0].");
                }

                this.rightThumbstickX = value;
            }
        }

        /// <summary>
        /// Gets or sets the right thumbsticks Y value.
        /// </summary>
        /// <remarks>Valid range is [-1.0, 1.0].</remarks>
        public float RightThumbstickY
        {
            get
            {
                return this.rightThumbstickY;
            }

            set
            {
                if (value < -1.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Valid range for RightThumbstickY value is [-1.0, 1.0].");
                }

                this.rightThumbstickY = value;
            }
        }

        /// <summary>
        /// Gets or sets the left triggers value.
        /// </summary>
        /// <remarks>Valid range is [0.0, 1.0].</remarks>
        public float LeftTrigger
        {
            get
            {
                return this.leftTrigger;
            }

            set
            {
                if (value < 0.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Valid range for left trigger is [0.0, 1.0].");
                }

                this.leftTrigger = value;
            }
        }

        /// <summary>
        /// Gets or sets the right triggers value.
        /// </summary>
        public float RightTrigger
        {
            get
            {
                return this.rightTrigger;
            }

            set
            {
                if (value < 0.0f || value > 1.0f)
                {
                    throw new ArgumentOutOfRangeException("value", "Valid range for right trigger is [0.0, 1.0].");
                }

                this.rightTrigger = value;
            }
        }

        /// <summary>
        /// Determines whether the two GamepadStates are equal.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if states are the same, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            XboxGamepadState other = obj as XboxGamepadState;
            if (other != null)
            {
                return
                    this.Buttons == other.Buttons &&
                    FloatEquals(other.LeftThumbstickX, this.LeftThumbstickX) &&
                    FloatEquals(other.LeftThumbstickY, this.LeftThumbstickY) &&
                    FloatEquals(other.LeftTrigger, this.LeftTrigger) &&
                    FloatEquals(other.RightThumbstickX, this.RightThumbstickX) &&
                    FloatEquals(other.RightThumbstickY, this.RightThumbstickY) &&
                    FloatEquals(other.RightTrigger, this.RightTrigger);
            }

            return false;
        }

        /// <summary>
        /// Gets a hash code for the GamepadState class.
        /// </summary>
        /// <returns>A hash value.</returns>
        public override int GetHashCode()
        {
            return (int)this.Buttons ^ 
                this.LeftThumbstickX.GetHashCode() ^ 
                this.LeftThumbstickY.GetHashCode() ^ 
                this.LeftTrigger.GetHashCode() ^
                this.RightThumbstickX.GetHashCode() ^
                this.RightThumbstickY.GetHashCode() ^
                this.RightTrigger.GetHashCode();
        }

        private static bool FloatEquals(float a, float b)
        {
            return Math.Abs(a - b) < 0.01f;
        }
    }
}
