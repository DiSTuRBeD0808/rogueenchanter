using Godot;
using RogueEnchanter.Models.Enums;

namespace RogueEnchanter.Systems
{
    /// <summary>
    /// Simple state management system for handling game state transitions
    /// Focuses on Rest ↔ Combat transitions with minimal complexity
    /// </summary>
    public partial class StateManagementSystem : Node
    {
        // Events for state changes
        [Signal] public delegate void StateChangedEventHandler(GameState newState, GameState previousState);
        [Signal] public delegate void StateTransitionRequestedEventHandler(StateTransition transition);

        // Current state tracking
        public GameState CurrentState { get; private set; } = GameState.Rest;
        public GameState PreviousState { get; private set; } = GameState.Menu;

        public override void _Ready()
        {
            DebugManager.Log(DebugCategory.GameManager, "StateManagementSystem initialized", DebugLevel.Info);
        }

        /// <summary>
        /// Request a state transition
        /// </summary>
        public void RequestStateTransition(StateTransition transition)
        {
            DebugManager.Log(DebugCategory.GameManager, $"State transition requested: {transition}", DebugLevel.Info);
            
            GameState newState = GetTargetState(transition);
            
            if (CanTransitionTo(newState))
            {
                TransitionToState(newState);
            }
            else
            {
                DebugManager.LogWarning(DebugCategory.GameManager, $"Invalid state transition: {CurrentState} → {newState}");
            }
        }

        /// <summary>
        /// Directly transition to a new state (with validation)
        /// </summary>
        public void TransitionToState(GameState newState)
        {
            if (newState == CurrentState)
            {
                DebugManager.Log(DebugCategory.GameManager, $"Already in state {newState}, no transition needed", DebugLevel.Verbose);
                return;
            }

            if (!CanTransitionTo(newState))
            {
                DebugManager.LogWarning(DebugCategory.GameManager, $"Cannot transition from {CurrentState} to {newState}");
                return;
            }

            var previousState = CurrentState;
            
            // Exit current state
            OnExitState(CurrentState);
            
            // Update state
            PreviousState = CurrentState;
            CurrentState = newState;
            
            // Enter new state
            OnEnterState(newState);
            
            // Emit signal
            EmitSignal(SignalName.StateChanged, (int)newState, (int)previousState);
            
            DebugManager.Log(DebugCategory.GameManager, $"State transition complete: {previousState} → {newState}", DebugLevel.Info);
        }

        /// <summary>
        /// Check if we can transition to the specified state
        /// </summary>
        private bool CanTransitionTo(GameState newState)
        {
            // Simple rules for now:
            switch (CurrentState)
            {
                case GameState.Menu:
                    return newState == GameState.Rest; // Menu can only go to Rest (main game)
                    
                case GameState.Rest:
                    return newState == GameState.Combat || newState == GameState.Menu;
                    
                case GameState.Combat:
                    return newState == GameState.Rest || newState == GameState.Menu;
                    
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the target state for a transition type
        /// </summary>
        private GameState GetTargetState(StateTransition transition)
        {
            return transition switch
            {
                StateTransition.StartCombat => GameState.Combat,
                StateTransition.EndCombat => GameState.Rest,
                StateTransition.EnterMenu => GameState.Menu,
                StateTransition.ExitMenu => GameState.Rest,
                _ => CurrentState
            };
        }

        /// <summary>
        /// Handle entering a new state
        /// </summary>
        private void OnEnterState(GameState state)
        {
            DebugManager.Log(DebugCategory.GameManager, $"Entering state: {state}", DebugLevel.Verbose);
            
            switch (state)
            {
                case GameState.Rest:
                    OnEnterRestState();
                    break;
                    
                case GameState.Combat:
                    OnEnterCombatState();
                    break;
                    
                case GameState.Menu:
                    OnEnterMenuState();
                    break;
            }
        }

        /// <summary>
        /// Handle exiting a state
        /// </summary>
        private void OnExitState(GameState state)
        {
            DebugManager.Log(DebugCategory.GameManager, $"Exiting state: {state}", DebugLevel.Verbose);
            
            switch (state)
            {
                case GameState.Rest:
                    OnExitRestState();
                    break;
                    
                case GameState.Combat:
                    OnExitCombatState();
                    break;
                    
                case GameState.Menu:
                    OnExitMenuState();
                    break;
            }
        }

        // State-specific enter/exit methods
        private void OnEnterRestState()
        {
            DebugManager.Log(DebugCategory.GameManager, "Player is now resting/exploring", DebugLevel.Info);
            // TODO: Enable rest-specific UI elements
            // TODO: Hide combat-specific UI elements
        }

        private void OnExitRestState()
        {
            DebugManager.Log(DebugCategory.GameManager, "Player is leaving rest state", DebugLevel.Verbose);
            // TODO: Cleanup rest-specific elements
        }

        private void OnEnterCombatState()
        {
            DebugManager.Log(DebugCategory.GameManager, "Player has entered combat!", DebugLevel.Info);
            // TODO: Enable combat-specific UI elements
            // TODO: Spawn enemy
            // TODO: Initialize combat systems
        }

        private void OnExitCombatState()
        {
            DebugManager.Log(DebugCategory.GameManager, "Player has left combat", DebugLevel.Info);
            // TODO: Cleanup combat
            // TODO: Hide enemy
            // TODO: Process combat rewards
        }

        private void OnEnterMenuState()
        {
            DebugManager.Log(DebugCategory.GameManager, "Player entered menu", DebugLevel.Info);
            // TODO: Show menu UI
            // TODO: Pause game logic
        }

        private void OnExitMenuState()
        {
            DebugManager.Log(DebugCategory.GameManager, "Player exited menu", DebugLevel.Verbose);
            // TODO: Hide menu UI
            // TODO: Resume game logic
        }

        /// <summary>
        /// Check if currently in a specific state
        /// </summary>
        public bool IsInState(GameState state)
        {
            return CurrentState == state;
        }

        /// <summary>
        /// Check if we can perform an action based on current state
        /// </summary>
        public bool CanPerformAction(string action)
        {
            return action switch
            {
                "attack" => IsInState(GameState.Rest), // Can only start combat from Rest
                "rest" => IsInState(GameState.Combat), // Can only rest after combat
                "inventory" => IsInState(GameState.Rest) || IsInState(GameState.Combat), // Can access inventory anytime in game
                _ => false
            };
        }
    }
}
