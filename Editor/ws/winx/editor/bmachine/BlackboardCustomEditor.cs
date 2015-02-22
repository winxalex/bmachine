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
				Func<Type,SwitchDrawerDelegate>  @switch;

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
				



									@switch = BlackboardCustomEditor.SwitchOfType (
					new Func<Type, SwitchDrawerDelegate>[]{

					Case<float,SwitchDrawerDelegate> (new SwitchDrawerDelegate (EditorGUILayoutEx.DrawFloatVar)),
						Case<Vector3,SwitchDrawerDelegate> (new SwitchDrawerDelegate (EditorGUILayoutEx.DrawVector3Var))
				});

			
						}
				}

				public static Func<Type, K> Case<T,K> (K action)
				{
			
						return  o => SwitchUtility.IsSameOrSubclass (typeof(T), o) ? 
				action 
					: default(K);
				}

				public static Func<Type,T> SwitchOfType<T> (params Func<Type, T>[] tests)
				{
						return o =>
						{
								var @case = tests
					.Select (f => f (o))
						.FirstOrDefault (a => a != null);


								return @case;
//				
//				if (@case != null)
//				{
//					@case();
//				}
						};
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

				void onMenuSelection(object userData){

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

			GUIContent[] displayOptions = new GUIContent[]{new GUIContent ("float")};//,"int","bool","Vector3","Quaternion"};
			Type[] values = new Type[]{typeof(float)};//System.Single,System.Int32,System.Boolean,typeof(UnityEngine.Vector3)};

			System.Reflection.Assembly

			GenericMenu genericMenu = EditorGUILayoutEx.GeneraterGenericMenu<Type> (displayOptions, values, onMenuSelection);

						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						
//						genericMenu.AddItem (new GUIContent ("Float"), false, delegate {
//								AddVariableToList<float> ("New Float", 0f, list);
//						});
//						genericMenu.AddItem (new GUIContent ("Int"), false, delegate {
//								AddVariableToList<int> ("New Int", 0, list);
//						});
//						genericMenu.AddItem (new GUIContent ("Bool"), false, delegate {
//								AddVariableToList<bool> ("New Bool", false, list);
//						});
//						genericMenu.AddItem (new GUIContent ("String"), false, delegate {
//								AddVariableToList<String> ("New String", String.Empty, list);
//						});
//						genericMenu.AddItem (new GUIContent ("Vector3"), false, delegate {
//								AddVariableToList<Vector3> ("New Vector3", new Vector3 (), list);
//						});
//						genericMenu.AddItem (new GUIContent ("Rect"), false, delegate {
//								AddVariableToList<Rect> ("New Rect", new Rect (), list);
//						});
//						genericMenu.AddItem (new GUIContent ("Color"), false, delegate {
//								AddVariableToList<Color> ("New Color", new Color (), list);
//						});
//						genericMenu.AddItem (new GUIContent ("Quaternion"), false, delegate {
//								AddVariableToList<Quaternion> ("New Quaternion", new Quaternion (), list);
//						});
//						genericMenu.AddItem (new GUIContent ("GameObject"), false, delegate {
//								AddVariableToList<GameObject> ("New GameObject", new GameObject (), list);
//						});
//						genericMenu.AddItem (new GUIContent ("Texture"), false, delegate {
//								AddVariableToList<Texture> ("New Texture", new Texture (), list);
//						});
//						genericMenu.AddItem (new GUIContent ("Material"), false, delegate {
//								AddVariableToList<Material> ("Material", new Material (Shader.Find ("Diffuse"), list));
//						});
						genericMenu.AddItem (new GUIContent ("Object property"), false, delegate {
				
						});
						genericMenu.AddItem (new GUIContent ("Custom Object"), false, delegate {

						});
						genericMenu.AddItem (new GUIContent ("FsmEvent"), false, delegate {
								//maybe we go with UnityEvent
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

						switchDrawerDelegate = @switch (currentVariable.ValueType);
				
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