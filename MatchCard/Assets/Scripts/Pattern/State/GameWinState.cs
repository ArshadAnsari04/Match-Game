// Game Win state
using UnityEngine;

public class GameWinState : IGameState
{
    public void EnterState()
    {
        Debug.Log("You Win!");
       
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
    }
}