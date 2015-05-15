using UnityEngine;
using System.Collections;
using ws.winx.unity;

using ws.winx.unity.attributes;


public class TestFunctionContainer : MonoBehaviour {

	public AnimationCurve curve;

	public GameObject g;

	[MinMaxRange(0f,1f)]
	public MinMaxRange range;

//	[UnityVariablePropertyAttribute(typeof(string))]
//	public UnityVariable variable;
//
//	[UnityVariablePropertyAttribute(typeof(Color))]
//	public UnityVariable variable1;
//
//
//
//	[UnityVariablePropertyAttribute(typeof(Bounds))]
//	public UnityVariable variable2;
//
//	[UnityVariablePropertyAttribute(typeof(Quaternion))]
//	public UnityVariable variable3;
//
//	[UnityVariablePropertyAttribute(typeof(AnimationCurve))]
//	public UnityVariable variable4;

//	[UnityVariablePropertyAttribute(typeof(Light))]
//	public UnityVariable variable5;


	[UnityVariablePropertyAttribute(typeof(float))]
	public UnityVariable variable6;

	[Range(1f,10f)]
	public float changer;

	public AnimationClip motion;

	public Rigidbody rigidBody;

	void OnEnable(){

		UnityEngine.Debug.Log("Enable");
	}

//	void Update(){
//
//
//	}

	 void Reset(){
//		variable = UnityVariable.CreateInstanceOf (typeof(string));
//		variable1 = UnityVariable.CreateInstanceOf (typeof(Color));
//		variable2 = UnityVariable.CreateInstanceOf (typeof(Bounds));
//		variable3 = UnityVariable.CreateInstanceOf (typeof(Quaternion));
//		variable4 = UnityVariable.CreateInstanceOf (typeof(AnimationCurve));
//		variable5 = UnityVariable.CreateInstanceOf (typeof(Light));
		variable6 = UnityVariable.CreateInstanceOf (typeof(float));
	}



   	public void Debug(string message){
		UnityEngine.Debug.Log (message);

	}
}
