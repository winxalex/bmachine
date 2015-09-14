using UnityEngine;
using System.Collections;

public class RoseComponent : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}


	public void GrabTheFlower(Transform parent){

		this.transform.parent = parent;

		this.transform.rotation=Quaternion.Euler(new Vector3(90,0, 0));
		this.transform.localPosition = Vector3.zero;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
