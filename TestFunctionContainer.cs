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

	public AnimatorState state;

	public AnimationClip motion;


	 void Reset(){


	}

   	public void Debug(string message){
		UnityEngine.Debug.Log (message);

	}
}
