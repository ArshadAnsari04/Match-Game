using UnityEngine;
public class GameStateManager : Singleton<GameStateManager> 
{
    private IGameState currentState;

    public void SetState(IGameState state)
    {
        currentState?.ExitState();
        currentState = state;
        currentState.EnterState();
    }

    private void Update()
    {
        currentState?.UpdateState();
    }
}