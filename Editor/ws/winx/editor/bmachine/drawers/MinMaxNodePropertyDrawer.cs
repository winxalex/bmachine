

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ws.winx.unity.attributes;
using BehaviourMachineEditor;

namespace ws.winx.editor.bmachine.drawers
{

[CustomNodePropertyDrawer( typeof( MinMaxRangeSAttribute ) )]
public class MinMaxRangeDrawer : NodePropertyDrawer
{
	




	public override void OnGUI (SerializedNodeProperty property, BehaviourMachine.ActionNode node, GUIContent guiContent)
	{

		
		// Now draw the property as a Slider or an IntSlider based on whether it’s a float or integer.
		if ( property.type != typeof(MinMaxRangeSO) )
			Debug.LogWarning( "Use only with MinMaxRange type" );
		else
		{
				var range = attribute as MinMaxRangeSAttribute;
				var so=(MinMaxRangeSO)property.value;


				//set or reset to default full range
				if(so.rangeStart==so.rangeEnd){
					so.rangeStart=range.minLimit;
					so.rangeEnd=range.maxLimit;

				}

				var newMin =so.rangeStart;
				var newMax = so.rangeEnd;


			Rect position=	GUILayoutUtility.GetRect(Screen.width-32f,32f);
			
			var xDivision = position.width * 0.33f;
			var yDivision = position.height * 0.5f;
			EditorGUI.LabelField( new Rect( position.x, position.y, xDivision, yDivision )
			                     , guiContent );
			
			EditorGUI.LabelField( new Rect( position.x, position.y + yDivision, position.width, yDivision )
			                     , range.minLimit.ToString( "0.##" ) );
			EditorGUI.LabelField( new Rect( position.x + position.width -28f, position.y + yDivision, position.width, yDivision )
			                     , range.maxLimit.ToString( "0.##" ) );
			
			
			EditorGUI.MinMaxSlider( new Rect( position.x + 24f, position.y + yDivision, position.width -48f, yDivision )
			                       , ref newMin, ref newMax, range.minLimit, range.maxLimit );
			
			EditorGUI.LabelField( new Rect( position.x + xDivision, position.y, xDivision, yDivision )
			                     , "From: " );
			newMin = Mathf.Clamp( EditorGUI.FloatField( new Rect( position.x + xDivision + 30, position.y, xDivision -30, yDivision )
			                                           , newMin )
			                     , range.minLimit, newMax );
			EditorGUI.LabelField( new Rect( position.x + xDivision * 2f, position.y, xDivision, yDivision )
			                     , "To: " );
			newMax = Mathf.Clamp( EditorGUI.FloatField( new Rect( position.x + xDivision * 2f + 24, position.y, xDivision -24, yDivision )
			                                           , newMax )
			                     , newMin, range.maxLimit );
			
			so.rangeStart = newMin;
			so.rangeEnd = newMax;

				property.ValueChanged();

				property.ApplyModifiedValue();

				property.serializedNode.ApplyModifiedProperties();
		}
	}
}
}