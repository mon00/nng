using UnityEngine;
using System.Collections;

[SerializeField]
public class MenuChenger : MonoBehaviour {

    public Camera MainCamera;
    public GameObject CurrentWindow;
    public GameObject[] Windows;
    [Range(0f, 5f)]
    public float CameraPositionSpeed;
    [Range(0f, 1f)]
    public float CameraScaleSpeed;

    private bool Move = false;
    private bool ChengePosition = false;
    private bool ChengeRotation = false; 
    private GameObject CurrentAnchor;

    public void ChengePlace(GameObject NewAnchor)
    {
        if (MainCamera.transform.position == NewAnchor.transform.position) return;
        CurrentAnchor = NewAnchor;
        ChengePosition = ChengeRotation = Move = true;
    }

    public void ChengeWindow(GameObject NewWindow)
    {
        if (CurrentWindow == NewWindow) return;
        if (CurrentWindow != null)  CurrentWindow.SetActive(false);
        CurrentWindow = NewWindow;
    }

    void Update()
    {
        if (Move)
        {
            if (ChengePosition)
            {
                float RangePosition;
                RangePosition = Vector3.Magnitude(MainCamera.transform.position - CurrentAnchor.transform.position);
                if (RangePosition > 5f) MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, CurrentAnchor.transform.position, Time.deltaTime * CameraPositionSpeed);
                else ChengePosition = false;
            }

            if (ChengeRotation)
            {
                float RangeRotation;
                RangeRotation = Quaternion.Angle(MainCamera.transform.rotation, CurrentAnchor.transform.rotation);
                if (RangeRotation > 5f) MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, CurrentAnchor.transform.rotation, Time.deltaTime * CameraScaleSpeed);
                else ChengeRotation = false;
            }

            if (!ChengePosition && !ChengeRotation)
            {
                if (CurrentWindow != null) CurrentWindow.SetActive(true);
                Move = false;
            }
        }
    }

    void Start()
    {
        foreach (GameObject window in Windows)
        {
            if (window == CurrentWindow) window.SetActive(true);
            else window.SetActive(false);
        }
    }
}
