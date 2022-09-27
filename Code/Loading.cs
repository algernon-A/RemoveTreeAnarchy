// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RemoveTreeAnarchy
{
    using AlgernonCommons;
    using ColossalFramework;
    using ICities;

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
            Logging.KeyMessage("version ", AssemblyUtils.TrimmedCurrentVersion, " loading");

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

            // Wait for all loading to complete.
            while (!Singleton<LoadingManager>.instance.m_loadingComplete)
            {
            }

            // Rellocate trees via simulation thread.
            Singleton<SimulationManager>.instance.AddAction(() => TreeHandler.ReallocateTrees());

            // Display status panel.
            StatusPanel.Create();

            Logging.Message("loading complete");
        }
    }
}