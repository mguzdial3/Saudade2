using UnityEngine;
using System.Collections;

//First person perspective for player, Don't Touch
public class FirstPersonPerspective : MonoBehaviour {
	public Vector2 prevMouse, displayMouse;
	public float xMin, xMax;
	public float yMin, yMax;
	private float yMinRotation=330, yMaxRotation=40;
	private float xMouseChange, yMouseChange;
	private float xSpeed = 10.0f, ySpeed=10.0f;
	
	// Use this for initialization
	void Start () {
		prevMouse= Input.mousePosition;
		
		if(transform.parent.rigidbody!=null){
			transform.parent.rigidbody.freezeRotation=true;
		}
		
		xMin = Screen.width/3;
		xMax= Screen.width-Screen.width/3;
		
		yMin= Screen.height/3;
		yMax=Screen.height-Screen.height/3;
		
		displayMouse = new Vector2(Screen.width/2,Screen.height/2);
		
		Screen.showCursor=false;
		Screen.lockCursor=true;
	}
	
	// Update is called once per frame
	void Update () {
		//Hide mouse
		
		xMouseChange = Input.GetAxis("Mouse X")*xSpeed;
		yMouseChange = Input.GetAxis("Mouse Y")*ySpeed;
		
		//displayMouse.x+=xMouseChange;
		//displayMouse.y-=yMouseChange;
		
		//displayMouse.x = Mathf.Clamp(displayMouse.x,xMin-1,xMax+1);
		//displayMouse.y = Mathf.Clamp(displayMouse.y,yMin-1,yMax+1);
		
		
		//Further left on screen
		if(xMouseChange<-1.0f){
		
			Vector3 currRotation = transform.localEulerAngles;
					
			currRotation.y-=Time.deltaTime*120;
					
			transform.localEulerAngles=currRotation;
			
			//displayMouse=prevMouse;
		}
		else if(xMouseChange>1.0f){
			
			Vector3 currRotation = transform.localEulerAngles;
					
			currRotation.y+=Time.deltaTime*120;
					
			transform.localEulerAngles=currRotation;
		
			//displayMouse=prevMouse;
		}
		//print("YMOUSE CHANGE: "+yMouseChange);
		//Lower on the screen (Look Down)
		if(yMouseChange>2.0f && (transform.localEulerAngles.x>yMinRotation || transform.localEulerAngles.x<200)){
		//if(displayMouse.y<yMin && (transform.localEulerAngles.x>yMinRotation || transform.localEulerAngles.x<200)){
			
			Vector3 currRotation = transform.localEulerAngles;
					
			currRotation.x-=Time.deltaTime*120;
				
			transform.localEulerAngles=currRotation;
			
			//displayMouse=prevMouse;
		}
		//Higher on screen
		else if(yMouseChange<-2.0f && (transform.localEulerAngles.x<yMaxRotation || transform.localEulerAngles.x>300)){
		//else if(displayMouse.y>yMax && (transform.localEulerAngles.x<yMaxRotation || transform.localEulerAngles.x>300)){
		
			Vector3 currRotation = transform.localEulerAngles;
					
			currRotation.x+=Time.deltaTime*120;
					
			transform.localEulerAngles=currRotation;
			
			//displayMouse=prevMouse;
		}
		
		prevMouse=displayMouse;
		
		//Allows one to actually stop playing
		if(Input.GetKey(KeyCode.Escape)){
			Screen.lockCursor=false;
			Screen.showCursor=true;
		}
		
	}
	
	
	void OnGUI(){
		GUI.Box(new Rect(displayMouse.x,displayMouse.y,5,5),"boop");
	}
}
