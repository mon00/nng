using UnityEngine;
using System.Collections;

[SerializeField]
public class PanelSwitcher : MonoBehaviour {

    [Header("Objects")]
    public GameObject[] GamePanels;
    public GameObject CurrentPanel;
    public GameObject CurrentAnchor;

    [Header("Speed")]
    [Range(0.01f, 1f)]
    public float TransformSpeed;
    [Range(0.01f, 1f)]
    public float RotationSpeed;

    private float speed;
    private bool Move = false;
    private bool ChengePosition = false;
    private bool ChengeRotation = false;

    void Start()
    {
        foreach (GameObject panel in GamePanels)
        {
            bool active = (panel == CurrentPanel) ? true : false;
            panel.SetActive(active);
        }
        transform.position = CurrentAnchor.transform.position;
        transform.rotation = CurrentAnchor.transform.rotation;
    }

    public void SwitchPanel(GameObject NewPanel)
    {
        if (NewPanel == CurrentPanel) return;
        CurrentPanel.SetActive(false);
        CurrentPanel = NewPanel;
        if (!Move) Move = true;
    }

    public void SwitchAnchor(GameObject NewAnchor)
    {
        if (CurrentAnchor == NewAnchor) return;
        CurrentAnchor = NewAnchor;
        ChengePosition = ChengeRotation = true;
        if (!Move) Move = true;
    }

    public void Update()
    {
        if (Move)
        {
            if (ChengePosition)
            {
                float RangePosition;
                RangePosition = Vector3.Magnitude(transform.position - CurrentAnchor.transform.position);
                if (RangePosition > 5f) transform.position = Vector3.Lerp(transform.position, CurrentAnchor.transform.position, Time.deltaTime * TransformSpeed);
                else ChengePosition = false;
            }

            if (ChengeRotation)
            {
                float RangeRotation;
                RangeRotation = Quaternion.Angle(transform.rotation, CurrentAnchor.transform.rotation);
                if (RangeRotation > 5f) transform.rotation = Quaternion.Lerp(transform.rotation, CurrentAnchor.transform.rotation, Time.deltaTime * RotationSpeed);
                else ChengeRotation = false;
            }

            if (!ChengePosition && !ChengeRotation)
            {
                if (CurrentPanel != null) CurrentPanel.SetActive(true);
                Move = false;
            }
        }
    }
}
