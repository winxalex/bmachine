//----------------------------------------------
//            Behaviour Machine
// Copyright © 2014 Anderson Campos Cardoso
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using BehaviourMachine;

using ws.winx.bmachine;
using BehaviourMachineEditor;
using UnityEditorInternal;
using ws.winx.unity;
using System;
using System.Collections.Generic;
using ws.winx.editor.extensions;
using System.Linq;
using ws.winx.csharp.utilities;
using System.Runtime.Serialization;
using System.Reflection;
using ws.winx.csharp.extensions;
using ws.winx.editor.windows;

namespace ws.winx.editor.bmachine
{

		/// <summary>
		/// Wrapper class for BehaviourTreeEditor.
		/// <seealso cref="BehaviourMachine.BehaviourTree" />
		/// </summary>
		[CustomEditor(typeof(BlackboardCustom))]
		public class BlackboardCustomEditor :Editor
		{
				public delegate Rect SwitchDrawerDelegate (Rect rect,UnityVariable variable);

				ReorderableList  __variablesReordableList;
				GenericMenu genericMenu;
				Func<Type,SwitchDrawerDelegate>  switchDrawer;
				Action<Type> switchMenuTypes;
				string _typeNameSelected = "None";
				SerializedProperty typesCustomSerialized;
			
				//into static editor Utilityor somthing
				static List<Func<Type,SwitchDrawerDelegate>> _drawers;
				static Func<Type,SwitchDrawerDelegate> _defaultSwitchDrawer;

				
				public static void AddCustomDrawer(SwitchDrawerDelegate drawer){
					 _drawers.Add(SwitchUtility.CaseIsClassOf<float,SwitchDrawerDelegate> (drawer));
				}		
				
				public static Func<Type, SwitchDrawerDelegate> GetDefaultSwitchDrawer ()
				{
					if (_defaultSwitchDrawer == null) {
						_drawers=new List<Func<Type,SwitchDrawerDelegate>>(
							
							new Func<Type, SwitchDrawerDelegate>[]{
							
							SwitchUtility.CaseIsClassOf<float,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawFloatVar),
							SwitchUtility.CaseIsClassOf<int,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawIntVar),
							SwitchUtility.CaseIsClassOf<bool,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawBoolVar),
							SwitchUtility.CaseIsClassOf<string,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawStringVar),
							SwitchUtility.CaseIsClassOf<Quaternion,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawQuaternionVar),
							SwitchUtility.CaseIsClassOf<Vector3,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawVector3Var),
							SwitchUtility.CaseIsClassOf<Rect,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawRectVar),
							SwitchUtility.CaseIsClassOf<Color,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawColorVar),
							SwitchUtility.CaseIsClassOf<Material,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
							SwitchUtility.CaseIsClassOf<UnityEngine.Texture,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
							SwitchUtility.CaseIsClassOf<GameObject,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
						}
						);
						
						
						_defaultSwitchDrawer=SwitchUtility.Switch (_drawers);
						
						
					}
					
					
					return _defaultSwitchDrawer;
				}

			
				

				private void OnEnable ()
				{


						//serializedObject.FindProperty("variablesList").objectReferenceValue
						if (__variablesReordableList == null) {

		

								SerializedProperty variableListSerialized = serializedObject.FindProperty ("variablesList");


								typesCustomSerialized = serializedObject.FindProperty ("typesCustom");
				
				
								__variablesReordableList = new ReorderableList (serializedObject, variableListSerialized, 
				                                       true, true, true, true);
								__variablesReordableList.drawElementCallback = onDrawElement;

								__variablesReordableList.onAddDropdownCallback = onAddDropdownCallback;

								__variablesReordableList.drawHeaderCallback = onDrawHeaderElement;

								__variablesReordableList.onRemoveCallback = onRmoveCallback;

				__variablesReordableList.onSelectCallback=onSelectCallback;
				

								genericMenu = EditorGUILayoutEx.GeneraterGenericMenu<Type> (EditorGUILayoutEx.unityTypesDisplayOptions, EditorGUILayoutEx.unityTypes, onTypeSelection);

								fillMenuCustomTypes ();


			//	SwitchUtility.AddDrawer(

					switchDrawer = GetDefaultSwitchDrawer();



												switchMenuTypes = SwitchUtility.SwitchExecute (
					new Func<Type, Action>[]{

					SwitchUtility.CaseIsClassOf<float,Action> (() => {
										AddVariableToList<float> ("New Float", 0f, __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<int,Action> (() => {
										AddVariableToList<int> ("New Int", 0, __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<string,Action> (() => {
										AddVariableToList<String> ("New String", String.Empty, __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<bool,Action> (() => {
										AddVariableToList<bool> ("New Bool", false, __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Vector3,Action> (() => {
										AddVariableToList<Vector3> ("New Vector3", new Vector3 (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Quaternion,Action> (() => {
										AddVariableToList<Quaternion> ("New Quaternion", new Quaternion (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Color,Action> (() => {
										AddVariableToList<Color> ("New Color", new Color (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Rect,Action> (() => {
										AddVariableToList<Rect> ("New Rect", new Rect (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<UnityEngine.Texture2D,Action> (() => {
										AddVariableToList<UnityEngine.Texture2D> ("New Texture", new UnityEngine.Texture2D (2, 2), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<UnityEngine.Texture3D,Action> (() => {
										AddVariableToList<UnityEngine.Texture3D> ("New Texture", new UnityEngine.Texture3D (2, 2, 2, TextureFormat.ARGB32, false), __variablesReordableList);}),

					SwitchUtility.CaseIsClassOf<Material,Action> (() => {
										AddVariableToList<Material> ("Material", new Material (Shader.Find ("Diffuse")), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<GameObject,Action> (() => {
										AddVariableToList<GameObject> ("New GameObject", new GameObject (), __variablesReordableList);})

						}
								);


						}

				}

				void fillMenuCustomTypes ()
				{
						Type[] derivedTypes = TypeUtility.GetDerivedTypes (typeof(System.Object));
						for (int i = 0; i < derivedTypes.Length; i++) {
				
				
								string text2 = derivedTypes [i].ToString ();
								genericMenu.AddItem (new GUIContent ("Custom Object/" + text2.Replace ('.', '/')), _typeNameSelected == text2, onTypeNewSelected, derivedTypes [i]);
						}

						genericMenu.AddSeparator (string.Empty);
						genericMenu.AddItem (new GUIContent ("Reload"), false, delegate {
								fillMenuCustomTypes ();
						});
			
			
						genericMenu.AddSeparator (string.Empty);
						genericMenu.AddItem (new GUIContent ("Global Blackboard"), false, delegate {
								EditorApplication.ExecuteMenuItem ("Tools/BehaviourMachine/Global Blackboard");
						});
				}
		
				void AddVariableToList<T> (string name, T value, ReorderableList list)
				{
						AddVariableToList1(name, value, list);
				}

				void AddVariableToList1(string name, object value, ReorderableList list)
				{

		

						var index = list.serializedProperty.arraySize;
						list.serializedProperty.arraySize++;
						list.index = index;
			
						UnityVariable variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
						variable.name = name;
						
						variable.Value = value;
			
						var element = list.serializedProperty.GetArrayElementAtIndex (index);
						element.objectReferenceValue = variable;
			
						serializedObject.ApplyModifiedProperties ();
				}

				void onTypeSelection (object userData)
				{

						switchMenuTypes ((Type)userData);
		

				}

		void onSelectCallback (ReorderableList list)
		{
			UnityVariable variable = list.serializedProperty.GetArrayElementAtIndex (list.index).objectReferenceValue as UnityVariable;

			if (variable != null) {
				UnityObjectEditorWindow.Show(variable);


			}
		}

				void onRmoveCallback (ReorderableList list)
				{
						if (EditorUtility.DisplayDialog ("Warning!", 
			                                "Are you sure you want to delete the Unity Variable?", "Yes", "No")) {
								ReorderableList.defaultBehaviours.DoRemoveButton (list);
								list.serializedProperty.arraySize--;//DON"T KNOW why this is nessery might be bug
								//ReorderableList.defaultBehaviours.DoRemoveButton (list); 
						}
				}

				void onAddDropdownCallback (Rect buttonRect, ReorderableList list)
				{

						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						

						genericMenu.ShowAsContext ();
		
				}
		
				void onDrawElement (Rect rect, int index, bool isActive, bool isFocused)
				{

						SerializedProperty property = __variablesReordableList.serializedProperty.GetArrayElementAtIndex (index); 

						if (property == null || property.objectReferenceValue == null) {
								return;
						}

						rect=DrawVariables (rect, property);



						
						//__variablesReordableList.elementHeight = rect.height;

				}

				void onDrawHeaderElement (Rect rect)
				{
						EditorGUI.LabelField (rect, "Blackboard Variables:");
				}

				public Rect DrawVariables (Rect position, SerializedProperty property)
				{
						//bool isAsset = AssetDatabase.Contains (blackboard.get_gameObject ());
					
						Type variableType;
						UnityVariable currentVariable;
						SwitchDrawerDelegate switchDrawerDelegate;

		
				

						currentVariable = (UnityVariable)property.objectReferenceValue;

						switchDrawerDelegate = switchDrawer (currentVariable.ValueType);
				
						if (switchDrawerDelegate != null) {
								//make drawing of the variable
								return switchDrawerDelegate (position, currentVariable);
						} else {
								EditorGUILayoutEx.DrawName(position,currentVariable);



			
						}



		
						return position;
		
				}

				void onTypeNewSelected (object userData)
				{

						//or use
						//Activator.CreateInstance ((Type)userData);

						AddVariableToList ("New " + userData.ToString (), FormatterServices.GetUninitializedObject ((Type)userData), __variablesReordableList);


				}

				public override void OnInspectorGUI ()
				{

						base.DrawDefaultInspector ();

						serializedObject.Update ();

						__variablesReordableList.DoLayoutList ();

						serializedObject.ApplyModifiedProperties ();
				}
		}
}