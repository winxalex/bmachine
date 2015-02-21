using BehaviourMachine;
using System;
using UnityEditor;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor.extensions;
using System.Collections.Generic;
using BehaviourMachineEditor;
using UnityEditorInternal;
using System.Linq;
using ws.winx.unity;

namespace ws.winx.editor.bmachine.extensions
{
		[CustomNodePropertyDrawer (typeof(UnityVariablePropertyAttribute))]
		public class VariablePropertyDrawer : NodePropertyDrawer
		{

				ReorderableList  __variablesList;

				public new UnityVariablePropertyAttribute attribute { 
						get{ return (UnityVariablePropertyAttribute)attribute;}
				}
				
		
				//
				// Methods
				//
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{

						attribute.serializedObject = property.serializedNode;
					


						__variablesList = new ReorderableList (attribute.variablesList, typeof(UnityVariable), 
				true, true, true, true);

					

		
						property.ApplyModifiedValue ();

				}
		

		}
}
