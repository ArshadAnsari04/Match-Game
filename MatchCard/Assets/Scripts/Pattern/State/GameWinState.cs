// Game Win state
using UnityEngine;

public class GameWinState : IGameState
{
    public void EnterState()
    {
        Debug.Log("You Win!");
        UIManager.Instance.ShowWinScreen();
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
    }
}