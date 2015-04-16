using UnityEngine;
using System.Collections;
using ws.winx.unity;
using UnityEditor.Animations;
using ws.winx.unity.attributes;
using UnityEditor;

public class TestFunctionContainer : MonoBehaviour {

	public AnimationCurve curve;

	[UnityVariablePropertyAttribute(typeof(string))]
	public UnityVariable variable;

	[UnityVariablePropertyAttribute(typeof(Color))]
	public UnityVariable variable1;

	[UnityVariablePropertyAttribute(typeof(Vector3))]
	public UnityVariable variable2;

	[UnityVariablePropertyAttribute(typeof(Quaternion))]
	public UnityVariable variable3;

	[UnityVariablePropertyAttribute(typeof(AnimationCurve))]
	public UnityVariable variable4;

	[UnityVariablePropertyAttribute(typeof(AnimationClip))]
	public UnityVariable variable5;

	public AnimatorState state;

	public AnimationClip motion;

	public Rigidbody rigidBody;

	void OnEnable(){


	}



	 void Reset(){
		variable = UnityVariable.CreateInstanceOf (typeof(string));
		variable1 = UnityVariable.CreateInstanceOf (typeof(Color));
		variable2 = UnityVariable.CreateInstanceOf (typeof(Vector3));
		variable3 = UnityVariable.CreateInstanceOf (typeof(Quaternion));
		variable4 = UnityVariable.CreateInstanceOf (typeof(AnimationCurve));
		variable5 = UnityVariable.CreateInstanceOf (typeof(AnimationCurve));
	}

   	public void Debug(string message){
		UnityEngine.Debug.Log (message);

	}
}
