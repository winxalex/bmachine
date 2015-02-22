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

namespace ws.winx.editor.bmachine {

    /// <summary>
    /// Wrapper class for BehaviourTreeEditor.
    /// <seealso cref="BehaviourMachine.BehaviourTree" />
    /// </summary>
    [CustomEditor(typeof(BlackboardCustom))]
	public class BlackboardCustomEditor :Editor{

		ReorderableList  __variablesReordableList;
		Action<object> @switch;

		private void OnEnable() {


			
			if (__variablesReordableList == null) {
				__variablesReordableList = new ReorderableList (serializedObject, serializedObject.FindProperty("variablesList"), 
				                                       true, true, true, true);
				__variablesReordableList.drawElementCallback=onDrawElement;

				__variablesReordableList.onAddDropdownCallback=onAddDropdownCallback;

//				@switch=SwitchUtility.SwitchOfType(
//					new []{
//					  SwitchUtility.Case<float>(
//					}
//				);



			}
		}


		void AddVariable(UnityVariable variable,Type type){

		}

		void onAddDropdownCallback (Rect buttonRect, ReorderableList list)
		{
			GUIUtility.hotControl = 0;
			GUIUtility.keyboardControl = 0;
			GenericMenu genericMenu = new GenericMenu ();
			genericMenu.AddItem (new GUIContent ("Float"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=0f;
				list.list.Add(variable);
			

			});
			genericMenu.AddItem (new GUIContent ("Int"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=(int)0;
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Bool"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=false;
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("String"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=string.Empty;
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Vector3"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new Vector3();
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Rect"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new Rect();
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Color"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new Color();
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Quaternion"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new Quaternion();
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("GameObject"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new GameObject();
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Texture"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new Texture();
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Material"), false, delegate
			                     {
				UnityVariable variable= ScriptableObject.CreateInstance<UnityVariable>();
				variable.Value=new Material(Shader.Find("Diffuse"));
				list.list.Add(variable);
			});
			genericMenu.AddItem (new GUIContent ("Object"), false, delegate
			                     {

			});
			genericMenu.AddItem (new GUIContent ("FsmEvent"), false, delegate
			                     {

			});

				genericMenu.AddSeparator (string.Empty);
				genericMenu.AddItem (new GUIContent ("Global Blackboard"), false, delegate
				                     {
					EditorApplication.ExecuteMenuItem ("Tools/BehaviourMachine/Global Blackboard");
				});

			genericMenu.ShowAsContext ();
		
		}		
		
		
		void onDrawElement(Rect rect, int index, bool isActive, bool isFocused){

			var element = __variablesReordableList.serializedProperty.GetArrayElementAtIndex(index); 
			//element.serializedObject
		}


		public void DrawVariables (Rect position, UnityVariable variable)
		{
			if (variable == null)
			{
				return;
			}

	
			//bool isAsset = AssetDatabase.Contains (blackboard.get_gameObject ());
			EditorGUI.BeginChangeCheck ();

			List<UnityVariable> variablesList = __variablesReordableList.list as List<UnityVariable>;

			int variablesNum = variablesList.Count;

			Type variableType;
			UnityVariable currentVariable;

			for (int i = 0; i < variablesNum; i++)
			{
				currentVariable=variablesList[i];

				@switch(currentVariable.ValueType);

			}

		
			EditorGUI.EndChangeCheck ();
		
		}


	

		public override void OnInspectorGUI ()
		{
			serializedObject.Update();
			__variablesReordableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}