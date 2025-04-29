namespace LibUR.GameStates
{
    public enum GameState
    {
        Preloading,    // Initial resource loading (splash, async bundles, etc.)
        Startup,       // Init systems, show logo, etc.
        MainMenu,      // Player has control in the menu
        Settings,      // Optional, separate config screen
        Loading,       // Scene/level transition loading
        Playing,       // Main gameplay loop
        Cutscene,      // Scripted non-interactive sequences
        Dialogue,      // Optional, text/narrative scenes
        Pause,         // Game paused
        GameOver,      // Result screen or retry prompt
        Cleanup,       // Internal shutdown/reset
        Error,         // Unexpected state or fail-safe mode
        Debug,         // Only for dev purposes
        CustomState1,  // Extra states
        CustomState2,  // Extra states
        CustomState3   // Extra states
    }
}
