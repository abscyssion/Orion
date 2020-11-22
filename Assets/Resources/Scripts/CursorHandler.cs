using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CursorHandler : MonoBehaviour
{
    private RectTransform cursorRect;
    private RaycastHit raycastHit;
    public RectTransform screen;
    public RectTransform lineX; private Image lineXImg;
    public RectTransform lineY; private Image lineYImg;

    public bool active { get; private set; }

    public bool changed { get; private set; } = false; //It WILL move beacuse of the animator. If causing performance problems, disable.

    private void Awake()
    {
        cursorRect = gameObject.GetComponent<RectTransform>();

        lineXImg = lineX.GetComponent<Image>();
        lineYImg = lineY.GetComponent<Image>();
    }


    private Vector3 prevPos = new Vector3();
    void Update()
    {
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out raycastHit)) //If hits anything.
            {
                if (raycastHit.transform == screen) //If hits screen.
                {
                    active = true;
                    lineXImg.enabled = true;
                    lineYImg.enabled = true;

                    Vector3 position = raycastHit.point;
                    cursorRect.position = position;
                    lineX.localPosition = new Vector3(0, cursorRect.localPosition.y);
                    lineY.localPosition = new Vector3(cursorRect.localPosition.x, 0);

                    if (position == prevPos)
                    {
                        changed = false;
                    }
                    else
                    {
                        changed = true;
                    }
                    prevPos = position;
                }

                else
                {
                    active = false;
                    lineXImg.enabled = false;
                    lineYImg.enabled = false;
                }
            }
        }
    }
}
