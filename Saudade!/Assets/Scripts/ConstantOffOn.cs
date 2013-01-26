using UnityEngine;
using System.Collections;

public class ConstantOffOn : MonoBehaviour {
	public Light myLight;
	public float topBound, bottomBound;
	public float speed;
	public bool rising;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(rising){
			if(myLight.intensity<topBound){
				myLight.intensity+=Time.deltaTime*speed;
			}
			else{
				rising=!rising;
			}
		}
		else{
			if(myLight.intensity>bottomBound){
				myLight.intensity-=Time.deltaTime*speed;
			}
			else{
				rising=!rising;
			}
		}
		
	
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag=="Enemy"){
			Destroy(gameObject);
		}
	}
}
