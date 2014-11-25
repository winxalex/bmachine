// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Graphs;

namespace ws.winx.editor.components
{

	public class Example : EditorWindow
	{ 
		static Example example;
		Graph stateMachineGraph;
		GraphGUI stateMachineGraphGUI;
		
		[MenuItem("Window/Example")]
		static void Do ()
		{
			example = GetWindow<Example> ();
		}
		
		void OnEnable ()
		{
			if (stateMachineGraph == null) {
				stateMachineGraph = ScriptableObject.CreateInstance<Graph> ();
				stateMachineGraph.hideFlags = HideFlags.HideAndDontSave;


				Node node=ScriptableObject.CreateInstance<Node>();
				node.title="mile";
				//node.style

				node.AddInputSlot("input");
				node.AddOutputSlot("output");
				node.AddProperty(new Property(typeof(System.Int32),"integer"));
				stateMachineGraph.AddNode(node);
			}
			if (stateMachineGraphGUI == null) {

				Node node=ScriptableObject.CreateInstance<Node>();
				node.title="mile";
				//node.style=Styles.
				
				node.AddInputSlot("input");
				node.AddOutputSlot("output");
				node.AddProperty(new Property(typeof(System.Int32),"integer"));
				stateMachineGraph.AddNode(node);
				stateMachineGraphGUI = (GetEditor (stateMachineGraph));
			}
		}
		
		void OnDisable ()
		{
			example = null;
		}
		
		void OnGUI ()
		{
			if (example && stateMachineGraphGUI != null) {
				stateMachineGraphGUI.BeginGraphGUI (example, new Rect (0, 0, example.position.width, example.position.height));
//				           stateMachineGraphGUI.OnGraphGUI ();
				Node node;
				for(int i=0;i<stateMachineGraph.nodes.Count;i++)
				{
					node=stateMachineGraph.nodes[i];
					stateMachineGraphGUI.NodeGUI(node);
					//node.NodeUI(stateMachineGraphGUI);
				}
				stateMachineGraphGUI.EndGraphGUI ();
				
			}
		}
		
		GraphGUI GetEditor (Graph graph)
		{
			GraphGUI graphGUI = CreateInstance ("GraphGUI") as GraphGUI;
			graphGUI.graph = graph;
			graphGUI.hideFlags = HideFlags.HideAndDontSave;
			return graphGUI;
		}
	}
}
