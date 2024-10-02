// Playing state
using UnityEngine;

public class PlayingState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Game Started");
    }

    public void UpdateState()
    {
        // Handle game logic while playing
    }

    public void ExitState()
    {
        Debug.Log("Exiting Playing State");
    }
}