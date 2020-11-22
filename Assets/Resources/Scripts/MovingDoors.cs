using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovingDoors : MonoBehaviour
{
    public AudioSource audioSrc;
    public Transform gateA;
    public Transform gateB;
    
    private Transform player;

    private static bool gateOpened = false;
    private bool gateToggling = false;

    private static float startPosA;
    private static float startPosB;

    public float move;
    public float delay = 0.025f;
    public float step = 0.05f;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();

        startPosA = gateA.position.x;
        startPosB = gateB.position.x;
    }

    private void Update()
    {
        if(Check())
        {
            ToggleGate();
        }
    }

    private bool Check()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(player.position, transform.position) <= 5.0f)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    private void ToggleGate()
    {
        if (!gateToggling)
        {
            audioSrc.Play();

            if (!gateOpened)
            {
                StartCoroutine(MoveGatePos(gateA, startPosA, startPosA + move));
                StartCoroutine(MoveGateNeg(gateB, startPosB, startPosB - move));
            }
            else
            {
                StartCoroutine(MoveGateNeg(gateA, startPosA + move, startPosA));
                StartCoroutine(MoveGatePos(gateB, startPosB - move, startPosB));
            }

            gateOpened = !gateOpened;
        }
    }

    private IEnumerator MoveGatePos(Transform gate, float startPos, float endPos)
    {
        gateToggling = true;

        float i = startPos;
        while(i < endPos)
        {
            i += step;
            gate.position = new Vector3(i, gate.position.y, gate.position.z);

            yield return new WaitForSeconds(delay);
        }

        if (i >= startPos)
        {
            gateToggling = false;
        }

    }
    
    private IEnumerator MoveGateNeg(Transform gate, float startPos, float endPos)
    {
        float i = startPos;
        while(i > endPos)
        {
            i -= step;
            gate.position = new Vector3(i, gate.position.y, gate.position.z);

            yield return new WaitForSeconds(delay);
        }


        if (i <= startPos)
        {
            gateToggling = false;
        }

    }
}
