// <copyright file="TreeHandler.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RemoveTreeAnarchy
{
    using AlgernonCommons;
    using ColossalFramework;
    using ColossalFramework.Math;

    /// <summary>
    /// Tree management.
    /// </summary>
    internal static class TreeHandler
    {
        /// <summary>
        /// Reallocates trees in the tree array to the vanilla range, where possible, and regenerates forest resources.
        /// </summary>
        internal static void ReallocateTrees()
        {
            // Local references.
            SimulationManager simulationManager = Singleton<SimulationManager>.instance;
            TreeManager treeManager = Singleton<TreeManager>.instance;
            Array32<TreeInstance> trees = treeManager.m_trees;
            TreeInstance[] treeBuffer = trees.m_buffer;
            Randomizer randomizer = default;

            // Wait for any existing simulation pauses.
            while (simulationManager.ForcedSimulationPaused)
            {
            }

            // Pause simulation while reallocating.
            simulationManager.ForcedSimulationPaused = true;

            // Counting trees.
            int treeCount = 0, okayCount = 0, successCount = 0;

            // ItemCount is one over.
            Logging.KeyMessage("commencing tree check and fix; tree buffer length is ", treeBuffer.Length, " and nominal tree count is ", trees.ItemCount() - 1);

            // Iterate through the tree buffer.
            for (uint i = 0; i < treeBuffer.Length; ++i)
            {
                // Look for active trees.
                if ((treeBuffer[i].m_flags & (ushort)TreeInstance.Flags.Created) != 0)
                {
                    ++treeCount;

                    // Limit check.
                    if (successCount >= TreeManager.MAX_TREE_COUNT)
                    {
                        Logging.Message("Vanilla tree count limit reached; aborting any further reallocation");
                        break;
                    }

                    // If the tree is outside the vanilla buffer range, try to reallocate it.
                    if (i >= TreeManager.MAX_TREE_COUNT)
                    {
                        bool stillTrying = true;
                        int failureCount = 0;

                        // Keep iterating while we're still successfully creating new trees.
                        while (stillTrying)
                        {
                            // Attempt to create new tree.
                            stillTrying = treeManager.CreateTree(out uint treeID, ref randomizer, treeBuffer[i].Info, treeBuffer[i].Position, true);

                            // Was creation successful?
                            if (stillTrying)
                            {
                                // Yes - check ID of new tree.
                                if (treeID >= TreeManager.MAX_TREE_COUNT)
                                {
                                    // ID is out of range; release it to try again.
                                    treeManager.ReleaseTree(treeID);

                                    // Failsafe to prevent infinite loops - give up after 1,000 attempts.
                                    if (++failureCount > 1000)
                                    {
                                        stillTrying = false;
                                        Logging.KeyMessage("failed to reallocate tree ", treeID);
                                    }
                                }
                                else
                                {
                                    // Sucesfully created a tree within the vanilla buffer size - release the old tree and exit the loop.
                                    treeManager.ReleaseTree(i);
                                    ++successCount;
                                    stillTrying = false;
                                    Logging.Message("released tree ", i, " with new ID ", treeID);
                                }
                            }
                        }
                    }
                    else
                    {
                        // This tree is okay (within Vanilla buffer) - just flag it for update to ensure rebuild of forestry resource.
                        treeManager.UpdateTree(i);
                        ++okayCount;
                    }
                }
            }

            string message = string.Concat("Total trees processed: ", treeCount.ToString("N0"), "\nNot requiring reallocation: ", okayCount.ToString("N0"), "\nReallocated: ", successCount.ToString("N0"), "\nUnable to be reallocated: ", (treeCount - okayCount - successCount).ToString("N0"));
            Logging.KeyMessage(message);

            // Let the panel know we're done.
            if (StatusPanel.Panel is StatusPanel panel)
            {
                panel.ProgressText = message;
                panel.ProcessingDone = true;
            }

            // Unpause simulation now we're done.
            simulationManager.ForcedSimulationPaused = false;
        }
    }
}