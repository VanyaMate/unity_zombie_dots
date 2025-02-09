using UnityEngine;

namespace Mouse
{
    public class World
    {
        public static Vector3 GetPosition()
        {
            Ray cameraRaycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRaycast, out RaycastHit hit))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}