using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Basic, WaypointFollower Type Enemy
public class WaypointFollower : MonoBehaviour{
	//List of waypoints that define the path the enemy takes when it's waypointing
	public Transform[] waypoints;
	//current Transform the enemy is heading towards
	private Transform currWaypoint;
	//Index of the waypoints array that the currWaypoint is from
	private int currWaypointIndex;
	//MovementStates! Only two, but might be more later. They just determines how the enemy moves
	private enum MovementState {WAYPOINTING, SEEKINGPLAYER};
	//The current MovementState the object is in
	private MovementState currMovementState;
	//momentary Waypoint is used when we collide with something along our path
	private GameObject momentaryWaypoint;
	//Aura amount determines how far the object can "see", redTimer determines how long the object stays red when it's hurt
	public float auraAmount = 10.0f, redTimer=0.0f;
	
	//Used for random movement
	private bool rising=true;
	private float riseAmount;
	
	//Used to calculate hit damage
	public Vector3 prevPos;
	
	//Player Reference
	public Player player;
	
	//Speed of Movement
	public float speed;
	
	//public const int UP_FORWARD = 0;
	//public const int UP_BACK = 1;
	//public const int
	
	void Start(){
		//Set up the waypoints stuff!
		if(waypoints.Length!=0){
			currWaypoint=waypoints[0];
			currWaypointIndex=0;
		}
		else{
			Transform[] _waypoints = {transform};
			waypoints = _waypoints;
			currWaypoint=waypoints[0];
			currWaypointIndex=0;
		}
		
		
		//Set current Movement State
		currMovementState=MovementState.SEEKINGPLAYER;
		
		//Find the player
		GameObject playah = GameObject.FindGameObjectWithTag("Player");
		
		player = playah.GetComponent("Player") as Player;
		
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		prevPos=transform.position;
		
		//Determine If Should Still be Red
		if(redTimer>0.0f){
			redTimer-=Time.deltaTime;
			if(redTimer<=0.0f){
				renderer.material.color=new Color(1.0f,1.0f,1.0f,1.0f);
				redTimer--;
			}
			
		}
		
		//Determine Movement State the object should be in
		DetermineState();
		
		//The existence of a momentaryWaypoint overrides all other movement
		if(momentaryWaypoint!=null){
			bool closeEnough = MoveTowards(momentaryWaypoint.transform,1.0f);
			
			//Once close enough to momentaryWaypoint destroy it
			if(closeEnough){
				Destroy(momentaryWaypoint.gameObject);
				momentaryWaypoint=null;
			}
		}
		else{
			//If waypointing, move closer to current waypoint
			if(currMovementState==MovementState.WAYPOINTING){
				
				bool closeEnough = MoveTowards(currWaypoint,1.0f);
				
				//If close enough to currWaypoint, move onto next one!
				if(closeEnough){
					if(currWaypointIndex+1>=waypoints.Length){
						currWaypoint=waypoints[0];
						currWaypointIndex=0;
					}
					else{
						currWaypoint=waypoints[currWaypointIndex+1];
						currWaypointIndex++;
					}
				}
			}
			//If seeking a player, move closer forever
			else if(currMovementState==MovementState.SEEKINGPLAYER){
				MoveTowards(player.transform, 2.0f);
				
				
			}
		}
		
		
		Hover();
	}
	
	//Returns true/false if MoveTowards is close enough to Transform
	public bool MoveTowards(Transform goal, float speedModifier){
		if(goal!=null){
			Vector3 direction = goal.position-transform.position;
			bool closeEnough =false;
			
			if(direction.magnitude<0.01f){
				closeEnough=true;
			}
			
			//Normalize so that it moves at a constant speed
			direction.Normalize();
			direction*=speed*Time.deltaTime*speedModifier;
			
			
			
			transform.position+=direction;
			
			return closeEnough;
		}
		else{
			return false;
		}
	}
	
	//Random hovering
	public virtual void Hover(){
		if(rising){
			if(riseAmount<1.0f){
				int rand = Random.Range(1,4);
				transform.position+=new Vector3(0,Time.deltaTime/rand,0);
				riseAmount+=Time.deltaTime*rand;
			}
			else{
				rising=false;
			}
		}
		else{
			if(riseAmount>-1.0f){
				int rand = Random.Range(1,4);
				transform.position-=new Vector3(0,Time.deltaTime/rand,0);
				riseAmount-=Time.deltaTime*rand;
			}
			else{
				rising=true;
			}
		}
	}
	
	//Determines state depending on whether or not it can "see" the player within it's aura
	public void DetermineState(){
		Vector3 vectorToPlayer = transform.position -player.transform.position;
		
		//If player is within aura
		if(vectorToPlayer.magnitude<auraAmount){
			
			RaycastHit hit;
			//If there's nothing blocking our "view" to the player
			if(Physics.Raycast(transform.position,player.transform.position-transform.position,out hit)){
				if(hit.collider.tag=="Player"){
					currWaypoint=hit.collider.transform;
					currMovementState=MovementState.SEEKINGPLAYER;
					//gameObject.BroadcastMessage("Activate",SendMessageOptions.DontRequireReceiver);
				}
			}
			else{
				if(currMovementState==MovementState.SEEKINGPLAYER){
					//gameObject.BroadcastMessage("Deactivate",SendMessageOptions.DontRequireReceiver);
				}
				
				currMovementState=MovementState.WAYPOINTING;
			}
		}
		else{
			if(currMovementState==MovementState.SEEKINGPLAYER){
				currWaypoint=waypoints[currWaypointIndex];
				//gameObject.BroadcastMessage("Deactivate",SendMessageOptions.DontRequireReceiver);
			}
			
			currMovementState=MovementState.WAYPOINTING;
		}
	}
	
	//Called automatically when something collides with the object
	void OnTriggerEnter(Collider other){
		
		//If the thing that hit us was the player
		if(other.tag=="Player"){
			Vector3 pushPlayer = transform.position-prevPos;
			pushPlayer.y=0;
			
			//Hurt the player depending on how fast you've been going
			//playerFields.hitpoints-=pushPlayer.magnitude*20*speed;
			
			//Shove the player based on our weight and speed
			Vector3 newPlayerPos = other.transform.position;
			newPlayerPos-=pushPlayer*2*(speed*10);
			newPlayerPos.y=other.transform.position.y;
			other.transform.position=newPlayerPos;
			
			//Back up to ram once more!
			momentaryWaypoint= new GameObject("Momentary Waypoint");
			
			momentaryWaypoint.transform.position=transform.position+20*(prevPos-transform.position);
			
		}
		else{
			
			Vector3 differenceVector = transform.position-prevPos;
			
			//Determine if we were moving fast enough that we should be hurt by the collision
			if(differenceVector.magnitude>1){
				
				//ApplyDamage(5*differenceVector.magnitude);
			}
			
			//Decide how to get around object
			Vector3 wayPointPosition=determineWayAroundObject(other.transform);
			
			Destroy(momentaryWaypoint);
			momentaryWaypoint = new GameObject("Momentary Waypoint");
			
			momentaryWaypoint.transform.position=wayPointPosition;
			
		}
		
	}
	
	
	
	//Complicated and silly way to determine way around another object based on transform alone
	public Vector3 determineWayAroundObject(Transform other){
		//If above a certain point
		Vector3 right = new Vector3(other.localScale.x/2+transform.localScale.x/2,0,0);
		Vector3 left = new Vector3(other.localScale.x/2 + transform.localScale.x/2,0,0);
		Vector3 up = new Vector3(0,other.localScale.y/2 + transform.localScale.y/2,0);
		Vector3 down = new Vector3(0,other.localScale.y/2 + transform.localScale.y/2,0);
		Vector3 front = new Vector3(0,0,other.localScale.z/2 + transform.localScale.z/2);
		Vector3 back = new Vector3(0,0,other.localScale.z/2 + transform.localScale.z/2);
		
		
		
		right = rotateAround(right,other.eulerAngles.y);
		left = rotateAround(left,other.eulerAngles.y);
		front = rotateAround(front, other.eulerAngles.z);
		back = rotateAround(back,other.eulerAngles.y);
	
	
		
		right=other.transform.position+right;
		left=other.transform.position-left;
		up=other.transform.position+up;
		down=other.transform.position-down;
		front=other.transform.position+front;
		back=other.transform.position-back;
	
		
		ArrayList faces = new ArrayList();
		
		faces.Add(right);
		faces.Add(left);
		faces.Add(up);
		faces.Add(down);
		faces.Add(front);
		faces.Add(back);
		
		float min = 1000.0f;
		Vector3 largest = new Vector3();
		
		
		
		Vector3 vectorToUse = new Vector3();
		int index = 6;
		while(vectorToUse.magnitude==0 && index>0){
		
			min = 1000.0f;
			largest = new Vector3();
			
			foreach(Vector3 face in faces){
				if((transform.position-face).magnitude<min){
					min = (transform.position-face).magnitude;
					largest=face;
				}
			}
			
			RaycastHit hit;
			
			
			Vector3 potentialVectorToUse = transform.position+((largest-transform.position).magnitude)*((largest-other.position)/((largest-other.position).magnitude));
			
			
			if (Physics.Raycast(potentialVectorToUse, (currWaypoint.position-potentialVectorToUse)/ ((currWaypoint.position-potentialVectorToUse).magnitude), out hit,(float)((currWaypoint.position-potentialVectorToUse).magnitude))
				&& hit.transform!=transform){
            	
				//if (!Physics.Raycast(transform.position, (potentialVectorToUse-transform.position)/ ((potentialVectorToUse-transform.position).magnitude), out hit2,(float)((potentialVectorToUse-transform.position).magnitude))){
			//	print("I hit a: "+hit.collider.name);	
				//}
        
    		}
			else{
				vectorToUse=potentialVectorToUse;
			}
			
			
			faces.Remove(largest);
			index--;
		
		}
		
		if(index==0){
			vectorToUse=currWaypoint.position-transform.position;
			vectorToUse.y=0;
			vectorToUse += new Vector3(Random.Range(-2.0f,2.0f),Random.Range(-2.0f,2.0f),Random.Range(-2.0f,2.0f));
			vectorToUse+=transform.position;
		}
		
		return vectorToUse;
		
	}
	
	//For use in above method
	Vector3 rotateAround(Vector3 vec, float angle){
		float x = vec.x;
		float y = vec.z;
		
		vec.x= (x*Mathf.Cos((angle/180.0f)*Mathf.PI))-(y*Mathf.Sin((angle/180.0f)*Mathf.PI));
		vec.z= (y*Mathf.Cos((angle/180.0f)*Mathf.PI))+(x*Mathf.Sin((angle/180.0f)*Mathf.PI));
		return vec;
	}
	
	
	
	
	
}
