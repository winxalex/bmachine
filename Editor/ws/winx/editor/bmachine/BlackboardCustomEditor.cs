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

namespace ws.winx.editor.bmachine
{

		/// <summary>
		/// Wrapper class for BehaviourTreeEditor.
		/// <seealso cref="BehaviourMachine.BehaviourTree" />
		/// </summary>
		[CustomEditor(typeof(BlackboardCustom))]
		public class BlackboardCustomEditor :Editor
		{

				ReorderableList  __variablesReordableList;
				GenericMenu genericMenu;
				Func<Type,SwitchDrawerDelegate>  switchDrawer;
				Action<Type> switchMenuTypes;

			
				private delegate void SwitchDrawerDelegate (Rect rect,UnityVariable variable);

				private void OnEnable ()
				{


						//serializedObject.FindProperty("variablesList").objectReferenceValue
						if (__variablesReordableList == null) {

								SerializedProperty variableListSerialized = serializedObject.FindProperty ("variablesList");





								__variablesReordableList = new ReorderableList (serializedObject, variableListSerialized, 
				                                       true, true, true, true);
								__variablesReordableList.drawElementCallback = onDrawElement;

								__variablesReordableList.onAddDropdownCallback = onAddDropdownCallback;

								__variablesReordableList.drawHeaderCallback = onDrawHeaderElement;
				

								genericMenu = EditorGUILayoutEx.GeneraterGenericMenu<Type> (EditorGUILayoutEx.unityTypesDisplayOptions, EditorGUILayoutEx.unityTypes, onMenuSelection);


								switchDrawer = SwitchUtility.Switch (
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
					SwitchUtility.CaseIsClassOf<Texture,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
					SwitchUtility.CaseIsClassOf<GameObject,SwitchDrawerDelegate> (EditorGUILayoutEx.DrawUnityObject),
				});



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
					SwitchUtility.CaseIsClassOf<Vector2,Action> (() => {
										AddVariableToList<Vector3> ("New Vector3", new Vector3 (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Quaternion,Action> (() => {
										AddVariableToList<Quaternion> ("New Quaternion", new Quaternion (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Color,Action> (() => {
										AddVariableToList<Color> ("New Color", new Color (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Rect,Action> (() => {
										AddVariableToList<Rect> ("New Rect", new Rect (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Texture,Action> (() => {
										AddVariableToList<Texture> ("New Texture", new Texture (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<Material,Action> (() => {
						AddVariableToList<Material> ("Material", new Material (Shader.Find ("Diffuse")), __variablesReordableList);}),

					SwitchUtility.CaseIsClassOf<GameObject,Action> (() => {
										AddVariableToList<GameObject> ("New GameObject", new GameObject (), __variablesReordableList);})

						}
								);


						}

				}

				void AddVariableToList<T> (string name, T value, ReorderableList list)//,object referenceInstance
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

				void onMenuSelection (object userData)
				{

						//this.GetType().MakeGenericType(new Type[]{type})
						//AddVariableToList("New "+type.ToString(),default(type),__variablesReordableList);


		

				}



//					Assembly GetAssemblyByName(string name)
//					{
//						return AppDomain.CurrentDomain.GetAssemblies().
//							SingleOrDefault(assembly => assembly.GetName().Name == name);
//					}

				void onAddDropdownCallback (Rect buttonRect, ReorderableList list)
				{




						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						

						
						genericMenu.AddItem (new GUIContent ("Custom Object"), false, delegate {

						});
						genericMenu.AddItem (new GUIContent ("FsmEvent"), false, delegate {
								//maybe we go with UnityEvent

								//new FsmEvent
						});

						genericMenu.AddSeparator (string.Empty);
						genericMenu.AddItem (new GUIContent ("Global Blackboard"), false, delegate {
								EditorApplication.ExecuteMenuItem ("Tools/BehaviourMachine/Global Blackboard");
						});

						genericMenu.ShowAsContext ();
		
				}
		
				void onDrawElement (Rect rect, int index, bool isActive, bool isFocused)
				{

						SerializedProperty property = __variablesReordableList.serializedProperty.GetArrayElementAtIndex (index); 

						DrawVariables (rect, property);
				}

				void onDrawHeaderElement (Rect rect)
				{
						EditorGUI.LabelField (rect, "Blackboard Variables:");
				}

				public void DrawVariables (Rect position, SerializedProperty property)
				{
						if (property == null || property.objectReferenceValue == null) {
								return;
						}

	
						//bool isAsset = AssetDatabase.Contains (blackboard.get_gameObject ());
						//	EditorGUI.BeginChangeCheck ();

		



						Type variableType;
						UnityVariable currentVariable;
						SwitchDrawerDelegate switchDrawerDelegate;

		
				

						currentVariable = (UnityVariable)property.objectReferenceValue;

						switchDrawerDelegate = switchDrawer (currentVariable.ValueType);
				
						if (switchDrawerDelegate != null) {
								//make drawing of the variable
								switchDrawerDelegate (position, currentVariable);
						}



		
						//EditorGUI.EndChangeCheck ();
		
				}

				public override void OnInspectorGUI ()
				{
						serializedObject.Update ();

						__variablesReordableList.DoLayoutList ();
						serializedObject.ApplyModifiedProperties ();
				}
		}
}