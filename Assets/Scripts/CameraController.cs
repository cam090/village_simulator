using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class CameraController : MonoBehaviour
    {
        public float panSpeed = 20f;
        public float panBorderThickness = 10f;
        public int panLimitX;
        public int panLimitZ;
        public float scrollSpeed = 20f;
        public float minY = 20f;
        public float maxY = 120f;
        private void Update()
        {
            Vector3 cameraPosition = transform.position;
            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                cameraPosition.z += panSpeed * Time.deltaTime;
            }
            
            else if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
            {
                cameraPosition.z -= panSpeed * Time.deltaTime;
            }
            
            else if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                cameraPosition.x += panSpeed * Time.deltaTime;
            }
            
            else if (Input.GetKey("a") || Input.mousePosition.x <= - panBorderThickness)
            {
                cameraPosition.x -= panSpeed * Time.deltaTime;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            cameraPosition.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
            
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, 0, panLimitX);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, minY, maxY);
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, 0, panLimitZ);

            transform.position = cameraPosition;
        }
    }
}