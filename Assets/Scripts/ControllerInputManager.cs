//============== James A. Feister  Udacity Rube Goldberg Project ==============
// 
// Setup interactions on the controllers
// 
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Valve.VR;


public class ControllerInputManager : MonoBehaviour {
	#region "VARS"
	public static ControllerInputManager leftController = null;
	public static ControllerInputManager rightController = null;
	public GameObject player;
	public GameManager gameManager;
	public SteamVR_ControllerManager cm;

	//  Teleportation 
	[Header("Teleport Settings")]
	[SerializeField]
	private float TeleportLength = 10f;
	[SerializeField]
	private float DashSpeed = 0.5f;
	[SerializeField]
	private Material enabledTeleportMaterial;
	public Material EnabledTeleportMaterial {
		get {
			return enabledTeleportMaterial;
		}
	}
	[SerializeField]
	private Material disabledTeleportMaterial;
	public Material DisabledTeleportMaterial {
		get {
			return disabledTeleportMaterial;
		}
	}

	[SerializeField]
	private GameObject TeleportMarker = null;

	//  Teleportation Variables
	private int teleportLayer;
	private float lerptime;
	private bool TeleportPadTouched = false;
	private  LineRenderer laserPointer;
	private Vector3 teleportLocation;
	private Vector3 dashStart;
	private Vector3 dashDestination;
	private bool isDashing = false;
	private bool rayHitInTeleportArea = false;
	private MyTeleportArea[] teleportAreas;

	//  Grabbing Objects Variables
	GameObject hoverObject;
	bool grabbing;
	private bool grabbedRidgidBodyOriginalKinematicState;
	private bool grabbedRidgidBodyOriginalGravityState;
	private FixedJoint grabJoint;

	//  Menu Variables
	[SerializeField]
	private MenuController objectMenuController = null;
	[SerializeField]
	private Transform objectMenuSpawnPoint = null;
	private bool menuEnabled = false;

	//  Hand
	private string handName = "";
	SteamVR_TrackedObject controllerTrackedObject = null;
	SteamVR_TrackedController trackedController = null;
	SteamVR_Controller.Device controllerDevice = null;

	public string HandName {
		get {
			return handName;
		}
	}

	//  Tutorial
	private bool tutorialTeleported = false;
	private bool tutorialGrabbedBall = false;
	private bool tutorialCreatedObject = false;
	private Coroutine teleportHintCoroutine = null;
	private Coroutine grabHintCoroutine = null;
	private Coroutine createObjectHintCoroutine = null;

	//  Hand Assignment
	private int assignedIndex = 0;
	private bool assignedToAnIndex = false;
	private ControllerInputManager[] controllers;

	//  Hints
	//  This whole section should be in its own class.
	[SerializeField]
	public GameObject padHint = null;
	[SerializeField]
	public GameObject triggerHint = null;

	public ushort hapticAlertDurration = 300;
	public ushort hapticInfoDurration = 800;
	private EVRButtonId padHintButton = EVRButtonId.k_EButton_SteamVR_Touchpad;
	private Transform padHintStart;
	private Transform padHintEnd;
	private LineRenderer padHintLineRenderer;
	private EVRButtonId triggerHintButton = EVRButtonId.k_EButton_SteamVR_Trigger;
	private EVRButtonId hintButton;
	private Transform triggerHintStart;
	private Transform triggerHintEnd;
	private LineRenderer triggerHintLineRenderer;
	private bool showingHint = false;

	//  Straight from valves code
	private float startTime;
	private float tickCount;

	//  Other
	Transform reference {
		get {
			var top = SteamVR_Render.Top ();
			return (top != null) ? top.origin : null;
		}
	}
	#endregion

	void ConnectedDeviceEvent(int i, bool b) {
		Debug.Log ("Device Connected");
		Debug.Log(OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand).ToString());
		Debug.Log(OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand).ToString());
				
		Debug.Log(SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Leftmost).ToString());
	}

	void Awake () {
		gameManager = GameObject.FindObjectOfType<GameManager> ();
	}
	// Use this for initialization
	void Start () {
		//  Setup the Controller
		Debug.Log("Start");
		trackedController = GetComponent<SteamVR_TrackedController> ();
		if (trackedController == null ) {
			Debug.Log ("Start - Created a trackedController Object");
			trackedController = gameObject.AddComponent<SteamVR_TrackedController> ();
		}
		controllerTrackedObject = gameObject.GetComponent<SteamVR_TrackedObject> ();
		Debug.Log ("Start - TrackedObject Index: " + controllerTrackedObject.index.ToString ());
		Debug.Log ("Start - controllerTrackedObject.index: " + ((int)(controllerTrackedObject.index)).ToString());

		this.BroadcastMessage( "OnHandInitialized", assignedIndex, SendMessageOptions.DontRequireReceiver ); // let child objects know we've initialized

		//  TUTORIAL
		tutorialGrabbedBall = false;
		tutorialTeleported = false;
		tutorialCreatedObject = false;

	}

	//  Setup the device for left hand or right hand functionallity.
	public void SetupUpDevice () {
		//  Get current role index's
		int lindex = (int)cm.left.GetComponent<SteamVR_TrackedObject> ().index;
		int rindex = (int)cm.right.GetComponent<SteamVR_TrackedObject> ().index;
		Debug.Log(assignedIndex.ToString() + " - SetupUpDevice LeftIndex: " + lindex.ToString());
		Debug.Log (assignedIndex.ToString () + " - SetupUpDevice RightIndex: " + rindex.ToString());

		//  Left hand or Right hand role assignement
		if (assignedIndex == lindex && leftController == null) {
			//  Setup left hand functionality
			leftController = this;
			handName = "left";
			Debug.Log(assignedIndex.ToString() + " - Hand: " + handName);

			trackedController = GetComponent<SteamVR_TrackedController> ();
			if (trackedController == null ) {
				trackedController = gameObject.AddComponent<SteamVR_TrackedController> ();
			}
			controllerTrackedObject = gameObject.GetComponent<SteamVR_TrackedObject> ();

			//  Assign Teleportation Events
			trackedController.PadTouched += new ClickedEventHandler (OnTeleportButtonOn);
			trackedController.PadUntouched += new ClickedEventHandler (OnTeleportButtonOff);
			trackedController.PadClicked += new ClickedEventHandler (OnTeleportButtonDown);
			trackedController.PadUnclicked += new ClickedEventHandler (OnTeleportButtonUp);

			//  Setup Teleportation Lazer
			laserPointer = gameObject.AddComponent<LineRenderer> ();
			laserPointer.startWidth = 0.02f;
			laserPointer.endWidth = 0.02f;
			laserPointer.enabled = false;

			//  Grab Teleportation Areas, Kinda the way Valve does it in their examples
			//  We highlight them on teleport button press.
			teleportAreas = GameObject.FindObjectsOfType<MyTeleportArea> ();

			//  Grab Teleportation Layer for our raycasts
			teleportLayer = LayerMask.GetMask ("TeleportLayer");

			//  Teleportation Marker
			if (TeleportMarker == null) {
				TeleportMarker = GameObject.Find ("PlayerTeleportMarker");
			}
			TeleportMarker.SetActive (false);

			//  Enable the Teleportation Tutorial
			if (gameManager.tutorial) {
				ControllerInputManager.leftController.Invoke ("StartTeleportHint", 2.0f);
			}
			Debug.Log(assignedIndex.ToString() + " - Hand: " + handName + " DONE!");
		} else if (assignedIndex == rindex) {
			//  Setup right hand controller functionality
			rightController = this;
			handName = "right";
			Debug.Log(assignedIndex.ToString() + " - Hand: " + handName);

			trackedController = GetComponent<SteamVR_TrackedController> ();
			if (trackedController == null ) {
				trackedController = gameObject.AddComponent<SteamVR_TrackedController> ();
			}
			controllerTrackedObject = gameObject.GetComponent<SteamVR_TrackedObject> ();


			//  Assign Menu events
			objectMenuController = gameObject.GetComponentInChildren<MenuController> ();
			objectMenuSpawnPoint = objectMenuController.spawnPoint.transform;

			trackedController.PadTouched += new ClickedEventHandler (OnMenuButtonOn);
			trackedController.PadUntouched += new ClickedEventHandler (OnMenuButtonOff);
			trackedController.PadClicked += new ClickedEventHandler (OnMenuButtonClicked);
			trackedController.TriggerClicked += new ClickedEventHandler (OnMenuAction);
			Debug.Log(assignedIndex.ToString() + " - Hand: " + handName + " DONE!");
		}

		//  Setup Both hands to grab things, they would be left or right hand
		if (handName != "") {
			//  Required for our physics calculations
			controllerDevice = SteamVR_Controller.Input ((int)controllerTrackedObject.index);

			//  Assigned grab events
			trackedController.TriggerClicked += new ClickedEventHandler (OnControllerTriggerClicked);
			trackedController.TriggerUnclicked += new ClickedEventHandler (OnControllerTriggerUnclicked);

			//  Grab state
			grabbing = false;

			//  Grabed object attach point
			grabJoint = gameObject.GetComponent<FixedJoint> ();
			SteamVR_Controller.Input(assignedIndex).TriggerHapticPulse(hapticInfoDurration);
			//  Setup hints
			padHint = transform.Find("Hints/TrackpadHint").gameObject;
			padHint.SetActive (false);
			padHintLineRenderer = padHint.transform.GetComponentInChildren<LineRenderer> ();
			padHintStart = transform.Find ("Hints/TrackpadHint/Text/Valve-ControllerTextHint/Start");
			padHintEnd = transform.Find ("Hints/TrackpadHint/Text/Valve-ControllerTextHint/End");

			triggerHint = transform.Find ("Hints/TriggerHint").gameObject;
			triggerHint.SetActive (false);
			triggerHintLineRenderer = triggerHint.transform.GetComponentInChildren<LineRenderer> ();
			triggerHintStart = transform.Find ("Hints/TriggerHint/Text/Valve-ControllerTextHint/Start");
			triggerHintEnd = transform.Find ("Hints/TriggerHint/Text/Valve-ControllerTextHint/End");
		}		


	}
	public void SetDeviceIndex (int trackedDeviceIndex) {
		Debug.Log ("SetDeviceIndex");
		int lindex = (int)cm.left.GetComponent<SteamVR_TrackedObject> ().index;
		int rindex = (int)cm.right.GetComponent<SteamVR_TrackedObject> ().index;
		if (assignedToAnIndex)
			return;
		assignedIndex = trackedDeviceIndex;
		assignedToAnIndex = true;

		//  Get tracked controller and assign one if it does not exist.
		trackedController = GetComponent<SteamVR_TrackedController> ();
		if (trackedController == null ) {
			Debug.Log ("Created a trackedController Object");
			trackedController = gameObject.AddComponent<SteamVR_TrackedController> ();
		}
		controllerTrackedObject = gameObject.GetComponent<SteamVR_TrackedObject> ();
		Debug.Log (trackedDeviceIndex.ToString() + " -  TrackedObject Index: " + controllerTrackedObject.index.ToString ());


		Debug.Log(trackedDeviceIndex.ToString() + " - SetDeviceIndex assigned index: " + assignedIndex.ToString());
		Debug.Log(trackedDeviceIndex.ToString() + " - SetDeviceIndex trackedDeviceIndex: " + trackedDeviceIndex.ToString() );
		Debug.Log(trackedDeviceIndex.ToString() + " - SetDeviceIndex LeftIndex: " + lindex.ToString());
		Debug.Log(trackedDeviceIndex.ToString() + " - SetDeviceIndex RightIndex: " + rindex.ToString());

		//  Grab Controllers
		controllers = GameObject.FindObjectsOfType<ControllerInputManager> ();
		if (controllers.Length <= 1) {
			Debug.Log ("SetDeviceIndex - Not enough controllers found");
			return;
		}

		//  Check both controllers are assign an index #
		for (int c = 0; c < controllers.Length; c++) {
			if (!controllers [c].assignedToAnIndex ) {
				//  Let the other controller call the setup.
				return;
			}
		}
		Debug.Log (assignedIndex.ToString() + " -  start Both Hands assigned now calling setup");
		//  If they are settled down, call them.
		for (int c = 0; c < controllers.Length; c++) {
			Debug.Log ("c: " + c.ToString ());
//			controllers [c].gameObject.SendMessage ("SetupUpDevice");
			controllers [c].SetupUpDevice();
		}

	}

	//  Menu Actions
	void OnMenuButtonOn (object sender, ClickedEventArgs e) {
		menuEnabled = true;
		objectMenuController.EnableMenu ();
	}
	void OnMenuButtonOff (object sender, ClickedEventArgs e) {
		menuEnabled = false;
		objectMenuController.DisableMenu ();
	}
	void OnMenuButtonClicked (object sender, ClickedEventArgs e) {
		Debug.Log ("Menu Button Clicked");
		if (e.padX <= 0) {
			Debug.Log ("Menu Button Clicked - Left");
			//  Rotate Menu Left
			objectMenuController.PrevMenuItem();
		} 
		else if (e.padX > 0) {
			Debug.Log ("Menu Button Clicked - Right");
			//  Rotate Menu Right
			objectMenuController.NextMenuItem();
		}
	}

	public void OnMenuAction (object sender, ClickedEventArgs e) {
		Debug.Log ("OnMenuAction Clicked");
		if (!menuEnabled) {
			return;
		}
		//  Get the current GameObject to spawn
		GameObject newObject = objectMenuController.GetMenuItemPrefab();
		//  Spawn it!!
		if (newObject != null) {
			Debug.Log ("Menu - Spawning Object");
			GameObject instObject = GameObject.Instantiate (newObject);
			//instObject.transform.position = objectMenuController.transform.position;
			instObject.transform.position = objectMenuSpawnPoint.position;

			//  Tutorial
			if (gameManager.tutorial) {
				Debug.Log ("tutorialCreatedObject: " + (!tutorialCreatedObject).ToString () + "tutorialGrabbedBall: " + tutorialGrabbedBall.ToString ());
				if (!tutorialCreatedObject && tutorialGrabbedBall) {
					ControllerInputManager.rightController.tutorialCreatedObject = true;
					ControllerInputManager.leftController.tutorialCreatedObject = true;
					ControllerInputManager.rightController.StopCreateObjectHint ();
					ControllerInputManager.leftController.StopCreateObjectHint ();
					gameManager.SendMessage ("CreatedObject");
				}
			}
		} else {
			Debug.Log ("No Object to spawn");
		}
	}

	//  -----------------------
	//  Grabbing
	//  -----------------------
	#region "Grabbing"
	void OnTriggerEnter( Collider col) {
/*		Debug.Log ("Trigger ENTER: Grabber Collision");*/
		if( (col.gameObject.CompareTag ("Structure") || col.gameObject.CompareTag ("Throwable")) && hoverObject == null && !grabbing) {
			hoverObject = col.gameObject;
		}
	}
	void OnTriggerExit( Collider col) {
/*		Debug.Log("Trigger EXIT: Grabber Collision");*/
		if( !grabbing && col.gameObject == hoverObject) {
			hoverObject = null;
		}

		//  If some reason the object was ripped from our hand
		if ( grabbing && col.gameObject == hoverObject ) {
			LetGoOfObject ();
			hoverObject = null;
		}
	}

	void LetGoOfObject () {
		Rigidbody rb = hoverObject.gameObject.GetComponent<Rigidbody> ();
		if( rb != null) {
			rb.isKinematic = grabbedRidgidBodyOriginalKinematicState;
			rb.useGravity = grabbedRidgidBodyOriginalGravityState;
			grabJoint.connectedBody = null;
			hoverObject.transform.parent = null;
			if (hoverObject.gameObject.CompareTag ("Throwable")) {
				Debug.Log ("Throwable Object");
				Debug.Log ("controllerDevice.angularVelocity: " + controllerDevice.angularVelocity);
				Debug.Log ("controllerDevice.velocity: " + controllerDevice.velocity);
					
				rb.angularVelocity = controllerDevice.angularVelocity;
				rb.velocity = controllerDevice.velocity;
				rb.maxAngularVelocity = controllerDevice.angularVelocity.magnitude;
				hoverObject.SendMessage ("Thrown", this, SendMessageOptions.DontRequireReceiver);
			} else {
				Debug.Log ("Non Throwable Object");
			}
			hoverObject.SendMessage ("UnGrabbed", this, SendMessageOptions.DontRequireReceiver);
		}
	}

	//  When user lets go of the controller trigger
	void OnControllerTriggerUnclicked( object sender, ClickedEventArgs e) {
		grabbing = false;
		if (hoverObject != null) {
			LetGoOfObject ();
		}
	}

	//  When user presses the controller trigger to grab an object
	void OnControllerTriggerClicked(object sender, ClickedEventArgs e) {
		Debug.Log ("OnControllerTriggerClicked: Trigger Clicked");
		grabbing = true;
		if (hoverObject != null) {
			Rigidbody rb = hoverObject.gameObject.GetComponent<Rigidbody> ();
			if(rb != null) {
				//  Grabbing the ball
				Debug.Log ("OnControllerTriggerClicked: grabbing object");
				grabbedRidgidBodyOriginalKinematicState = rb.isKinematic;
				grabbedRidgidBodyOriginalGravityState = rb.useGravity;
				grabJoint.connectedBody = rb;
				hoverObject.SendMessage ("OnGrabbed", this, SendMessageOptions.DontRequireReceiver);
				gameManager.SendMessage ("PlayerGrabbedBall", this);

				//  Tutorial
				if (gameManager.tutorial) {
					Debug.Log ("!tutorialGrabbedBall: " + (!tutorialGrabbedBall).ToString () + " tutorialTeleported: " + tutorialTeleported.ToString ()); 
					if (!tutorialGrabbedBall && tutorialTeleported) {
						Debug.Log ("Tutorial - Reseting Grab Ball Hint");
						ControllerInputManager.rightController.tutorialGrabbedBall = true;
						ControllerInputManager.leftController.tutorialGrabbedBall = true;
						ControllerInputManager.rightController.StopGrabHintCoroutine ();
						ControllerInputManager.leftController.StopGrabHintCoroutine ();
					}
					//  TUTORIAL
					//  Tell player where to throw ball when the pick it up.
					gameManager.SendMessage ("StartGameTutorial", this);
				}
			}
			SteamVR_Controller.Input(assignedIndex).TriggerHapticPulse(200);
			hoverObject.transform.parent = gameObject.transform;
		}
	}
	#endregion


	// --------------------------
	//  Teleportation
	// -------------------------
	#region "Teleportation"
	//  Tell the teleportable areas to highlight themselves.
	public void HighlightTeleportAreas (bool highlight) {
		foreach (MyTeleportArea teleportArea in teleportAreas ) {
			teleportArea.Hightlight (highlight);
		}
	}

	//  When user touches the pad
	void OnTeleportButtonOn (object sender, ClickedEventArgs e) {
/*		Debug.Log ("Button On");*/
		TeleportPadTouched = true;
		UpdateTeleportVisual ();
		laserPointer.enabled = true;
		HighlightTeleportAreas (true);
	}

	//  When user stops touching the PAD
	void OnTeleportButtonOff (object sender, ClickedEventArgs e) {
/*		Debug.Log ("Button Off");*/
		TeleportPadTouched = false;
		HighlightTeleportAreas (false);
		TeleportMarker.SetActive (false);
		laserPointer.enabled = false;
	}

	//  User has started clicking the
	void OnTeleportButtonDown (object sender, ClickedEventArgs e) {
	}
	//  After a user has clicked the pad
	void OnTeleportButtonUp (object sender, ClickedEventArgs e) {
/*		Debug.Log ("Button UP");*/
		if (rayHitInTeleportArea) {
			dashStart = new Vector3 (SteamVR_Render.Top ().head.position.x, reference.position.y, SteamVR_Render.Top ().head.position.z);
			dashDestination = teleportLocation + (reference.position - new Vector3 (SteamVR_Render.Top ().head.position.x, teleportLocation.y, SteamVR_Render.Top ().head.position.z));
			isDashing = true;	
		}
	}

	//  Dash the player
	void DashPlayer () {
		lerptime += 1 * DashSpeed;
		player.transform.position = Vector3.Lerp (dashStart, dashDestination, lerptime);
		if (lerptime >= 1) {
			isDashing = false;
			lerptime = 0;
		}
		if (gameManager.tutorial) {
			StopTeleportHintCoroutine ();
			if (!tutorialTeleported) {
				ControllerInputManager.leftController.tutorialTeleported = true;
				ControllerInputManager.rightController.tutorialTeleported = true;
				ControllerInputManager.rightController.Invoke ("StartGrabHint", 2.0f);
				ControllerInputManager.leftController.Invoke ("StartGrabHint", 2.0f);
			}
		}
	}

	void UpdateTeleportVisual () {
		laserPointer.SetPosition (0, gameObject.transform.position);
		RaycastHit hit;
		Ray ray = new Ray (transform.position, transform.forward);
		if (Physics.Raycast(ray, out hit, TeleportLength, teleportLayer)) {
			rayHitInTeleportArea = true;
			teleportLocation = hit.point;
			laserPointer.SetPosition (1, teleportLocation);
			TeleportMarker.transform.position = teleportLocation;
			TeleportMarker.SetActive (true);
		} else {
			rayHitInTeleportArea = false;
			TeleportMarker.SetActive (false);
			teleportLocation = reference.position;
			Vector3 laserPosition2 = new Vector3 (transform.forward.x * TeleportLength + transform.position.x, transform.forward.y * TeleportLength + transform.position.y, transform.forward.z * TeleportLength + transform.position.z);
			laserPointer.SetPosition (1, laserPosition2);
		}
	}
	#endregion

	// Update is called once per frame
	void Update () {
		if (isDashing) {
			DashPlayer ();
		} else {
			if (TeleportPadTouched) {
				UpdateTeleportVisual ();
			}
		}
		if (grabbing && hoverObject != null ) {
			hoverObject.SendMessage ("GrabbedUpdate", this, SendMessageOptions.DontRequireReceiver);
		}
		if (showingHint) {
			//  Right from valves code.
			float ticks = ( Time.realtimeSinceStartup - startTime );
			if ( ticks - tickCount > 1.0f )
			{
				tickCount += 1.0f;
				// OpenVR.System.TriggerHapticPulse ((uint)assignedIndex, (uint)hintButton, (char)hapticAlertDurration);
				// SteamVR_Controller.Input (assignedIndex).TriggerHapticPulse (hapticAlertDurration, hintButton);  EVRButtonId.k_EButton_SteamVR_Touchpad
				SteamVR_Controller.Input (assignedIndex).TriggerHapticPulse (hapticAlertDurration, EVRButtonId.k_EButton_SteamVR_Touchpad);  
			}
			if (triggerHint.activeSelf) {
				triggerHintLineRenderer.SetPosition (0, triggerHintStart.position);
				triggerHintLineRenderer.SetPosition (1, triggerHintEnd.position);
			}
			if (padHint.activeSelf) {
				padHintLineRenderer.SetPosition (0, padHintStart.position);
				padHintLineRenderer.SetPosition (1, padHintEnd.position);
			}
		}
	}

	// --------------------------
	//  Tutorial Hints - BEGIN
	//  Most of this pattern is borrowed from Valves implementation within Teleport.cs
	// --------------------------
	#region "Tutorial_hints"
	public void ShowHint(GameObject hint, EVRButtonId buttonID, string text) {
		//  Set text
		hint.GetComponentInChildren<Text> ().text = text;

		//  Show Hint
		hint.SetActive(true);

		//  Vibration
		showingHint = true;
		hintButton = buttonID;
		startTime = Time.realtimeSinceStartup;
		tickCount = 0.0f;
	}

	public void StopHint(GameObject hint) {
		hint.SetActive (false);
		showingHint = false;
	}

	private IEnumerator TeleportHintCoroutine() {
		while (true) {
			Debug.Log ("Tutorial - Teleport Hint - Flash Pad");
			ShowHint (padHint, padHintButton, "Teleport");
			yield return new WaitForSeconds (3.0f);
		}
	}
	private void StopTeleportHintCoroutine () {
		if (teleportHintCoroutine != null) {
			StopCoroutine (teleportHintCoroutine);
			teleportHintCoroutine = null;
		}
		CancelInvoke ("StartTeleportHint");
		StopHint (padHint);
	}
	public void StartTeleportHint () {
		StopTeleportHintCoroutine ();
		teleportHintCoroutine = StartCoroutine ( TeleportHintCoroutine() );
	}

	private IEnumerator GrabHintCoroutine () {
		while (true) {
			Debug.Log ("Tutorial - Grab Ball Hint - Enable Sign");
			Debug.Log ("Tutorial - Grab Ball Hint - Flash trigger");
			ShowHint (triggerHint, triggerHintButton, "Grab Ball");
			yield return new WaitForSeconds (3.0f);
		}
	}
	public void StartGrabHint() {
		StopGrabHintCoroutine ();
		grabHintCoroutine = StartCoroutine (GrabHintCoroutine());

	}
	public void StopGrabHintCoroutine() {
		if (grabHintCoroutine != null) {
			StopCoroutine (grabHintCoroutine);
			grabHintCoroutine = null;
		}
		CancelInvoke ("StartGrabHint");
		StopHint (triggerHint);
	}

	private IEnumerator CreateObjectHintCoroutine () {
		while (true) {
			Debug.Log ("Tutorial - Object Hint - Flash Pad");
			Debug.Log ("Tutorial - Object Hint - Flash Trigger");
			ShowHint (padHint, padHintButton, "Create Object");
			yield return new WaitForSeconds (3.0f);
		}
	}
	public void StopCreateObjectHint () {
		if (createObjectHintCoroutine != null) {
			StopCoroutine (createObjectHintCoroutine);
			createObjectHintCoroutine = null;
		}
		CancelInvoke ("StartCreateObjectHint");
		StopHint (padHint);
	}
	public void StartCreateObjectHint () {
		StopCreateObjectHint ();
		createObjectHintCoroutine = StartCoroutine (CreateObjectHintCoroutine ());
	}
	public void InvokeObjectHint () {
		Invoke ("StartCreateObjectHint", 2.0f);
	}
	//  Tutorial Hints - END
	#endregion
}
