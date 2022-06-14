using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;


namespace RemoveTreeAnarchy
{


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
		private static GameObject uiGameObject;
		private static StatusPanel panel;
		internal static StatusPanel Panel => panel;

		// Panel components.
		private readonly UILabel progressLabel;

		// Status.
		internal bool processingDone = false;
		internal string processingText;
		private float timer;
		private int timerStep;
		private bool done = false;


		/// <summary>
		/// Called by Unity every tick.  Used here to track state of any in-progress replacments.
		/// </summary>
		public override void Update()
		{
			base.Update();

			// Don't do anything further if we're done.
			if (done)
			{
				return;
			}

			// Is processing completed?
			if (processingDone)
			{
				// Done! Update text label to show result.
				progressLabel.text = processingText;
				done = true;
			}
			else
			{
				// No - still in progress - update timer.
				timer += Time.deltaTime;

				// Add a period to the progress label every 100ms.  After 30, clear and start again.
				if (timer > .1f)
				{
					if (++timerStep > 30)
					{
						progressLabel.text = "Processing";
						timerStep = 0;
					}
					else
					{
						progressLabel.text += ".";
					}

					// Either way, reset timer to zero.
					timer = 0f;
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
				if (uiGameObject == null)
				{
					// Give it a unique name for easy finding with ModTools.
					uiGameObject = new GameObject("RTAPanel");
					uiGameObject.transform.parent = UIView.GetAView().transform;

					// Create new panel instance and add it to GameObject.
					panel = uiGameObject.AddComponent<StatusPanel>();
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
			if (panel == null || !panel.done)
			{
				return;
			}

			// Destroy game objects.
			GameObject.Destroy(panel);
			GameObject.Destroy(uiGameObject);

			// Let the garbage collector do its work (and also let us know that we've closed the object).
			panel = null;
			uiGameObject = null;
		}


		/// <summary>
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
			titleLabel.text = RTAMod.ModName;

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
			//iconSprite.atlas = Textures.RonButtonSprites;
			iconSprite.spriteName = "normal";

			progressLabel = (UILabel)this.AddUIComponent<UILabel>();
			progressLabel.autoSize = false;
			progressLabel.autoHeight = false;
			progressLabel.wordWrap = true;
			progressLabel.height = LabelHeight;
			progressLabel.width = LabelWidth;
			progressLabel.text = "Processing";
			progressLabel.relativePosition = new Vector2(Margin, TitleHeight);
		}
	}
}