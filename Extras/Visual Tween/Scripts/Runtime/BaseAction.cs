using UnityEngine;
using System.Collections;

namespace VisualTween.Action{
	[System.Serializable]
	public class BaseAction : ScriptableObject {
		public virtual void OnEnter(GameObject target){
			
		}
		
		public virtual void OnUpdate(GameObject target, float percentage){
			
		}
		
		public virtual void OnGUI(){
			
		}
		
		public virtual void OnExit(GameObject target){
			
		}

		public virtual void RecordAction(GameObject target){
			
		}

		public virtual void UndoAction(GameObject target){

		}
	}
}