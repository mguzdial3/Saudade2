using UnityEngine;
using System.Collections;

//Character motion for player, don't touch
public class CharacterMotion : MonoBehaviour {
	public float speed=10f, jumpTimer=0.0f;
	public float gravitySpeed=0.0f, jumpSpeed=5.0f, transferHeight;
	public enum MovementState{NORMAL, CROUCHING, SPRINTING, JUMPING};
	public bool jumping=false, transferring, airborne;
	public MovementState currMovementState, prevMovementState;
	private Vector3 prevPos;
	public GameObject look;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		prevPos=transform.position;	
	
		ModeSwitch();
		
		//Transfer from normal height to crouching and back
		if(transferring){
			if(Mathf.Abs(transferHeight-transform.position.y)>0.1f){
				transform.position=Vector3.Lerp(transform.position,new Vector3(transform.position.x, transferHeight,transform.position.z),Time.deltaTime*speed);
				
				
				
			}
			else{
				transferring=false;
			}
		}
		
		CheckGrounded();
		
		
		MovementHandler();
		if(jumping){
			JumpHandler();
		}
		
	}
	
	
	private void ModeSwitch(){
		//JUMPING
		if(Input.GetAxis("Jump")>0 && jumpTimer<=0.0f && !jumping ){
			//prevMovementState=currMovementState;
			//currMovementState=MovementState.JUMPING;
			jumpTimer=0.2f;
			jumping=true;
			
			prevMovementState=currMovementState;
			currMovementState=MovementState.JUMPING;
		}
		else if(jumpTimer>0){
			jumpTimer-=Time.deltaTime;
		}
		
		//Sprinting
		
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && currMovementState==MovementState.NORMAL){
			if(currMovementState!=MovementState.SPRINTING){
				prevMovementState=currMovementState;
			}
			currMovementState=MovementState.SPRINTING;
			//gun.active=false;
			//gun.renderer.enabled=false;
		}
		else{
			if(currMovementState==MovementState.SPRINTING){
				currMovementState=prevMovementState;
			}
			/**
			if(gun!=null){
				gun.active=true;
				gun.renderer.enabled=true;
			}
			*/
		}
		
		//Crouching
		if(Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl)){
			float crouchAmount = 1.0f;
			
			if(currMovementState==MovementState.NORMAL){
				prevMovementState=currMovementState;
				currMovementState=MovementState.CROUCHING;
				transferring=true;
				
				//lower height slightly
				transferHeight = transform.position.y;
				transferHeight-=crouchAmount;
				
				
				//Change the size so you don't slip through the ground
				
			}
			else if(currMovementState==MovementState.CROUCHING){
				prevMovementState=currMovementState;
				currMovementState=MovementState.NORMAL;
				transferring=true;
				
				//raise height slightly
				transferHeight = transform.position.y;
				transferHeight+=crouchAmount;
				
			}
		}
	}
	
	public void MovementHandler(){
		if(currMovementState==MovementState.NORMAL){
			moveYourself(1.0f);
		}
		else if(currMovementState==MovementState.JUMPING){
			moveYourself(0.75f);
		}
		else if(currMovementState==MovementState.CROUCHING){
			moveYourself(0.1f);
		}
		else if(currMovementState==MovementState.SPRINTING){
			moveYourself(1.75f);
		}
			
	}
	
	//Handles jumping and gravity while in the air
	public void JumpHandler(){
		Vector3 jumpPos = transform.position;
		
		jumpPos.y+=jumpSpeed*Time.deltaTime;
		
		transform.position=jumpPos;
	}
	
	public void Gravity(){
		if(gravitySpeed<1.0f){
			gravitySpeed+=Time.deltaTime/4;
		}
		
		transform.position-= new Vector3(0,gravitySpeed,0);
		
	}
	
	//Moves the player an amount based on the passed in multiple
	private void moveYourself(float multiplier){
		Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		
			//Square magnitude is cheaper for checking if Input was pressed at all
			if(movement.sqrMagnitude!=0){
			
				Vector3 currFacing = new Vector3(look.transform.position.x,transform.position.y,look.transform.position.z);
			
				Vector3 differenceVector = currFacing-transform.position;
				
				//The change along the local z axis
				Vector3 newPos = transform.position;
				newPos += differenceVector*movement.z*Time.deltaTime*(speed)*multiplier;
				
				//The change along the local x axis
				Vector3 differenceVectorHorz = new Vector3(differenceVector.z,0,-1*differenceVector.x);
				newPos += differenceVectorHorz*movement.x*Time.deltaTime*(speed)*multiplier/2;
			
			
				transform.position=newPos;
				//Use Raycast to check for stuff around player
				//RaycastHit hit;
				/**
				if(Physics.Raycast(transform.position,newPos-transform.position,out hit, 10*((newPos-transform.position).magnitude))){
					//You can't go there! There's a thing there!
					if(hit.collider.tag!="NoCollision"){
						//print("Hit a: "+hit.collider.name);
					}
					else{
						transform.position=newPos;
					}
				}
				else{
					transform.position=newPos;
				}
				*/
				
			}
	}
	
	void CheckGrounded(){
		RaycastHit hit;
		
		if(currMovementState==MovementState.CROUCHING){
			if(Physics.Raycast(transform.position,Vector3.down,out hit,0.1f)){
				//GROUNDED
				gravitySpeed=0.0f;
				//print("Name of thing: "+hit.collider.name);
				if(hit.distance<0.1f){
					transform.position+=new Vector3(0,0.1f-hit.distance,0);
				}
			}
			else{
				Gravity();
			}
		}
		else {
			if(Physics.Raycast(transform.position,Vector3.down,out hit,transform.localScale.y)){
				//GROUNDED
				gravitySpeed=0.0f;
				//print("Name of thing: "+hit.collider.name);
				if(hit.distance<transform.localScale.y){
					transform.position+=new Vector3(0,transform.localScale.y-hit.distance,0);
				}
			}
			else{
				Gravity();
			}
		}
	}
	
	
	void OnTriggerEnter(Collider other){
		if(other.transform.position.y<transform.position.y){
			jumping=false;
			airborne=false;
			gravitySpeed=0.0f;
			
			if(currMovementState==MovementState.JUMPING){
				currMovementState=prevMovementState;
				prevMovementState=MovementState.JUMPING;
				
			}
			
			//prevMovementState=currMovementState;
			//currMovementState=MovementState.NORMAL;
		}
		else if(other.tag!="NoCollision"){
			//MAKE NOISE
			
			
			if(other.tag!="Projectile" && other.transform.eulerAngles.x==0){
				
				//Push back
				Vector3 pushBack = transform.position;
				
				pushBack-=5*(pushBack-prevPos);
				pushBack.y=transform.position.y;
				transform.position=pushBack;
			}
		}
	}
	
	/**
	void OnTriggerExit(Collider other){
		if(other.transform.position.y<transform.position.y){
			if(!jumping){
				airborne=true;
			}
		}
	}
	*/
	/**
	public void setGun(Gun gunToSet){
		if(gun!=null){
			gunToSet.transform.position=gun.transform.position;
			gunToSet.transform.localEulerAngles=gun.transform.localEulerAngles;
			
			gunToSet.transform.parent=gun.transform.parent;
			
			gun.transform.parent=null;
			Destroy(gun.gameObject);
			gun=gunToSet;
		}
		else{
			//gunToSet.transform.position=gun.transform.position;
			//gunToSet.transform.localEulerAngles=gun.transform.localEulerAngles;
			
			gunToSet.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
			gunToSet.transform.position=gunToSet.transform.parent.position+ new Vector3(0,-0.8f,1.3f);
			gun = gunToSet;
		}
		
	}
	*/
}
