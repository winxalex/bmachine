using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public Transform bone;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		bone.GetComponent<Rigidbody>().AddForce (-transform.forward * 500,ForceMode.Force);
	}
}
