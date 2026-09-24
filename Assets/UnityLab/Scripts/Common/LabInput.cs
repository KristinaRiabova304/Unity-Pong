using UnityEngine;

namespace UnityLab.Components
{
    /// <summary>
    /// Reads the lab's simple keyboard layout. Movement components use this so WASD and Arrows stay consistent.
    /// </summary>
    public static class LabInput
    {
        public static float GetHorizontal(InputScheme scheme)
        {
            if (scheme == InputScheme.Arrows)
            {
                return GetAxis(KeyCode.LeftArrow, KeyCode.RightArrow);
            }

            return GetAxis(KeyCode.A, KeyCode.D);
        }

        public static float GetVertical(InputScheme scheme)
        {
            if (scheme == InputScheme.Arrows)
            {
                return GetAxis(KeyCode.DownArrow, KeyCode.UpArrow);
            }

            return GetAxis(KeyCode.S, KeyCode.W);
        }

        public static Vector2 GetAxes(InputScheme scheme)
        {
            return new Vector2(GetHorizontal(scheme), GetVertical(scheme));
        }

        private static float GetAxis(KeyCode negative, KeyCode positive)
        {
            float value = 0f;
            if (Input.GetKey(negative))
            {
                value -= 1f;
            }

            if (Input.GetKey(positive))
            {
                value += 1f;
            }

            return value;
        }
    }
}
