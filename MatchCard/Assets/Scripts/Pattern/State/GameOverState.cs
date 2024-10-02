// Game Over state
using UnityEngine;

public class GameOverState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Game Over");
      
    }

    public void UpdateState()
    {
    }

    public void ExitState()
    {
    }
}