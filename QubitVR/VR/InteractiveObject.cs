//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem.Sample
{
	//-------------------------------------------------------------------------
	/** Controls VR interactions with gates (e.g. grabbing)
	*
	* Example file provided by Valve. Heavily modified for custom functionality.
	* Modifications include sound, gate alerts to ModuleManager, and grab validation.
	*/
	[RequireComponent(typeof(Interactable))]
	public class InteractiveObject : MonoBehaviour
	{
		public TextMeshProUGUI toolText;
		private ToolObject m_toolObject = null;

		// SteamVR probably has an indicator like this, but this is simple and easily accessable...
		public bool AttachedToHand { get; set; }

		private Interactable interactable;
		private Vector3 oldPosition;
		private Quaternion oldRotation;

		private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

		public AudioSource audioSource;
		public AudioClip gatePickup, gateApplied;

		//-------------------------------------------------
		void Awake()
		{
			toolText.enabled = false;

			interactable = this.GetComponent<Interactable>();
			AttachedToHand = false;

			m_toolObject = transform.gameObject.GetComponent<ToolObject>();
		}

		//-------------------------------------------------
		// Called when a Hand starts hovering over this object
		//-------------------------------------------------
		private void OnHandHoverBegin(Hand hand)
		{
			// Turn on the label of the tools name.
			toolText.enabled = true;
		}

		//-------------------------------------------------
		// Called when a Hand stops hovering over this object
		//-------------------------------------------------
		private void OnHandHoverEnd(Hand hand)
		{
			// Turn off the label of the tools name.
			toolText.enabled = false;
		}

		//-------------------------------------------------
		// Called every Update() while a Hand is hovering over this object
		//-------------------------------------------------
		private void HandHoverUpdate(Hand hand)
		{
			if (m_toolObject == null)
            {
				Debug.LogError("ToolObject is null in InteractiveObject");
				return;
			}	

			GrabTypes startingGrabType = hand.GetGrabStarting();
			bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

			// User can only grab the object with the grip button.
			// This was chosen because the trigger button triggers the laserpointer and causes unpleasant flashes when grabbing objects.
			if (interactable.attachedToHand == null && startingGrabType == GrabTypes.Grip) // startingGrabType != GrabTypes.None
			{
				m_toolObject.setGateIsBeingHeld(true);
				m_toolObject.setGeneralFlagInQubitManager("grabbed");

				// Save our position/rotation so that we can restore it when we detach
				oldPosition = transform.localPosition;
				oldRotation = transform.localRotation;

				// Rotate the object so it aligns with the hand before it attaches.
				transform.forward = hand.transform.forward;

				// Call this to continue receiving HandHoverUpdate messages,
				// and prevent the hand from hovering over anything else
				hand.HoverLock(interactable);

				// Attach this object to the hand
				hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
			}
			else if (isGrabEnding)
			{
				m_toolObject.setGateIsBeingHeld(false);
				m_toolObject.setGeneralFlagInQubitManager("released");

				// If gate was applied, play corresponding sound.
				audioSource.clip = gateApplied;
				audioSource.Play();

				// Detach this object from the hand
				hand.DetachObject(gameObject);

				// Call this to undo HoverLock
				hand.HoverUnlock(interactable);

				// Restore position/rotation
				transform.localPosition = oldPosition;
				transform.localRotation = oldRotation;
			}
		}


		//-------------------------------------------------
		// Called when this GameObject becomes attached to the hand
		//-------------------------------------------------
		private void OnAttachedToHand(Hand hand)
		{
			// Turn off the label of the tools name.
			toolText.enabled = false;
			AttachedToHand = true;

			audioSource.clip = gatePickup;
			audioSource.Play(0);

			// This turns off the laserpointer while the object is held by the right hand
			// (We only have the laser pointer on the right hand)
			if (hand.handType == SteamVR_Input_Sources.RightHand)
				EventManager.PickedUpItem.Invoke();
		}

		//-------------------------------------------------
		// Called when this GameObject is detached from the hand
		//-------------------------------------------------
		private void OnDetachedFromHand(Hand hand)
		{
			// This turns on the laserpointer because the object is no longer held
			AttachedToHand = false;
			EventManager.DroppedItem.Invoke();
		}

		private bool lastHovering = false;
		private void Update()
		{
			if (interactable.isHovering != lastHovering) //save on the .tostrings a bit
				lastHovering = interactable.isHovering;
		}
	}
}