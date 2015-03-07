using UnityEngine;
using System.Collections;

public class MouseMove : MonoBehaviour
{

    public Camera camera;
    [Range(0f, 1f)]
    public float cameraSpeed = 1F;

    void Start()
    {
        if (camera == null)
        {
            Debug.Log("No camera on script!");
        }
    }

    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * cameraSpeed;
    }
}