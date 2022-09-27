// <copyright file="Mod.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RemoveTreeAnarchy
{
    using AlgernonCommons;
    using ICities;

    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public sealed class Mod : ModBase, IUserMod
    {
        /// <summary>
        /// Gets the mod's base display name (name only).
        /// </summary>
        public override string BaseName => "Remove Tree Anarchy";

        /// <summary>
        /// Gets the mod's description for display in the content manager.
        /// </summary>
        public string Description => "Repacks trees created with Tree Anarchy into the vanilla array";

        /// <summary>
        /// Saves settings file.
        /// </summary>
        public override void SaveSettings()
        {
        }

        /// <summary>
        /// Loads settings file.
        /// </summary>
        public override void LoadSettings()
        {
        }
    }
}
