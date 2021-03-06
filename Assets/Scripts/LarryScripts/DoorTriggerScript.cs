using UnityEngine;
using System.Collections;

public class DoorTriggerScript : Photon.MonoBehaviour {

	public Texture viewerTexture;
	private Texture originalTexture;
	private Texture originalDoor1Texture;
	private Texture originalDoor2Texture;
	private Texture originalDoor3Texture;
	private Texture originalDoor4Texture;
	
	public GameObject door1 = null;
	public GameObject door2 = null;
	public GameObject door3 = null;
	public GameObject door4 = null;
	
	private DoorScript ds1 = null;
	private DoorScript ds2 = null;
	private DoorScript ds3 = null;
	private DoorScript ds4 = null;
	
	private bool triggered = false;
			
	public enum TriggerMode {
		All_At_Once,
	   One_By_One_Sequential,
	   Hold_To_Open
	}	
	/*
	public enum TriggerFunc {
		OnEnter = 0,
		OnExit = 1,
		OnStay = 2,
		Start = 3
	}*/
	
	private int lastTriggered;
	public TriggerMode triggerMode;
		
	private int sequentialCounter = 0;	

		
	private bool started = false;
	
	public static bool revealColours = false;	
	
	private Transform target = null;
		
	
	void resetDoorTime(PhotonTargets ptarget)
	{
		photonView.RPC("resetDoors",ptarget, false);		
	}
	
	void Awake()
	{
		
		target = this.transform.FindChild("Lever");	
		if (target == null)
			target = this.transform.FindChild("PressurePlate");
	}
	// Use this for initialization
	void Start () 
	{
		started = false;
		
		if (target == null)
			originalTexture = this.renderer.material.mainTexture;
		else
			originalTexture = target.renderer.material.mainTexture;
		
		if (door1){
			ds1 = door1.GetComponent<DoorScript>();
			originalDoor1Texture = door1.renderer.material.mainTexture;
		}
		
		if (door2){
			ds2 = door2.GetComponent<DoorScript>();
			originalDoor2Texture = door2.renderer.material.mainTexture;
		}
	
		if (door3){
			ds3 = door3.GetComponent<DoorScript>();
			originalDoor3Texture = door3.renderer.material.mainTexture;
		}
		
		if (door4){
			ds4 = door4.GetComponent<DoorScript>();
			originalDoor4Texture = door4.renderer.material.mainTexture;
		}
	}	
	
	void FixedUpdate()
	{
		GameObject SpawnManager = GameObject.Find("Code");
		GameManagerVik MoverTest = SpawnManager.GetComponent<GameManagerVik>();
			
			if (!started)
			{
				
				if (MoverTest.gameStarted)
				{		
					resetDoorTime(PhotonTargets.All);
				lastTriggered = 3;
					started = true;	
			
				}	
			}		
					
			
			if(MoverTest.selectedClass == "Viewer"){
			//print(revealColours);
				if(revealColours == true)
				{				
					
					if (target == null)
						this.renderer.material.mainTexture = viewerTexture;
					else
						target.renderer.material.mainTexture = viewerTexture;
					
					if(door1)
						door1.renderer.material.mainTexture = viewerTexture;
					if(door2)
						door2.renderer.material.mainTexture = viewerTexture;
					if(door3)
						door3.renderer.material.mainTexture = viewerTexture;
					if(door4)
						door4.renderer.material.mainTexture = viewerTexture;
				}
				else{
				
					if (target == null)
						this.renderer.material.mainTexture = originalTexture;
					else
						target.renderer.material.mainTexture = originalTexture;
					
					if(door1)
						door1.renderer.material.mainTexture = originalDoor1Texture;
					if(door2)
						door2.renderer.material.mainTexture = originalDoor2Texture;
					if(door3)
						door3.renderer.material.mainTexture = originalDoor3Texture;
					if(door4)
						door4.renderer.material.mainTexture = originalDoor4Texture;
				}
			}
		
		
	}		
		
	//The door will open/shut upon touch, doesn't matter if the player is on the button or not. Therefore TriggerEntry is the best method, doors will not have
	//to reset this way.
	void OnTriggerEnter(){
	    if (ds1 == null && ds2 == null && ds3 == null && ds4 == null) return;
		if (triggerMode == TriggerMode.Hold_To_Open) return;
		if (!started) return;
		if(!photonView.isMine)return;
		photonView.RPC("openDoors",PhotonTargets.AllBuffered,0);	
		/*
		if (triggerMode == TriggerMode.One_By_One_Sequential)
		{
				
			if (sequentialCounter == 0)
				{
					if (ds1 == null)
						//skip doors that do not exist
						sequentialCounter ++;
					else
						ds1.TriggerDoor();
				}
			if (sequentialCounter == 1)
				{
					if (ds2 == null)
						sequentialCounter ++;
					else
						ds2.TriggerDoor();
				}
			if (sequentialCounter == 2)
				{				
					if (ds3 == null)
						sequentialCounter ++;
					else
						ds3.TriggerDoor();
				}	
			
			if (sequentialCounter == 3)
				{				
					//there's only a limit of 4 doors. If all 4 doors don't exist, nothing happens.
					if (ds4 != null)
						ds4.TriggerDoor();
				}	
					
					
			sequentialCounter++;
			if (sequentialCounter > 3) sequentialCounter = 0;
		}
		else if(triggerMode == TriggerMode.All_At_Once){
			if (ds1 != null)	
				ds1.TriggerDoor();		
			
			if (ds2 != null)	
				ds2.TriggerDoor();	
	
			if (ds3 != null)	
				ds3.TriggerDoor();	
	
			if (ds4 != null)	
				ds4.TriggerDoor();	
		}
		*/
	}
		
		void OnTriggerStay(){
			if (triggerMode != TriggerMode.Hold_To_Open) return;
				if (!started) return;

		if(!photonView.isMine)return;
			if (triggered) return;
			photonView.RPC("openDoors",PhotonTargets.AllBuffered, 1);	

			
			/*
			if(triggerMode == TriggerMode.Hold_To_Open && !triggered){				
				if (ds1 != null)	
					ds1.TriggerDoor();		
				
				if (ds2 != null)	
					ds2.TriggerDoor();	
		
				if (ds3 != null)	
					ds3.TriggerDoor();	
		
				if (ds4 != null)	
					ds4.TriggerDoor();	
			
				triggered = true;
			}
			*/
		}	
	
		void OnTriggerExit(){
				if (!started) return;

		if(!photonView.isMine)return;
				photonView.RPC("openDoors",PhotonTargets.AllBuffered,2);	

		
		/*
			if(triggerMode == TriggerMode.Hold_To_Open && triggered){				
				if (ds1 != null)	
					ds1.TriggerDoor();		
				
				if (ds2 != null)	
					ds2.TriggerDoor();	
		
				if (ds3 != null)	
					ds3.TriggerDoor();	
		
				if (ds4 != null)	
					ds4.TriggerDoor();	
			
				triggered = false;
			}
			*/
		}
	
		public void toggleRevealColours(){
			revealColours = !revealColours;
		}
	public void showColours(){
			revealColours = true;
	}
	public void hideColours(){
			revealColours = false;
	}
	
	
	//RPCs
		[RPC] 
	void resetDoors(bool doors)
	{	
		//Debug.Log("Resetting Doors");
		lastTriggered = 3;
		if (ds1 != null)	
				ds1.ResetDoor();	
	
			
		if (ds2 != null)	
				ds2.ResetDoor();	
	
	
		if (ds3 != null)	
				ds3.ResetDoor();	
	
	
		if (ds4 != null)	
				ds4.ResetDoor();	
	
	
		sequentialCounter = 0;
	}
	
	[RPC] 
	void openDoors(int mode)
	{
	//	if (last_trigger_time > 0) return;
		
	//	if (mode == lastTriggered) return;
		
		//lastTriggered = mode;
		//on trigger enter		
		if (mode == 0)
		{
			if (triggerMode == TriggerMode.Hold_To_Open) return;
			if (triggerMode == TriggerMode.One_By_One_Sequential)
			{
				
				if (sequentialCounter == 0)
					{
						
						if (ds1 == null)
							//skip doors that do not exist
							sequentialCounter ++;
						else
							ds1.TriggerDoor();
					}
				if (sequentialCounter == 1)
					{
						if (ds2 == null)
							sequentialCounter ++;
						else
							ds2.TriggerDoor();
					}
				if (sequentialCounter == 2)
					{				
						if (ds3 == null)
							sequentialCounter ++;
						else
							ds3.TriggerDoor();
					}	
				
				if (sequentialCounter == 3)
					{				
						//there's only a limit of 4 doors. If all 4 doors don't exist, nothing happens.
						if (ds4 != null)
						
							ds4.TriggerDoor();
						//else
						//	sequentialCounter++;
					}	
						
				sequentialCounter++;	
				
				if (sequentialCounter >= 3) sequentialCounter = 0;
			}
			else if(triggerMode == TriggerMode.All_At_Once){
				if (ds1 != null)	
					ds1.TriggerDoor();		
				
				if (ds2 != null)	
					ds2.TriggerDoor();	
		
				if (ds3 != null)	
					ds3.TriggerDoor();	
		
				if (ds4 != null)	
					ds4.TriggerDoor();	
			}
		}
		
		
		if (mode == 1)
		{
			//if (triggerMode != TriggerMode.Hold_To_Open) return;
			//if (triggered) return;
			if(triggerMode == TriggerMode.Hold_To_Open && !triggered){				
				if (ds1 != null)	
					ds1.TriggerDoor();		
				
				if (ds2 != null)	
					ds2.TriggerDoor();	
		
				if (ds3 != null)	
					ds3.TriggerDoor();	
		
				if (ds4 != null)	
					ds4.TriggerDoor();	
			
				triggered = true;
			}
		}
		
		if (mode == 2)
		{
			
			//if (triggerMode != TriggerMode.Hold_To_Open) return;
			if(triggerMode == TriggerMode.Hold_To_Open && triggered){				
				if (ds1 != null)	
					ds1.TriggerDoor();		
				
				if (ds2 != null)	
					ds2.TriggerDoor();	
		
				if (ds3 != null)	
					ds3.TriggerDoor();	
		
				if (ds4 != null)	
					ds4.TriggerDoor();	
			
				triggered = false;
			}
		}
	}
	
	
	
}