using UnityEngine;
using UnityEditor;
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
using ws.winx.editor.drawers;
using ws.winx.editor.utilities;
using ws.winx.unity.ai.behaviours;

namespace ws.winx.editor.ai.behaviours
{
	[CustomEditor(typeof(Blackboard))]
	public class BlackboardEditor :Editor
	{
				
		ReorderableList  __variablesReordableList;
		string _typeNameSelected = "None";
		List<Type> _typesCustom;
		Rect variableNameTextFieldPos = new Rect (32, 0, 80, 16);
		GenericMenu menu;
		SerializedProperty variableListSerialized;
		string typeFullPath;


		//!!! Don't Create staics in Editor (Editor is recreated on every selection)
		//Have problems with static GenericMenu losing instances

		/// <summary>
		/// Raises the enable event on every selection of Blackboard GameObject and creates new instance of BlackboardEditor
		/// </summary>
		private void OnEnable ()
		{
			//on every selection 
			//Debug.Log ("OnEnable BlackBoard");

			variableListSerialized = serializedObject.FindProperty ("variablesList");
			
			((Blackboard)target).variablesList.ForEach (var => {
				if (var != null && var.serializedProperty == null) {
					var.serializedProperty = EditorUtilityEx.SerializeObject (var);
					EditorUtilityEx.UpdateSerializedProperty (var);
					
					
				}
			});
			
			
			__variablesReordableList = new ReorderableList (serializedObject, variableListSerialized, 
			                                                true, true, true, true);
			__variablesReordableList.drawElementCallback = onDrawElement;
			
			__variablesReordableList.onAddDropdownCallback = onAddDropdownCallback;
			
			__variablesReordableList.drawHeaderCallback = onDrawHeaderElement;
			
			__variablesReordableList.onRemoveCallback = onRemoveCallback;
			
			__variablesReordableList.onSelectCallback = onSelectCallback;
			
			__variablesReordableList.elementHeight = 32f;
			
				
			_typesCustom = ((Blackboard)target).typesCustom as List<Type>;

			if (_typesCustom == null)
				_typesCustom = new List<Type> ();
			

					

			CreateGenericMenu ();
				


		}

		void CreateGenericMenu ()
		{
			menu = EditorGUILayoutEx.GeneraterGenericMenu<Type> (EditorGUILayoutEx.unityTypesDisplayOptions, EditorGUILayoutEx.unityTypes, onTypeSelection);

			menu.AddSeparator (string.Empty);
					

			menu.AddItem (new GUIContent ("Any UnityVariable"), false, (userData) => {

				AddUnityVariableOfTypeToList ("UniUnityVar", (Type)userData, new UniUnityVariablePropertyDrawer ());
			}, typeof(float)
			                     
			                     
			                     
			);


//			Type[] derivedTypes = EditorUtilityEx.GetDerivedTypes (typeof(System.Object));
//			int i = 0;
//			Type typeUnityObject = typeof(UnityEngine.Object);
//			for (i = 0; i < derivedTypes.Length; i++) {
//				
//				if (!derivedTypes [i].IsSubclassOf (typeUnityObject)) {
//					string text2 = derivedTypes [i].ToString ();
//					menu.AddItem (new GUIContent ("Custom Object/" + text2.Replace ('.', '/')), _typeNameSelected == text2, onTypeCustomSelected, derivedTypes [i]);
//				}
//			}

			menu.AddSeparator (string.Empty);
			menu.AddItem (new GUIContent ("Reload"), false, delegate {
				CreateGenericMenu ();
			});

						
			
			
			menu.AddSeparator (string.Empty);
//			menu.AddItem (new GUIContent ("Global Blackboard"), false, delegate {
//				Debug.LogWarning ("Not implemented");
//				//EditorApplication.ExecuteMenuItem ("Tools/Global Blackboard");
//			});
		}


		/// <summary>
		/// Adds the unity variable of type to list.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="type">Type.</param>
		/// <param name="drawer">Drawer.</param>
		void AddUnityVariableOfTypeToList (String name, Type type, PropertyDrawer drawer=null)
		{

			ReorderableList list = __variablesReordableList;
			var index = list.serializedProperty.arraySize;
			list.serializedProperty.arraySize++;
			list.index = index;

			UnityVariable variable = UnityVariable.CreateInstanceOf (type);
			variable.name = name;
			variable.drawer = drawer;
							
			
//						UnityVariable variable = (UnityVariable)ScriptableObject.CreateInstance<UnityVariable> ();
//						variable.name = name;
//						variable.drawer = drawer;
//						variable.Value = value;
			
			var element = list.serializedProperty.GetArrayElementAtIndex (index);
			element.objectReferenceValue = variable;
			
			serializedObject.ApplyModifiedProperties ();

						
		}

		void onTypeSelection (object userData)
		{
			Type type = (Type)userData;
			//AddUnityVariableOfTypeToList ("New " + type.Name, type, __variablesReordableList);	
			AddUnityVariableOfTypeToList ("New " + type.Name, type);
					
		}

		void onSelectCallback (ReorderableList list)
		{
			UnityVariable variable = list.serializedProperty.GetArrayElementAtIndex (list.index).objectReferenceValue as UnityVariable;

			//show popup just for complex types and UnityEvent
			if (variable != null
				&& (Array.IndexOf (EditorGUILayoutEx.unityTypes, variable.ValueType) < 0
				|| variable.ValueType == typeof(UnityEvent)) 
			    ) {
				UnityVariableEditorWindow.Show (variable);


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



			if (_typesCustom != null && _typesCustom.Count > 0) {
				menu.AddSeparator (string.Empty);
				for (i=0; i<_typesCustom.Count; i++) {
					menu.AddItem (new GUIContent (_typesCustom [i].Name), true, onTypeCustomSelected, _typesCustom [i]);
				}
							    
			}

		

			menu.ShowAsContext ();
		
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

						
						
			if (currentVariable != null) {
				Type type = currentVariable.ValueType;

				if (currentVariable.serializedProperty == null) {
					currentVariable.Value = UnityVariable.Default (type);
					currentVariable.serializedProperty = EditorUtilityEx.SerializeObject (currentVariable);
				}

								
				//if UnityVariable isn't of type of known unityTypes or it is UnityEvent or have custom Drawer
				if (Array.IndexOf (EditorGUILayoutEx.unityTypes, type) < 0
					|| type == typeof(UnityEvent) || (currentVariable.drawer != null)) {

										


					//!!! this part is for Any UnityVariable with UnityTypes wiht UniUnityVariablePropertyDrawer (experimental)
					if (currentVariable.ValueType != typeof(UnityEvent) && currentVariable.drawer != null) {			

												
						variableNameTextFieldPos.y = position.y;
						variableNameTextFieldPos.x = position.x;
						
						EditorGUI.BeginChangeCheck ();
						currentVariable.name = EditorGUI.TextField (variableNameTextFieldPos, currentVariable.name);
						if (EditorGUI.EndChangeCheck ()) {
							

							property.serializedObject.ApplyModifiedProperties ();
							
							//EditorUtility.SetDirty (currentVariable);
							
							
							
						}
										
						position.xMin = variableNameTextFieldPos.xMax + 10;
							
												
						(currentVariable.drawer as UniUnityVariablePropertyDrawer).OnGUI (position, property, null);
					} else
						currentVariable.name = EditorGUI.TextField (position, currentVariable.name);
										

				} else {
					PropertyDrawer drawer;

										
					SerializedProperty serialzedProperty=currentVariable.serializedProperty as SerializedProperty;
										

					if (currentVariable.IsBinded ()){
						drawer = new UniUnityVariablePropertyDrawer ();
						serialzedProperty=property;
					}
					else{
						drawer = EditorUtilityEx.GetDrawer (type);//Search drawer for type
					}

					if (drawer == null)
						drawer = EditorUtilityEx.GetDefaultDrawer ();
			
			
					variableNameTextFieldPos.y = position.y;
					variableNameTextFieldPos.x = position.x;
						
					currentVariable.name = EditorGUI.TextField (variableNameTextFieldPos, currentVariable.name);
					position.xMin = variableNameTextFieldPos.xMax + 10;
					//position.width =position.width- variableNameTextFieldPos.width;

					EditorGUI.BeginChangeCheck ();
				
										

					drawer.OnGUI (position, serialzedProperty, new GUIContent (""));
			
					if (EditorGUI.EndChangeCheck ()) {
						
						EditorUtilityEx.ApplySerializedPropertyChangeTo (currentVariable);

						
						EditorUtility.SetDirty (currentVariable);


						
					}
										
				}
			}
			
					



			
					



		
			return position;
		
		}

		void onTypeCustomSelected (object userData)
		{
			Type type = (Type)userData;


			//or use
			//Activator.CreateInstance ((Type)userData);
						

		
			if (_typesCustom.IndexOf (type) < 0) {
				_typesCustom.Add (type);

			}

				
			AddUnityVariableOfTypeToList ("New " + type.Name, type);


		}

		public override void OnInspectorGUI ()
		{

			base.DrawDefaultInspector ();

			EditorGUILayout.LabelField ("Add new type ex.'testNamespace.test.MyClassSerializable,Assembly-CSharp'");

			EditorGUILayout.BeginHorizontal ();

			typeFullPath = EditorGUILayout.TextField ("New type:", typeFullPath);
			if (GUILayout.Button ("+")) {

				Type type = Type.GetType (typeFullPath);

				if (type != null && _typesCustom.IndexOf (type) < 0) {
					_typesCustom.Add (type);
					menu.AddItem (new GUIContent (type.Name), true, onTypeCustomSelected, type);
					typeFullPath = "";
					this.Repaint();
				}
			}


			if (GUILayout.Button ("-")) {

				
				Type type = Type.GetType (typeFullPath);

				if (type != null && _typesCustom.IndexOf (type) >-1) {
					_typesCustom.Remove (type);

					CreateGenericMenu();
					typeFullPath = "";
					this.Repaint();
				}
			}
			EditorGUILayout.EndHorizontal ();

			//serializedObject.Update ();
			if (__variablesReordableList != null)
				__variablesReordableList.DoLayoutList ();

			//serializedObject.ApplyModifiedProperties ();
		}

		void OnDisable ()
		{


		}
	}
}