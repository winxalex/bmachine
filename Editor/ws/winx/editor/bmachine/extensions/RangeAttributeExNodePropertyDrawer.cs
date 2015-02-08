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
using System.Reflection;

namespace ws.winx.editor.bmachine.extensions
{
		[CustomNodePropertyDrawer (typeof(RangeAttributeEx))]
		public class RangeAttributeExNodePropertyDrawer : NodePropertyDrawer
		{

			
				float _valueCurrent;
				
			    
				//
				// Properties
				//
				public new RangeAttributeEx attribute {
						get {
						
								return  (RangeAttributeEx)base.attribute;
						}
				}

				//
				// Methods
				//
				public override void OnGUI (SerializedNodeProperty property, ActionNode node, GUIContent guiContent)
				{
	

						attribute.serializedObject = node;

					

						GUILayout.BeginHorizontal ();


						if (attribute.Min < attribute.Max) {
								if (attribute.HasEnableToggle)
										attribute.Enabled = GUILayout.Toggle (attribute.Enabled, property.label);

								if (attribute.Enabled) {
									
										GUILayout.Label (attribute.Min.ToString ());
									
										_valueCurrent = GUILayout.HorizontalSlider (_valueCurrent, attribute.Min, attribute.Max);
										property.value = _valueCurrent;
										GUILayout.Label (attribute.Max.ToString ());


										property.ApplyModifiedValue ();
								}
						}
					
						GUILayout.EndHorizontal ();

				}

				
		

		}
}
