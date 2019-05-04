﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Contains all of the custom events needed among scripts.
/// </summary>
public class EventManager : Singleton<EventManager> {
	#region DELEGATES

	public delegate void Cancel();
	public delegate void CanvasClick();
	public delegate void RefreshPanel();

	#endregion

	/// <summary>
	/// When cancel has been pressed.
	/// </summary>
	public static event Cancel onCancel;

	/// <summary>
	/// When empty space on canvas is pressed with either one of the mouse buttons.
	/// </summary>
	public static event CanvasClick onCanvasClick;

	/// <summary>
	/// Event triggered by other classes to let them know that changes has happened and they need to update their
	/// object contents.
	/// </summary>
	public static event RefreshPanel onRefreshPanel;

	private bool refreshPanelEventIsTriggered = false;

	void OnGUI() {
		if (Input.GetButtonDown("Cancel")) {
			// Notify all subscribed classes when user wishes to cancel either by pressing escape, or
			// clicking on a non-occupied area on canvas
			onCancel?.Invoke();
		}

		if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
			if (EventSystem.current.IsPointerOverGameObject() == false) {
				// Notify if canvas is pressed (where there are no objects) using mouse buttons
				onCanvasClick?.Invoke();
			}
		}
	}

	void Update() {
		// Ask classes to update panels 
		if (Instance.refreshPanelEventIsTriggered) {
			onRefreshPanel?.Invoke();
			Instance.refreshPanelEventIsTriggered = false;
		}
	}

	/// <summary>
	/// Every panel subscribed to onRefreshPanel will be asked to update their contents. This method is executed 
	/// by other classes.
	/// </summary>
	public static void TriggerRefreshPanelEvent() {
		Instance.refreshPanelEventIsTriggered = true;
	}
}