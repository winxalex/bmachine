using UnityEngine;
using System.Collections;
using ws.winx.unity;

public class UnityChenFaceLayerWeigthAnimatedValues : MonoBehaviour,IAnimatedValues {

	public Animator animator;


	public float FaceLayerWeight;
	public int FaceLayerIndex;

	#region IAnimatedValues implementation
	public void UpdateValues ()
	{
		animator.SetLayerWeight (FaceLayerIndex, FaceLayerWeight);

		//Debug.Log ("Layer " + FaceLayerIndex + " weight:" + animator.GetLayerWeight(FaceLayerIndex));
	}
	public void ResetValues ()
	{
		FaceLayerWeight = 0f;
		animator.SetLayerWeight (FaceLayerIndex, FaceLayerWeight);
	}
	#endregion

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
