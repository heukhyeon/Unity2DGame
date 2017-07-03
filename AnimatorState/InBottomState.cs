using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InBottomState : StateMachineBehaviour
{
    public StateStruct[] strcuts;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(!animator.GetBool("Bottom"))
        {
            if (animator.GetBool("Jumping")) animator.Play("JumpStart");
            else animator.Play("Jumping");
        }
        foreach(StateStruct state in strcuts)
        {
            switch (state.parametertype)
            {
                case StateStruct.ParameterType.Int:
                    break;
                case StateStruct.ParameterType.Float:
                    break;
                case StateStruct.ParameterType.Bool:
                    break;
            }
        }
    }
}
