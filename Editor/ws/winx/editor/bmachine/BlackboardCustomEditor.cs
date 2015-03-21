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
using UnityEngine.Events;

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
				Action<Type> switchMenuTypes;
				string _typeNameSelected = "None";
				List<Type> typesCustom;

				private void OnEnable ()
				{


						//serializedObject.FindProperty("variablesList").objectReferenceValue
						if (__variablesReordableList == null) {

								
								SerializedProperty variableListSerialized = serializedObject.FindProperty ("variablesList");

								typesCustom = ((BlackboardCustom)serializedObject.targetObject).typesCustom;
								if (typesCustom == null)
										typesCustom = ((BlackboardCustom)serializedObject.targetObject).typesCustom = new List<Type> ();
								
				
				
								__variablesReordableList = new ReorderableList (serializedObject, variableListSerialized, 
				                                       true, true, true, true);
								__variablesReordableList.drawElementCallback = onDrawElement;

								__variablesReordableList.onAddDropdownCallback = onAddDropdownCallback;

								__variablesReordableList.drawHeaderCallback = onDrawHeaderElement;

								__variablesReordableList.onRemoveCallback = onRemoveCallback;

								__variablesReordableList.onSelectCallback = onSelectCallback;
				

								genericMenu = EditorGUILayoutEx.GeneraterGenericMenu<Type> (EditorGUILayoutEx.unityTypesDisplayOptions, EditorGUILayoutEx.unityTypes, onTypeSelection);

								fillMenuCustomTypes ();

								

							





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
					SwitchUtility.CaseIsClassOf<UnityEvent,Action> (() => {
										AddVariableToList<UnityEvent> ("New UnityEvent", new UnityEvent (), __variablesReordableList);}),
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
										AddVariableToList<GameObject> ("New GameObject", new GameObject (), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<AnimationCurve,Action> (() => {
										AddVariableToList<AnimationCurve> ("New AnimationCurve", new AnimationCurve (new Keyframe (0f, 0f), new Keyframe (1f, 1f)), __variablesReordableList);}),
					SwitchUtility.CaseIsClassOf<AnimationClip,Action> (() => {
										AddVariableToList<AnimationClip> ("New AnimationClip", new AnimationClip (), __variablesReordableList);}),
				
				
					SwitchUtility.CaseIsClassOf<UnityEngine.Object,Action> (() => {
										AddVariableToList<UnityEngine.Object> ("Name of Variable", new UnityEngine.Object (), __variablesReordableList);})

						}
								);


						}

				}

				void fillMenuCustomTypes ()
				{
						genericMenu.AddSeparator (string.Empty);
						genericMenu.AddItem (new GUIContent ("Any UnityObject"), false, onTypeSelection, typeof(UnityEngine.Object));


						Type[] derivedTypes = TypeUtility.GetDerivedTypes (typeof(System.Object));
						int i = 0;
						Type typeUnityObject = typeof(UnityEngine.Object);
						for (i = 0; i < derivedTypes.Length; i++) {
				
								if (!derivedTypes [i].IsSubclassOf (typeUnityObject)) {
										string text2 = derivedTypes [i].ToString ();
										genericMenu.AddItem (new GUIContent ("Custom Object/" + text2.Replace ('.', '/')), _typeNameSelected == text2, onTypeCustomSelected, derivedTypes [i]);
								}
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
						AddVariableToList1 (name, value, list);
				}

				void AddVariableToList1 (string name, object value, ReorderableList list)
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

						//show popup just for complex types and UnityEvent
						if (variable != null
								&& (Array.IndexOf (EditorGUILayoutEx.unityTypes, variable.ValueType) < 0
								|| variable.ValueType == typeof(UnityEvent)) 
			    ) {
								UnityObjectEditorWindow.Show (variable);


						}
				}

				void onRemoveCallback (ReorderableList list)
				{
						if (UnityEditor.EditorUtility.DisplayDialog ("Warning!", 
			                                "Are you sure you want to delete the Unity Variable?", "Yes", "No")) {
								ReorderableList.defaultBehaviours.DoRemoveButton (list);
								
								list.serializedProperty.DeleteArrayElementAtIndex (list.index);
								

								serializedObject.ApplyModifiedProperties ();
								
						}
				}

				void onAddDropdownCallback (Rect buttonRect, ReorderableList list)
				{

						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						int i;

						if (typesCustom != null && typesCustom.Count > 0) {
								genericMenu.AddSeparator (string.Empty);
								for (i=0; i<typesCustom.Count; i++) {
										genericMenu.AddItem (new GUIContent (typesCustom [i].Name), true, onTypeCustomSelected, typesCustom [i]);
								}
							    
						}

						genericMenu.ShowAsContext ();
		
				}
		
				void onDrawElement (Rect rect, int index, bool isActive, bool isFocused)
				{

						SerializedProperty property = __variablesReordableList.serializedProperty.GetArrayElementAtIndex (index); 

						if (property == null || property.objectReferenceValue == null) {
								return;
						}

						DrawVariables (rect, property);



				}

				void onDrawHeaderElement (Rect rect)
				{
						EditorGUI.LabelField (rect, "Blackboard Variables:");
				}

				public Rect DrawVariables (Rect position, SerializedProperty property)
				{
						
					
						UnityVariable currentVariable;
					

		
				

						currentVariable = (UnityVariable)property.objectReferenceValue;

						Type type = currentVariable.ValueType;

						if (Array.IndexOf (EditorGUILayoutEx.unityTypes, type) < 0
								|| type == typeof(UnityEvent)) {

								currentVariable.name = EditorGUI.TextField (position, currentVariable.name);

						} else {
								

								PropertyDrawer drawer = EditorUtilityEx.GetDefaultDrawer ();
			
			
								Rect pos = new Rect (32, position.y, 80, 16);

								currentVariable.name=EditorGUI.TextField (pos, currentVariable.name);
								position.x = 113f;
								position.width -= 80f;
								drawer.OnGUI (position, currentVariable.serializedProperty, new GUIContent (""));
			
								currentVariable.ApplyModifiedProperties ();

						}
			
					



			
					



		
						return position;
		
				}

				void onTypeCustomSelected (object userData)
				{
						Type type = (Type)userData;


						//or use
						//Activator.CreateInstance ((Type)userData);
						

		
						if (typesCustom.IndexOf (type) < 0) {
								typesCustom.Add (type);

						}

				
						AddVariableToList ("New " + type.Name, FormatterServices.GetUninitializedObject (type), __variablesReordableList);


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