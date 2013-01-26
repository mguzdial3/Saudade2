using UnityEngine;
using System.Collections;

public class OnTriggerRaycast : MonoBehaviour {
	private Color toSet;
	private bool activated;
	private float activatedTimer, amountOfGlow;
	//Out of 1
	public float weight;
	private Vector3 prevPos;
	private bool setPos;
	private float currSpeed;
	
	Light light;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(activated){
			if(activatedTimer>0){
				//renderer.material.color = Color.Lerp(renderer.material.color, new Color(Random.Range(0.9f,1.0f), Random.Range(0.9f, 1.0f), Random(0.9,1.0f), 1.0f), Time.deltaTime*amountOfGlow);
				
				activatedTimer-=Time.deltaTime;
			}
			else{
				activated=false;
			}
		}
		
		
		if(light!=null){
			if(light.intensity>0){
			light.intensity-=Time.deltaTime;
			}
			else{
				Destroy(light.gameObject);
			}
		}
		
		if(setPos){
			prevPos=transform.position;
		}
		else{
			currSpeed = (transform.position-prevPos).magnitude;
		}
		
		
		setPos=!setPos;
	}
	
	public void Viewable(float timer ){
		activated=true;
		activatedTimer= timer;
		amountOfGlow = timer;
		
	}
	
	virtual public void OnTriggerEnter(Collider other){
		
		if(light==null && other.tag!="Ground"){
		GameObject test = new GameObject("Obj for: "+name);
		
		test.AddComponent<Light>();
		
		test.transform.position = other.transform.position;
		
		light = test.GetComponent<Light>();
		
		activatedTimer = 1.0f;
		}
		//print("Trigger "+other.name);
		/**
		if(hits!=null){
			print("Hits length: "+hits.Length);
			foreach(RaycastHit hit in hits){
				print("Got here");
				OnTriggerRaycast otherRaycast = hit.collider.gameObject.GetComponent("OnTriggerRaycast") as OnTriggerRaycast;
				
				otherRaycast.Viewable(1.0f);
			}
		}
		*/
	}
}
