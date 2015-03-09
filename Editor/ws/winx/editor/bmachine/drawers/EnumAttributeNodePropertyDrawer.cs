using BehaviourMachine;
using System;
using UnityEditor;
using UnityEngine;
using ws.winx.bmachine.extensions;
using ws.winx.editor;
using System.Collections.Generic;
using BehaviourMachineEditor;
using UnityEditorInternal;
using System.Linq;
using ws.winx.unity;
using System.Reflection;
using ws.winx.unity.attributes;


namespace ws.winx.editor.bmachine.drawers
{
		[CustomNodePropertyDrawer (typeof(EnumAttribute))]
		public class EnumAttributeNodePropertyDrawer : NodePropertyDrawer
		{

			
				float _valueCurrent;
				
			    
				//
				// Properties
				//
				public new EnumAttribute attribute {
						get {
						
							return  (EnumAttribute)base.attribute;
						}
				}

				//
				// Methods
				//
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{
		
					Enum @enum;
		
					if (Enum.IsDefined (this.attribute.GetEnumType (), property.value))
					{
						@enum = (Enum)Enum.ToObject (this.attribute.GetEnumType (), property.value);
					}
					else
					{
						@enum = this.attribute.GetEnumValue ();
					}

					@enum=EditorGUILayout.EnumPopup (@enum);
				
			        
					property.value= ((int)Convert.ChangeType (@enum, @enum.GetTypeCode ()));
					property.ApplyModifiedValue ();
				
		

				}

				
		

		}
}
