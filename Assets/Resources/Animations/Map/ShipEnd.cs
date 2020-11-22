using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Image shipSpin = animator.transform.Find("Spin").GetComponent<Image>();

        shipSpin.enabled = false;
    }
}
