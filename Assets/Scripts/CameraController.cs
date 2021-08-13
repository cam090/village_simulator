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
            
            // please note: most coordinates are flipped by 180 degrees as I have flipped the terrain
            // move camera up when 'w' is pressed
            // or when mouse is at the top of the screen (determined using Screen.height)
            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                // no matter what frame rate we're running at,
                // Time.deltaTime helps to keep movement consistent
                cameraPosition.z -= panSpeed * Time.deltaTime;
            }
            
            // move camera down when 's' is pressed
            // or when mouse is at the bottom of the screen
            else if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
            {
                cameraPosition.z += panSpeed * Time.deltaTime;
            }
            
            // move camera right when 'd' is pressed
            // or when mouse is at the right-most of the screen
            else if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                cameraPosition.x -= panSpeed * Time.deltaTime;
            }
            
            // move camera left when 'a' is pressed
            // or when mouse is at the left-most of the screen
            else if (Input.GetKey("a") || Input.mousePosition.x <= - panBorderThickness)
            {
                cameraPosition.x += panSpeed * Time.deltaTime;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            cameraPosition.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
            
            // Mathf.Clamp helps to make sure camera's position doesn't go out of bounds
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, 0, panLimitX);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, minY, maxY);
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, 0, panLimitZ);

            transform.position = cameraPosition;
        }
    }
}