using GameCore;
using UnityEngine;

namespace GameUI
{
    public class JumpBtn : StateMachineBehaviour
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 当状态机进入 MoveLeft 状态时，执行左移操作
            Player.Instance.SimulateJump();
        }
    }
}
