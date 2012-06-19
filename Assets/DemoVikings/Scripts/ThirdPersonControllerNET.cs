using UnityEngine;
using System.Collections;

public delegate void JumpDelegate ();
enum GravityGunState { Free, Catch, Occupied, Charge, Release};
public class ThirdPersonControllerNET : Photon.MonoBehaviour
{
	
	private bool menuOn = false;
	
	
	private int level_number=0;
	public Rigidbody target;
	public static int blockammo;
	public static int plankammo;
	private GravityGunState gravityGunState =0;
	
	public float triggerHoldRange = 2.0f;
	public float holdDistance = 1.0f;
	
	private Rigidbody rigid;
		// The object we're steering
	public float speed = 1.5f, jumpforwardspeed = 2.5f, walkSpeedDownscale = 1.0f, 
	jumpSpeedDownScale = 3.0f,
	turnSpeed = 2.0f, mouseTurnSpeed = 0.9f, jumpSpeed = 1.0f;
		// Tweak to ajust character responsiveness
	public LayerMask groundLayers = -1;
		// Which layers should be walkable?
		// NOTICE: Make sure that the target collider is not in any of these layers!
	public float groundedCheckOffset;
		// Tweak so check starts from just within target footing
	public bool
		showGizmos = true,
			// Turn this off to reduce gizmo clutter if needed
		requireLock = true,
			// Turn this off if the camera should be controllable even without cursor lock
		controlLock = true;
			// Turn this on if you want mouse lock controlled by this script
	public JumpDelegate onJump = null;
		// Assign to this delegate to respond to the controller jumping
	
	
	private const float inputThreshold = 0.01f,
		groundDrag = 5.0f,
		directionalJumpFactor = 0.7f;
		// Tweak these to adjust behaviour relative to speed
	private const float groundedDistance = 0.2f;
		// Tweak if character lands too soon or gets stuck "in air" often
		
	
	private bool grounded, walking;

    private bool isRemotePlayer = true;
	
	public bool Grounded
	// Make our grounded status available for other components
	{
		get
		{
			return grounded;
		}
	}

    public void SetIsRemotePlayer(bool val)
    {
        isRemotePlayer = val;
    }

	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
	}
	
	
	void Setup ()
	// If target is not set, try using fallbacks
	{
		blockammo = 1;
		plankammo = 5;
		if (target == null)
		{
			target = GetComponent<Rigidbody> ();
		}
		
		
	}
	
	void Awake(){
		DontDestroyOnLoad(this);	
	}
	void Start ()
	// Verify setup, configure rigidbody
	{
		Setup ();
		
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.");
			enabled = false;
			return;
		}

		target.freezeRotation = true;
			// We will be controlling the rotation of the target, so we tell the physics system to leave it be
		walking = false;
	}
	
	
	void Update ()
	// Handle rotation here to ensure smooth application.
	{
        if (isRemotePlayer) return;
		
		
		float rotationAmount;
		
		
			if(target.name.Contains("Builder"))
			{
				if (Input.GetKeyUp("r")){
					if(blockammo>0){
						Playtomic.Log.LevelCounterMetric("BuildBlock", level_number);
						var builtBlock = PhotonNetwork.Instantiate("pBlock", transform.position + transform.forward, transform.rotation, 0);
						builtBlock.tag = "PlacedBlock";
					
						blockammo--;
					}
				}
				if (Input.GetKeyUp("t")){
					
					if(plankammo>0){
						Playtomic.Log.LevelCounterMetric("BuildPlank", level_number);
						var builtPlatform = PhotonNetwork.Instantiate("pPlatform", transform.position + transform.forward * transform.localScale.z * 2, transform.rotation, 0);
						builtPlatform.tag = "PlacedPlatform";
					
						plankammo--;
						}
					}
				
			}
			
			
		if(target.name.Contains("Mover"))
		{
				if(gravityGunState == GravityGunState.Free) 
				{
					    if(Input.GetKeyDown("t")) 
						{
									float range = target.transform.localScale.z * triggerHoldRange;
									//float rad = target.collider.radius;
									
					                RaycastHit hit;
									LayerMask layerMask = 1;
										print ("Searching for pickable objects");
									if (Physics.SphereCast(target.transform.position, 0.2f, target.transform.forward, out hit, 2.0f, layerMask))	
														   //origin,          height, direction,       , hit,   radius, layer
																			//Distance										radius
									//Vector3 endPoint = Vector3
									//if(Physics.CapsuleCast(target.transform.position, target.transform.position + target.transform.forward * range, 2.0, target.transform.forward, out hit, 0.0f, layerMask)) 
							//Physics.CapsuleCast(
									//Debug.DrawRay(transform.position, transform.forward, Color.green);
					
					                //if(Physics.Raycast(transform.position, transform.forward - transform.up, out hit, range, layerMask)) 
									{
					                    if(hit.rigidbody) 
										{
											Debug.Log("Picked an object");
					                        rigid = hit.rigidbody;
										
					                  //      rigid.isKinematic = true;
											//This prevents vikings from picking up other vikings. Only platforms and blocks can be picked up. 
											if (rigid.tag.Contains("BlockTrigger") || rigid.tag.Contains("PlacedBlock")
								 				|| rigid.tag.Contains("PlatformTrigger") || rigid.tag.Contains("PlacedPlatform"))
											{
					                        	if (rigid.gameObject.GetComponent<BoxUpdate>())
												{
													rigid.gameObject.GetComponent<BoxUpdate>().setCarry(true);
												}
												gravityGunState = GravityGunState.Catch;
											}
											else
					                       		rigid = null;
					                    }
					                }
					
//									if(Physics.Raycast(transform.position, transform.forward, out hit, range, layerMask)) 
//									{
//					                    if(hit.rigidbody) 
//										{
//					                        rigid = hit.rigidbody;
//										
//					                  //      rigid.isKinematic = true;
//											//This prevents vikings from picking up other vikings. Only platforms and blocks can be picked up. 
//											if (rigid.tag.Contains("PlatformTrigger") || rigid.tag.Contains("PlacedPlatform"))
//											{
//					                        	if (rigid.gameObject.GetComponent<BoxUpdate>())
//												{
//													Debug.Log("this?");
//													rigid.gameObject.GetComponent<BoxUpdate>().setCarry(true);
//												}
//												gravityGunState = GravityGunState.Catch;
//											}
//											else
//					                       		rigid = null;
//					                    }
//					                }
					     }
				}
				else if(gravityGunState == GravityGunState.Catch) 
				{
						holdDistance =  transform.localScale.z + rigid.transform.localScale.z;
				
					    rigid.transform.position = transform.position + transform.forward * holdDistance;
					    rigid.transform.rotation = transform.rotation;
					    if(!Input.GetKey("t"))
					           gravityGunState = GravityGunState.Occupied;     
				}
				else if(gravityGunState == GravityGunState.Occupied) 
				{            
					    rigid.transform.position = transform.position + transform.forward * holdDistance;
						rigid.transform.rotation = transform.rotation;
					    if(Input.GetKey("t"))
							gravityGunState = GravityGunState.Charge;
				}
			
				else if(gravityGunState == GravityGunState.Charge) 
				{
						rigid.transform.position = transform.position + transform.forward * holdDistance;
						rigid.transform.rotation = transform.rotation;
					    if(!Input.GetKey("t"))
					    {
							if(rigid.name.Contains("pPlatform"))
								rigid.isKinematic = true;
							else
							{
								if (rigid.gameObject.GetComponent<BoxUpdate>())
												{
													rigid.gameObject.GetComponent<BoxUpdate>().setCarry(false);
												}

								rigid.isKinematic = false;
							}
							gravityGunState = GravityGunState.Release;
					                   
					    }
				}
				else if(gravityGunState == GravityGunState.Release) 
				{
					            
					   	rigid = null;
						gravityGunState = GravityGunState.Free;
				}
		
		}
		
		//swapped
		//if (Input.GetMouseButton (1) && (!requireLock || controlLock || Screen.lockCursor))
		if (menuOn && (!requireLock || controlLock || Screen.lockCursor))
		
		// If the right mouse button is held, rotation is locked to the mouse
		{
			if (controlLock)
			{
				Screen.lockCursor = false;
			}
			
			rotationAmount = Input.GetAxis ("Horizontal") * turnSpeed * Time.deltaTime;
		
		}
		else
		{
			if (controlLock)
			{
				Screen.lockCursor = true;
			}
			
			rotationAmount = Input.GetAxis ("Mouse X") * mouseTurnSpeed * Time.deltaTime;
			
		}
		
		target.transform.RotateAround (target.transform.up, rotationAmount);
		
		if (Input.GetKeyDown(KeyCode.Backslash) || Input.GetKeyDown(KeyCode.Plus))
		{
			walking = !walking;
		}
		
		//turn off an on menu
		if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Escape))
		{
			menuOn = !menuOn;
		}
	}
	
	
	float SidestepAxisInput
	// If the right mouse button is held, the horizontal axis also turns into sidestep handling
	{
		get
		{
			//Swapped
			if (menuOn)//Input.GetMouseButton (1))
			{
				//not supposed to move when menu is on.
				float sidestep = 0;// -(Input.GetKey(KeyCode.Q) ? 1 : 0) + (Input.GetKey(KeyCode.E) ? 1 : 0);
                return sidestep;

			}
			else
			{
				float sidestep = -(Input.GetKey(KeyCode.Q)?1:0) + (Input.GetKey(KeyCode.E)?1:0);
                float horizontal = Input.GetAxis ("Horizontal");
				
				return Mathf.Abs (sidestep) > Mathf.Abs (horizontal) ? sidestep : horizontal;

				
			}
		}
	}
	
	
	bool isFourPointGrounded()
	{
		bool pt1, pt2, pt3, pt4;
		
	    Vector3 dist_forward = target.transform.forward/2;
		Vector3 dist_right = target.transform.right/2;
		
		pt1 = Physics.Raycast (                                                                                                                    
			
			(target.transform.position + dist_forward) + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);

		pt2 = Physics.Raycast (                                                                                                                    
			
			(target.transform.position - dist_forward) + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);
		pt3 = Physics.Raycast (                                                                                                                    
			
			(target.transform.position + dist_right) + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);
		pt4 = Physics.Raycast (                                                                                                                    
			
			(target.transform.position - dist_right) + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);
		
		if (pt1 == false && pt2 == false && pt3 == false && pt4 == false)		
			return false;
		else //any one of the points is touching the ground
			return true;
	}
	
	

	void FixedUpdate ()
	// Handle movement here since physics will only be calculated in fixed frames anyway
	{

		
		grounded = isFourPointGrounded ();
		//print (grounded);
      	if (isRemotePlayer) return;
		if (menuOn) return;	
	
		if (grounded)
		{
			
			
			target.drag = groundDrag;
				// Apply drag when we're grounded
			
			if (Input.GetButton ("Jump") || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			// Handle jumping
			{	
				Playtomic.Log.Heatmap("Movement2", "Level0", 1 , 1);
				print("sending analytics");
				target.AddForce (
					jumpSpeed * target.transform.up +
						target.velocity.normalized * directionalJumpFactor,
					ForceMode.VelocityChange
				);
					// When jumping, we set the velocity upward with our jump speed
					// plus some application of directional movement
				
				if (onJump != null)
				{
					onJump ();
				}
				
			}
			else
			// Only allow movement controls if we did not just jump
			{
				Vector3 movement = Input.GetAxis ("Vertical") * target.transform.forward +
					SidestepAxisInput * target.transform.right;
				
				float appliedSpeed = walking ? speed / walkSpeedDownscale : speed;
					// Scale down applied speed if in walk mode
				
				/*
				if (Input.GetAxis ("Vertical") < 0.0f)
				// Scale down applied speed if walking backwards
				{
					appliedSpeed /= walkSpeedDownscale;
				}*/
				
				//Larry - this movement threshold thing is what's causing the viking to fall through the elevator! 
				//if (movement.magnitude > inputThreshold)
				// Only apply movement if we have sufficient input
				//{
					target.AddForce (movement.normalized * appliedSpeed, ForceMode.VelocityChange);
				//}
				//else
				// If we are grounded and don't have significant input, just stop horizontal movement
				//{
			//		target.velocity = new Vector3 (0.0f, target.velocity.y, 0.0f);
		//			return;
		//		}
			}
		}
		else
		{
			    //clamping jump speed
				if (rigidbody.velocity.y > jumpSpeed)
					rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpSpeed, rigidbody.velocity.z);
				target.drag = 0.5f;
				
				// If we're airborne, we should have less drag
				
				Vector3 movement = Input.GetAxis ("Vertical") * target.transform.forward +
				SidestepAxisInput * target.transform.right;
				
				float appliedSpeed = speed/10;
					// Scale down applied speed if in walk mode
				/*
				if (Input.GetAxis ("Vertical") < 0.0f)
				// Scale down applied speed if walking backwards
				{
					appliedSpeed /= walkSpeedDownscale;
				}*/

				//if (movement.magnitude > inputThreshold)
				// Only apply movement if we have sufficient input
				//{
					target.AddForce (movement.normalized * appliedSpeed, ForceMode.VelocityChange);

				//}
		}
		
		
		
	}
	/*
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		
		if (hit.moveDirection.y > 0.01)
		{
			return;
		}
		
		if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.9)
		{
			target.transform.position = new Vector3(target.transform.position.x,
													target.transform.position.y - hit.moveDirection.y,
													target.transform.position.z);
		}
	}*/
	
	void OnDrawGizmos ()
	// Use gizmos to gain information about the state of your setup
	{
		if (!showGizmos || target == null)
		{
			return;
		}
		
		Gizmos.color = grounded ? Color.blue : Color.red;
		Gizmos.DrawLine (target.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.position + target.transform.up * -(groundedCheckOffset + groundedDistance));
	}
}
