using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using ws.winx.editor;
using ws.winx.unity;
using ws.winx.csharp.utilities;
using System.Reflection;
using ws.winx.csharp.extensions;
using System.Linq.Expressions;
using System.Reflection.Emit;
using UnityEditor.Animations;

public class AnimaTestEditor : EditorWindow
{

		bool m_AutoRecord;
		float timeCurrent;
		bool bSave;
		public AnimationCurve xCurve;
		public AnimationCurve[] curves;
		public UnityVariable[] variables;
		public Quaternion qGlobal;
		float timeLast = Time.realtimeSinceStartup;

		Vector3 animatedObjectPosition;

		void OnEnable ()
		{

				curves = new AnimationCurve[0];
				variables = new UnityVariable[0];

		}

		public int frameCurrent {
				get {
						return (int)(timeCurrent * clip.frameRate);
			
				}
		}

		public  float frameFloor {
				get {
						return ((float)frameCurrent - 0.5f) / clip.frameRate;
				}
		}

		public  float frameCeiling {
				get {
						return ((float)frameCurrent + 0.5f) / clip.frameRate;
				}
		}

		public  AnimationClip clip;
		public GameObject animatedObject;
		public  AnimationClip clip1;
		public GameObject animatedObject1;
		public float timeTotal = 10;
		int frameCount;

		Transform boneRoot;

		// Add menu named "My Window" to the Window menu
		[MenuItem ("Window/My Window")]
		static void Init ()
		{
				// Get existing open window or if none, make a new one:
				AnimaTestEditor window = (AnimaTestEditor)EditorWindow.GetWindow (typeof(AnimaTestEditor));
				window.Show ();
		}


		// Use this for initialization
		void Start ()
		{
	
		}

		public void SetAutoRecordMode (bool record)
		{
				if (this.m_AutoRecord != record) {
						if (record) {
								//Undo.postprocessModifications+=this.PostprocessAnimationRecordingModifications;

								Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine (Undo.postprocessModifications, new Undo.PostprocessModifications (this.PostprocessAnimationRecordingModifications));
						} else {
								Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove (Undo.postprocessModifications, new Undo.PostprocessModifications (this.PostprocessAnimationRecordingModifications));
						}
						this.m_AutoRecord = record;
//			if (this.m_AutoRecord)
//			{
//				this.EnsureAnimationMode ();
//			}
				}
		}

		private UndoPropertyModification[] PostprocessAnimationRecordingModifications (UndoPropertyModification[] modifications)
		{

				return AnimationModeUtility.Process (animatedObject, clip, modifications, timeCurrent).Concat (AnimationModeUtility.Process (animatedObject1, clip1, modifications, timeCurrent)).ToArray ();
		}




		public struct SomeType
		{
				public int member;
				public int getter;

				public int Getter {
						get {
								return getter;
						}
						set {
								getter = value;
						}
				}
		}

		public class SomeTypeClass
		{

				public static int memberStatic;
				public int member;
				public int getter;
				public static Vector3 memberStructStatic;
				public Vector3 memberStruct;

				public int Getter {
						get {
								return getter;
						}
						set {
								getter = value;
						}
				}
		}



		
		void OnGUI ()
		{
				//"transform.rotation.x" "directionLight.intesity"
				//LambdaExpression lmb;
				//lmb.Compile().Method.I
		
				animatedObject = EditorGUILayout.ObjectField (animatedObject, typeof(GameObject), true) as GameObject;
				clip = EditorGUILayout.ObjectField (clip, typeof(AnimationClip), true) as AnimationClip;

				animatedObject1 = EditorGUILayout.ObjectField (animatedObject1, typeof(GameObject), true) as GameObject;
				clip1 = EditorGUILayout.ObjectField (clip1, typeof(AnimationClip), true) as AnimationClip;

				//


			AnimatorController aniController;
		//AnimatorController.


		//CREATE NEW MOVIE CLIP



//		private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
//		{
//			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
//			AssetDatabase.AddObjectToAsset(animationClip, controller);
//			State dst = AnimatorController.AddAnimationClipToController(controller, animationClip);
//			controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
//			StateMachine stateMachine = controller.GetLayer(0).stateMachine;
//			Transition transition = stateMachine.AddAnyStateTransition(dst);
//			AnimatorCondition condition = transition.GetCondition(0);
//			condition.mode = TransitionConditionMode.If;
//			condition.parameter = name;
//			
//			return animationClip;
//		}
		
		//		public static AnimationClip AllocateAnimatorClip (string name)
//		{
//			AnimationClip animationClip = AnimationSelection.AllocateAndSetupClip (true);
//			animationClip.name = name;
//			return animationClip;
//		}

		//Create CONTROLER AT 
		//AnimatorController.CreateAnimatorControllerAtPath(text4);
		//AnimatorController.CreateAnimatorControllerAtPathWithClip (path,clip);

				if (GUILayout.Button ("Test")) {

//			Type newType=typeof(MemberDelegate<>).MakeGenericType (new Type[] {
//				typeof(int)
//			});
//
//			var dlgate = Delegate.CreateDelegate(newType,this.GetType().GetMethod("Foo"));

						//Foo1(dlgate,123);

						//object instance=animatedObject1.transform.rotation;
						Transform transformInstance = animatedObject1.transform;
						object transformObject = animatedObject1.transform;
		


//						MemberInfo infoX =  typeof(Transform).GetMemberFromPath ("rotation.x");
//						MemberInfo infoMember = typeof(SomeTypeClass).GetField ("member");
//						MemberInfo infoMemberStatic = typeof(SomeTypeClass).GetField ("memberStatic", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
//						MemberInfo infoProp = typeof(SomeTypeClass).GetProperty ("Getter");
						SomeTypeClass cls = new SomeTypeClass ();

						var s1 = cls.GetType ().GetSetDelegate ("memberStatic");
						object clsObject = cls;
						s1 (ref clsObject, 101);

						var s2 = cls.GetType ().GetSetDelegate ("memberStructStatic");
						s2 (ref clsObject, new Vector3 (3f, 33f, 333f));


						var s3 = cls.GetType ().GetSetDelegate ("memberStruct");
						s3 (ref clsObject, new Vector3 (3f, 33f, 333f));

						Quaternion q = Quaternion.identity;//Quaternion.Euler(25f,170f,14f);
						object qObject = Quaternion.identity;//Quaternion.Euler(25f,170f,14f);

						//object transformObject;//=animatedObject1.GetType().GetMember("transform")[0].GetValue(animatedObject1);
						//
						UnityEngine.Object gameObject = animatedObject1;

						//var so=CreateSetMethod(animatedObject1.GetType().GetMemberFromPath("transform.rotation"));
			
						//so(ref transformObject,new Quaternion(0f,1f,0f,0f));


//			var so=CreateSetMethod(animatedObject1.GetType().GetMemberFromPath("transform.rotation.x"));
//			
//			so(ref qObject,0.16f);




						//get component

						var transformGetterDelegate = animatedObject1.GetType ().GetGetDelegate ("transform");
//
//
//
//
//			//get prop1
						var quaternionGetDelegate = animatedObject1.GetType ().GetGetDelegate ("transform.rotation");
						var quaternionSetDelegate = animatedObject1.GetType ().GetSetDelegate ("transform.rotation");




//			var mdfdf=animatedObject1.GetType().GetMemberFromPath("transform.rotation").GetSetMemberInfoDelegate<Transform,Quaternion>();
//			mdfdf(ref transformInstance,q);


//			var mdfdf=animatedObject1.GetType().GetMemberFromPath("transform.rotation").GetSetMemberInfoDelegate<object,object>();
//			mdfdf(ref transformObject,q);

						var mdfdf = animatedObject1.GetType ().GetMemberFromPath ("transform.rotation.x").GetSetMemberInfoDelegate<object,object> ();
						mdfdf (ref qObject, 0.5f);

//			var mdfdf=animatedObject1.GetType().GetMemberFromPath("transform.rotation.x").GetSetMemberInfoDelegate<Quaternion,float>();
//			mdfdf(ref q,0.3f);
			
						//			//get prop2
						var xGetter = typeof(Transform).GetGetDelegate ("rotation.x"); 

						var xSetter = typeof(Transform).GetSetDelegate ("rotation.x"); 

						object transfromObject = transformGetterDelegate (gameObject);

//			object xOut=xGetter(quaternionGetDelegate(transformGetterDelegate(gameObject)));
//
//			object quaternionObject=quaternionGetDelegate(transformGetterDelegate(gameObject));
//
//			xSetter(ref quaternionObject,1.5f);
//
//			quaternionSetDelegate(ref transfromObject,quaternionObject);

			
		

					
						////setter(ref a,20);

						//setter(instance,0.7f);
						//Action a=CreateSetMethod(info)

						//a(ref instance,34f);
						Debug.Log ("q: " + q);
						Debug.Log ("qObject: " + qObject);
						Debug.Log ("animatedObject " + animatedObject1.transform.rotation);
//						Debug.Log ("Field 'member'" + cls.member);
//						Debug.Log ("Property 'Getter'" + cls.Getter);
						Debug.Log ("Property 'memberStruct'" + cls.memberStruct);
						Debug.Log ("Static :" + SomeTypeClass.memberStructStatic);
						Debug.Log ("Static :" + SomeTypeClass.memberStatic);
//						Debug.Log ("rotation.x set:" + animatedObject1.transform.rotation.x);

						//Debug.Log("result="+result+" "+q);
				}



				if (clip != null && animatedObject != null) {
						if (GUILayout.Button ("Save Curves")) {


								EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings (clip);

								int curveBindingLen = curveBindings.Length;
								curves = new AnimationCurve[curveBindingLen];
								variables = new UnityVariable[curveBindingLen];
								String propertyName = null;
								EditorCurveBinding curveBindingCurrent;
								UnityVariable variableCurrent;
								for (int num = 0; num < curveBindingLen; num++) {
										curveBindingCurrent = curveBindings [num];


										UnityEngine.Object instance = AnimationUtility.GetAnimatedObject (animatedObject, curveBindingCurrent);
										if (instance != null) {




												curves [num] = AnimationUtility.GetEditorCurve (clip, curveBindingCurrent);


												propertyName = curveBindingCurrent.propertyName;
												//process "m_Rotation.x" "m_Intensity" into "rotation.x", "intensity"
												propertyName = char.ToLower (propertyName [2]) + propertyName.Remove (0, 3);

												Type type = curveBindingCurrent.type.GetMemberFromPath (propertyName).GetUnderlyingType ();

												variableCurrent = variables [num] = UnityVariable.CreateInstanceOf (type);

												//ex. Transform or Light component
												var instanceToBind = instance.GetType () == typeof(GameObject) ? ((GameObject)instance).transform : instance;
			

												variableCurrent.Bind (instanceToBind, propertyName);
											
										} else {

												Debug.LogWarning (curveBindingCurrent.propertyName + " not found in " + animatedObject);
										}
					                                              


										Debug.Log ("i:" + instance.name + " path " + curveBindingCurrent.path + " prop:" + curveBindingCurrent.propertyName + " t:" + curveBindingCurrent.type
										);
								}

						}


				}
				EditorGUI.BeginChangeCheck ();
				
				timeCurrent = GUILayout.HorizontalSlider (timeCurrent, 0, timeTotal);
				EditorGUILayout.BeginHorizontal ();
				int i;
				if (curves != null && variables != null) {
						EditorGUILayout.BeginVertical ();
						for (i = 0; i < curves.Length; i++) 
				if(curves[i]!=null)
								EditorGUILayout.CurveField (variables [i].memberPath, curves [i]);


						EditorGUILayout.EndVertical ();
				}
		


				//Debug.Log (Time.renderedFrameCount+" "+Application.targetFrameRate);


				if (Time.realtimeSinceStartup - timeLast > 0.016) {
						frameCount++;
						timeLast = Time.realtimeSinceStartup;
						if (curves != null && variables != null && curves.Length > 0) {
//						for (i = 0; i < curves.Length; i++) {
//								if (curves [i] != null) {


								Quaternion quatNew = new Quaternion (curves [0].Evaluate (timeCurrent), curves [1].Evaluate (timeCurrent), curves [2].Evaluate (timeCurrent), curves [3].Evaluate (timeCurrent));

								//animatedObject.transform.GetChild(0).GetChild(0).transform.localRotation=quatNew;

								Debug.Log ("quatNew " + quatNew);

								variables [0].Value = curves [0].Evaluate (timeCurrent);
								variables [1].Value = curves [1].Evaluate (timeCurrent);
								variables [2].Value = curves [2].Evaluate (timeCurrent);
								variables [3].Value = curves [3].Evaluate (timeCurrent);

								Debug.LogFormat ("{0} {1} {2} {3}", variables [0].Value, variables [1].Value, variables [2].Value, variables [3].Value);
								Debug.Log ("status:" + animatedObject.transform.GetChild (0).GetChild (0).transform.localRotation);

								Debug.Log ("======== end ----------");


								//animatedObject.transform.GetChild(0).GetChild(0).transform.localRotation=
										

								//variables [i].Value = curves [i].Evaluate (timeCurrent);
								//if(i>0){
								//variables[i].Value=curves [i].Evaluate (timeCurrent);
								
								//Debug.Log (variables [i]+" FRM:"+frameCount);
								//}
								//}
								//}

						}
				}


				EditorGUILayout.LabelField ("", timeCurrent.ToString () + " s");
				EditorGUILayout.EndHorizontal ();

				if (clip != null && animatedObject != null) {

						if (GUILayout.Button ("Reset Pose")) {



								//Vector3 positionSaved=animatedObject1.transform.position;
								UnityEditorInternal.ComponentUtility.CopyComponent (animatedObject1.transform);
								

								PropertyModification[] modifications = PrefabUtility.GetPropertyModifications (animatedObject1).Select ((itm) => itm).Where ((itm) => itm.target.GetType () != typeof(UnityEngine.Transform)).ToArray ();

								//PrefabUtility.ResetToPrefabState(animatedObject1);

								PrefabUtility.SetPropertyModifications (animatedObject1, modifications);
			
								//animatedObject1.transform.position=positionSaved;
								UnityEditorInternal.ComponentUtility.PasteComponentValues (animatedObject1.transform);


						}

		
						EditorGUILayout.LabelField ("Frame:", ((int)(timeCurrent * clip.frameRate)).ToString ());


						if (EditorGUI.EndChangeCheck () && AnimationMode.InAnimationMode ()) {

								AnimationModeUtility.SampleClipBindingAt (new GameObject[] {
										animatedObject,
										animatedObject1
								}, new AnimationClip[] {
										clip,
										clip1
								}, ref timeCurrent);




				boneRoot.transform.position= animatedObjectPosition +boneRoot.transform.position;
				Debug.Log ("pos z:"+animatedObject1.transform.GetChild(2).GetChild(0).transform.position.z+" lpos z:"+animatedObject1.transform.GetChild(2).GetChild(0).transform.localPosition.z);
				//animatedObject1.transform.GetChild(2).GetChild(0).transform.position=animatedObject1.transform.GetChild(2).GetChild(0).transform.position-animatedObjectPosition;
								
						}



			bSave = EditorGUILayout.Toggle ("Save:",bSave);

//		Debug.Log (timeCurrent + " " + AnimationMode.InAnimationMode ());
						if (GUILayout.Button ("StartAnimationMode") && !AnimationMode.InAnimationMode ()) {
								AnimationMode.StartAnimationMode ();

				//if(animatedObjectPosition==null)
								

						boneRoot=animatedObject1.GetRootBone();

				var bonePositionPrev=boneRoot.transform.position;
				//animatedObjectPosition=-boneRoot.transform.position;
								AnimationModeUtility.SampleClipBindingAt (new GameObject[] {
										animatedObject,
										animatedObject1
								}, new AnimationClip[] {
										clip,
										clip1
								}, ref timeCurrent);




				animatedObjectPosition=bonePositionPrev-boneRoot.transform.position;

				
				Debug.Log("start: diff="+animatedObjectPosition+" GO:"+animatedObject1.transform.position+" "+boneRoot.transform.position);



				boneRoot.transform.position= animatedObjectPosition +boneRoot.transform.position;

				Debug.Log("at star bone should be same as animatedObject1 BONE:"+boneRoot.transform.position+" GO:"+animatedObject1.transform.position);




						if (bSave)

										SetAutoRecordMode (true);




						}

						

						if (GUILayout.Button ("StopAnimationMode")) {
								AnimationMode.StopAnimationMode ();

								

								bSave=false;
								SetAutoRecordMode (false);
						}

				}	
		}


	
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
