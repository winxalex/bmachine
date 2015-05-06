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

public class AnimaTestEditor : EditorWindow
{

		bool m_AutoRecord;
		float timeCurrent;
		bool bSave;
		public AnimationCurve[] curves;
		public UnityVariable[] variables;
		public Quaternion qGlobal;

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



		public static class ExpressionEx
		{
				public static Expression Assign (Expression left, Expression right)
				{
						var assign = typeof(Assigner<>).MakeGenericType (left.Type).GetMethod ("Assign");

						var assignExpr = Expression.Call (assign, left, right);
						//var assignExpr = Expression.Add(left, right, assign);
			
						return assignExpr;
				}
		
				private static class Assigner<T>
				{
						public static void Assign (ref T left, T right)
						{
								left = right;
								//return left;
						}
				}
		}

		public delegate void RefAction<T,T1> (ref T obj,T1 t);

		static Func<S, T> CreateGetter<S, T> (FieldInfo field)
		{
				string methodName = field.ReflectedType.FullName + ".get_" + field.Name;
				DynamicMethod setterMethod = new DynamicMethod (methodName, typeof(T), new Type[1] { typeof(S) }, true);
				ILGenerator gen = setterMethod.GetILGenerator ();
				if (field.IsStatic) {
						gen.Emit (OpCodes.Ldsfld, field);
				} else {
						gen.Emit (OpCodes.Ldarg_0);
						gen.Emit (OpCodes.Ldfld, field);
				}
				gen.Emit (OpCodes.Ret);
				return (Func<S, T>)setterMethod.CreateDelegate (typeof(Func<S, T>));
		}


//	static Action<S, T> CreateSetter<S,T>(FieldInfo field)
//	{
//		string methodName = field.ReflectedType.FullName+".set_"+field.Name;
//		DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[2]{typeof(S),typeof(T)},true);
//
//	
//		ILGenerator gen = setterMethod.GetILGenerator();
//		if (field.IsStatic)
//		{
//			gen.Emit(OpCodes.Ldarg_1);
//			gen.Emit(OpCodes.Stsfld, field);
//		}
//		else
//		{
//			gen.Emit(OpCodes.Ldarg_0);
//			gen.Emit(OpCodes.Ldarg_1);
//			gen.Emit(OpCodes.Stfld, field);
//		}
//		gen.Emit(OpCodes.Ret);
//		return (Action<S, T>)setterMethod.CreateDelegate(typeof(Action<S, T>));
//	}




		static RefAction<T,T1> MakeSetter<T,T1> (FieldInfo info)
		{
				var objectParameter = Expression.Parameter (typeof(T).MakeByRefType (), "source");
				var valueParameter = Expression.Parameter (typeof(T1), "value");
				var setterExpression = Expression.Lambda<RefAction<T,T1>> (
			ExpressionEx.Assign (
			objectParameter,
			//Expression.Convert(objectParameter, typeof(T)),

			Expression.Convert (valueParameter, typeof(T1))),
//			Expression.Field(
//			Expression.Convert(objectParameter, info.DeclaringType),
//			info),
//			Expression.Convert(valueParameter, info.FieldType)),
			objectParameter,
			valueParameter);
		
				var comp = setterExpression.Compile ();

				return comp;
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

				public int Getter {
						get {
								return getter;
						}
						set {
								getter = value;
						}
				}
		}

		public delegate void ByRefStructAction (ref SomeType instance,object value);
	
		private static ByRefStructAction BuildSetter (FieldInfo field)
		{
				ParameterExpression instance = Expression.Parameter (typeof(SomeType).MakeByRefType (), "instance");
				ParameterExpression value = Expression.Parameter (typeof(object), "value");
		
				Expression<ByRefStructAction> expr =
			Expression.Lambda<ByRefStructAction> (
				ExpressionEx.Assign (
				Expression.Field (instance, field),
				Expression.Convert (value, field.FieldType)),
				instance,
				value);
		
				return expr.Compile ();
		}

//	public delegate void SetterDelegate<T,K>(ref T target, K value);
////	private static Type[] ParamTypes = new Type[]
////	{
////		typeof(T).MakeByRefType(), typeof(K)
////	};
////	
//	private static SetterDelegate<T,K> CreateSetMethod<T,K>(MemberInfo memberInfo)
//	{
//		Type ParamType;
//		if (memberInfo is PropertyInfo)
//			ParamType = ((PropertyInfo)memberInfo).PropertyType;
//		else if (memberInfo is FieldInfo)
//			ParamType = ((FieldInfo)memberInfo).FieldType;
//		else
//			throw new Exception("Can only create set methods for properties and fields.");
//		
//		DynamicMethod setter = new DynamicMethod(
//			"",
//			typeof(void),
//			new Type[]
//			{
//			typeof(T).MakeByRefType(), typeof(K)
//		},
//			memberInfo.ReflectedType.Module,
//			true);
//		ILGenerator generator = setter.GetILGenerator();
//		generator.Emit(OpCodes.Ldarg_0);
//		generator.Emit(OpCodes.Ldind_Ref);
//		
//		if (memberInfo.DeclaringType.IsValueType)
//		{
//			#if UNSAFE_IL
//			generator.Emit(OpCodes.Unbox, memberInfo.DeclaringType);
//			#else
//			generator.DeclareLocal(memberInfo.DeclaringType.MakeByRefType());
//			generator.Emit(OpCodes.Unbox, memberInfo.DeclaringType);
//			generator.Emit(OpCodes.Stloc_0);
//			generator.Emit(OpCodes.Ldloc_0);
//			#endif // UNSAFE_IL
//		}
//		
//		generator.Emit(OpCodes.Ldarg_1);
//		if (ParamType.IsValueType)
//			generator.Emit(OpCodes.Unbox_Any, ParamType);
//		
//		if (memberInfo is PropertyInfo)
//			generator.Emit(OpCodes.Callvirt, ((PropertyInfo)memberInfo).GetSetMethod());
//		else if (memberInfo is FieldInfo)
//			generator.Emit(OpCodes.Stfld, (FieldInfo)memberInfo);
//		
//		if (memberInfo.DeclaringType.IsValueType)
//		{
//			#if !UNSAFE_IL
//			generator.Emit(OpCodes.Ldarg_0);
//			generator.Emit(OpCodes.Ldloc_0);
//			generator.Emit(OpCodes.Ldobj, memberInfo.DeclaringType);
//			generator.Emit(OpCodes.Box, memberInfo.DeclaringType);
//			generator.Emit(OpCodes.Stind_Ref);
//			#endif // UNSAFE_IL
//		}
//		generator.Emit(OpCodes.Ret);
//		
//		return (SetterDelegate<T,K>)setter.CreateDelegate(typeof(SetterDelegate<T,K>));
//	}

	public Delegate _setterGlobal;
	public Delegate _getterGlobal;

	Type _setterGenericType;



  

		

//	
//	Action<T> ConvertToUntyped<T>(Action<T> action){
//		return o => action((T)o);
//	}

//	MemberInfoSetterDelegate<T,K> ConvertToUntyped<T,K>(MemberInfoSetterDelegate<T,K> action,Type a,Type b){
//		//return o => action((T)o);
//		return (ref T o,K b) => action(ref (T)o,(K)b);
//	}

//	T Action<T> ReturnType(T a){
//
//		return T;
//	}








//	public static unsafe IntPtr GetAddress(object obj)
//	{
//		var typedReference = __makeref(obj);
//		//return *(IntPtr*)(&typedReference);
//		return IntPtr.Zero;
//	}


	static Expression<Func<IEnumerable<T>, T>> CreateLambda<T>()
	{
		var source = Expression.Parameter(
			typeof(IEnumerable<T>), "source");
		
		var call = Expression.Call(
			typeof(Enumerable), "Last", new Type[] { typeof(T) }, source);
		
		return Expression.Lambda<Func<IEnumerable<T>, T>> (call, source);
	}

	static LambdaExpression CreateLambda(Type type)
	{
		var source = Expression.Parameter(
			typeof(IEnumerable<>).MakeGenericType(type), "source");
		
		var call = Expression.Call(
			typeof(Enumerable), "Last", new Type[] { type }, source);
		
		return Expression.Lambda (call, source);
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

				if (GUILayout.Button ("Test")) {

						//object instance=animatedObject1.transform.rotation;
						Quaternion instance = animatedObject1.transform.rotation;
		


			MemberInfo infoX =  typeof(Transform).GetMemberFromPath ("rotation.x");
						MemberInfo infoMember = typeof(SomeTypeClass).GetField ("member");
						MemberInfo infoMemberStatic = typeof(SomeTypeClass).GetField ("memberStatic", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
						MemberInfo infoProp = typeof(SomeTypeClass).GetProperty ("Getter");
						SomeTypeClass cls = new SomeTypeClass ();





		

			//Debug.Log("Get field:"+Getter(g1,cls,0));




			var sAll= typeof(SomeTypeClass).GetSetDelegate("member");       //infoMember.GetSetMemberInfoDelegate();

			int x=234;
			ReflectionUtility.SetThruDelegate (sAll,ref cls,x);

			var s2 = infoProp.GetSetMemberInfoDelegate<SomeTypeClass,int>();
						s2 (ref cls, 101);

			var s3 = infoMemberStatic.GetSetMemberInfoDelegate<SomeTypeClass,int> ();
						s3 (ref cls, 111);

	

			var s =typeof(Transform).GetSetDelegate("rotation.x"); //infoX.GetSetMemberInfoDelegate<Quaternion,float> ();
						//s (ref instance, 1.5f);

			ReflectionUtility.SetThruDelegate (s,ref instance,5);

			var g1=infoX.GetGetMemberInfoDelegate();
			
			float quat_x;
			
			ReflectionUtility.GetThruDelegate(g1,instance,out quat_x);


						//var setter=	ReflectionUtility.GetSetMemberInfoDelegate<object,object>(info);

						//var setter = ReflectionUtility.GetSetMemberInfoDelegate<object,object> (infoMember);

						object a = cls;
						////setter(ref a,20);

						//setter(instance,0.7f);
						//Action a=CreateSetMethod(info)

						//a(ref instance,34f);
						Debug.Log("fieldValue:"+quat_x);
						Debug.Log ("s1:" + cls.member);
						Debug.Log ("s2:" + cls.Getter);
						Debug.Log ("s3:" + SomeTypeClass.memberStatic);
						Debug.Log ("Direct:" + ((Quaternion)instance).x);
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


												variableCurrent = variables [num] = UnityVariable.CreateInstanceOf (curveBindingCurrent.type);


												variableCurrent.instanceSystemObject = instance.GetType () == typeof(GameObject) ? ((GameObject)instance).GetComponent<Transform> () : instance;
									



												propertyName = curveBindingCurrent.propertyName;
												propertyName = char.ToLower (propertyName [2]) + propertyName.Remove (0, 3);

												variableCurrent.propertyName = propertyName;
												string[] propertyPath = propertyName.Split ('.');



												//propertyName=propertyPath[0];

											

												//variableCurrent.MemberInfo=GetMemberInfo(ref instance,propertyName);					
												//variableCurrent.MemberInfo=instance.GetType ().GetField (propertyName) as MemberInfo ??
												//	instance.GetType ().GetProperty (propertyName) as MemberInfo;

												if (propertyPath.Length == 2) {
														Type propertyType = variableCurrent.MemberInfo.GetUnderlyingType ();
						
														//variableCurrent.methodInfo=propertyType.GetMethod("set_"+propertyPath[1],BindingFlags.Static | BindingFlags.Public);
				
												}
										} else {

												Debug.LogWarning (curveBindingCurrent.propertyName + " not found in " + animatedObject);
										}
					                                              


										Debug.Log ("i:" + instance.name + " path " + curveBindingCurrent.path + " prop:" + curveBindingCurrent.propertyName + " t:" + curveBindingCurrent.type
										);
								}

						}


				}
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.BeginHorizontal ();
				timeCurrent = GUILayout.HorizontalSlider (timeCurrent, 0, timeTotal);

				//	
				if (curves != null && variables != null)
						for (int i = 0; i < curves.Length; i++) {
								if (curves [i] != null) {





										variables [i].Value = curves [i].Evaluate (timeCurrent);
										Debug.Log (variables [i]);
								}
						}


				EditorGUILayout.LabelField ("", timeCurrent.ToString () + " s");
				EditorGUILayout.EndHorizontal ();

				if (clip != null && animatedObject != null) {

						if (GUILayout.Button ("CLone")) {





								PropertyModification[] modifications = PrefabUtility.GetPropertyModifications (animatedObject1).Select ((itm) => itm).Where ((itm) => itm.target.GetType () != typeof(UnityEngine.Transform)).ToArray ();

								//PrefabUtility.ResetToPrefabState(animatedObject1);

								PrefabUtility.SetPropertyModifications (animatedObject1, modifications);
			



						}

		
						EditorGUILayout.LabelField ("Frame:", ((int)(timeCurrent * clip.frameRate)).ToString ());


						if (EditorGUI.EndChangeCheck () && AnimationMode.InAnimationMode ()) {

								AnimationModeUtility.ResampleAnimation (new GameObject[] {
										animatedObject,
										animatedObject1
								}, new AnimationClip[] {
										clip,
										clip1
								}, ref timeCurrent);
						}

//		Debug.Log (timeCurrent + " " + AnimationMode.InAnimationMode ());
						if (GUILayout.Button ("StartAnimationMode") && !AnimationMode.InAnimationMode ()) {
								AnimationMode.StartAnimationMode ();



								AnimationModeUtility.ResampleAnimation (new GameObject[] {
										animatedObject,
										animatedObject1
								}, new AnimationClip[] {
										clip,
										clip1
								}, ref timeCurrent);



								if (bSave)
										SetAutoRecordMode (true);


						}

						bSave = EditorGUILayout.Toggle (bSave);

						if (GUILayout.Button ("StopAnimationMode")) {
								AnimationMode.StopAnimationMode ();

								//animatedObject.transform.position=
								animatedObject1.transform.rotation = Quaternion.identity;
								animatedObject1.transform.position = Vector3.zero;


								SetAutoRecordMode (false);
						}

				}	
		}


	
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
