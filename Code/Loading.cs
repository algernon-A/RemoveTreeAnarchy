using ICities;
using ColossalFramework;


namespace RemoveTreeAnarchy
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Internal flags.
        internal static bool isModEnabled = false;

        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            Logging.KeyMessage("version ", RTAMod.Version, " loading");

            // Don't do anything if not in game (e.g. if we're going into an editor).
            if (loading.currentMode != AppMode.Game && loading.currentMode != AppMode.MapEditor)
            {
                isModEnabled = false;
                Logging.KeyMessage("not loading into game or map editor, skipping activation");
                return;
            }

            // All good to go at this point.
            isModEnabled = true;
            base.OnCreated(loading);
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Don't do anything further if we're not operating.
            if (!isModEnabled)
            {
                Logging.Message("exiting");
                return;
            }

            // Display status panel.
            StatusPanel.Create();

            // Rellocate trees via simulation thread.
            Singleton<SimulationManager>.instance.AddAction(() => TreeHandler.ReallocateTrees());

            Logging.Message("loading complete");
        }
    }
}