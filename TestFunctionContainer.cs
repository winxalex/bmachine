using UnityEngine;
using System.Collections;
using ws.winx.unity;

public class TestFunctionContainer : MonoBehaviour {

	public AnimationCurve curve;

	public UnityVariable variable;


	 void Reset(){
		if(variable==null)
		variable=(UnityVariable)ScriptableObject.CreateInstance<UnityVariable>();

		if (curve == null)
						curve = new AnimationCurve ();
	}

   	public void Debug(string message){
		UnityEngine.Debug.Log (message);

	}
}
