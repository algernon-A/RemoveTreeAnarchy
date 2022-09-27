// <copyright file="StatusPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace RemoveTreeAnarchy
{
    using System;
    using AlgernonCommons;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Static class to manage the RON info panel.
    /// </summary>
    internal class StatusPanel : UIPanel
    {
        // Layout constants - general.
        private const float Margin = 5f;

        // Layout constants - Y.
        private const float TitleHeight = 45f;
        private const float LabelHeight = 90f;
        private const float PanelHeight = TitleHeight + LabelHeight + Margin;

        // Layout constants - X.
        private const float LabelWidth = 450f;
        private const float PanelWidth = Margin + Margin + LabelWidth;

        // Instance references.
        private static GameObject s_uiGameObject;
        private static StatusPanel s_panel;

        // Panel components.
        private readonly UILabel _progressLabel;

        // Status.
        private bool _processingDone = false;
        private float _timer;
        private int _timerStep;
        private bool _done = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusPanel"/> class.
        /// Constructor.
        /// </summary>
        internal StatusPanel()
        {
            // Basic behaviour.
            autoLayout = false;
            canFocus = true;
            isInteractive = true;

            // Appearance.
            backgroundSprite = "MenuPanel2";
            opacity = 1f;

            // Size.
            width = PanelWidth;
            height = PanelHeight;

            // Default position - centre in screen.
            relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - PanelWidth) / 2), Mathf.Floor((GetUIView().fixedHeight - PanelHeight) / 2));

            // Drag bar.
            UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.width = PanelWidth - 50f;
            dragHandle.height = TitleHeight;
            dragHandle.relativePosition = Vector2.zero;
            dragHandle.target = this;

            // Title label.
            UILabel titleLabel = AddUIComponent<UILabel>();
            titleLabel.relativePosition = new Vector2(50f, 13f);
            titleLabel.text = Mod.Instance.Name;

            // Close button.
            UIButton closeButton = AddUIComponent<UIButton>();
            closeButton.relativePosition = new Vector2(width - 35, 2);
            closeButton.normalBgSprite = "buttonclose";
            closeButton.hoveredBgSprite = "buttonclosehover";
            closeButton.pressedBgSprite = "buttonclosepressed";
            closeButton.eventClick += (component, clickEvent) => Close();

            // Decorative icon (top-left).
            UISprite iconSprite = AddUIComponent<UISprite>();
            iconSprite.relativePosition = new Vector2(5, 5);
            iconSprite.height = 32f;
            iconSprite.width = 32f;
            iconSprite.spriteName = "normal";

            _progressLabel = (UILabel)this.AddUIComponent<UILabel>();
            _progressLabel.autoSize = false;
            _progressLabel.autoHeight = false;
            _progressLabel.wordWrap = true;
            _progressLabel.height = LabelHeight;
            _progressLabel.width = LabelWidth;
            _progressLabel.text = "Processing";
            _progressLabel.relativePosition = new Vector2(Margin, TitleHeight);
        }

        /// <summary>
        /// Gets the active instance.
        /// </summary>
        internal static StatusPanel Panel => s_panel;

        /// <summary>
        /// Sets the progress label text.
        /// </summary>
        internal string ProgressText { set => _progressLabel.text = value; }

        /// <summary>
        /// Sets a value indicating whether processing is complete.
        /// </summary>
        internal bool ProcessingDone { set => _processingDone = value; }

        /// <summary>
        /// Called by Unity every tick.  Used here to track state of any in-progress replacments.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Don't do anything further if we're done.
            if (_done)
            {
                return;
            }

            // Is processing completed?
            if (_processingDone)
            {
                // Done! Update text label to show result.
                _done = true;
            }
            else
            {
                // No - still in progress - update timer.
                _timer += Time.deltaTime;

                // Add a period to the progress label every 100ms.  After 30, clear and start again.
                if (_timer > .1f)
                {
                    if (++_timerStep > 30)
                    {
                        _progressLabel.text = "Processing";
                        _timerStep = 0;
                    }
                    else
                    {
                        _progressLabel.text += ".";
                    }

                    // Either way, reset timer to zero.
                    _timer = 0f;
                }
            }
        }

        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Create()
        {
            try
            {
                // If no GameObject instance already set, create one.
                if (s_uiGameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_uiGameObject = new GameObject("RTAPanel");
                    s_uiGameObject.transform.parent = UIView.GetAView().transform;

                    // Create new panel instance and add it to GameObject.
                    s_panel = s_uiGameObject.AddComponent<StatusPanel>();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating InfoPanel");
            }
        }

        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        internal static void Close()
        {
            // Don't do anything if no panel, or if we're in the middle of replacing.
            if (s_panel == null || !s_panel._done)
            {
                return;
            }

            // Destroy game objects.
            GameObject.Destroy(s_panel);
            GameObject.Destroy(s_uiGameObject);

            // Let the garbage collector do its work (and also let us know that we've closed the object).
            s_panel = null;
            s_uiGameObject = null;
        }
    }
}