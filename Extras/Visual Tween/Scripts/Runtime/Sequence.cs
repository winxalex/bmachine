using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System.Linq;

namespace VisualTween{
	public class Sequence : MonoBehaviour {

		[SerializeField]
		List<SequenceChannel> _channels;

		public List<SequenceChannel> channels {
			get {
				if(_channels==null) _channels=new List<SequenceChannel>();
				return _channels;
			}
		}

		public SequenceNode selectedNode;
		public float sequenceEnd;
		public SequenceWrap wrap=SequenceWrap.ClampForever;
		private float time;
		private bool stop=true;
		private bool pause;
		private float pauseTime;
		public bool playOnStart=true;




		private void Start(){
			if (playOnStart) {
				Play();			
			}
		}

		public float passedTime;
		private bool playForward=true;
//		private void Update(){
//			if (pause || stop) {
//				return;			
//			}
//
//			foreach (var kvp in GetGroupTargets()) {
//				SequenceNode mNode=kvp.Value.OrderBy(x=>x.startTime).First();
//				mNode.RecordAction();
//			}
//
//
//			if (Time.time > time) {
//				switch(wrap){
//				case SequenceWrap.PingPong:
//					time=Time.time+sequenceEnd;
//					playForward=!playForward;
//					break;
//				case SequenceWrap.Once:
//					Stop(false);
//					break;
//				case SequenceWrap.ClampForever:
//					Stop(true);
//					break;
//				case SequenceWrap.Loop:
//					Restart();
//					break;
//				}			
//			}
//
//			passedTime += Time.deltaTime*(playForward?1.0f:-1.0f);
//
//			foreach (SequenceNode node in nodes) {
//				node.UpdateTween (passedTime);		
//			}
//
//			
//			foreach (var kvp in GetGroupTargets()) {
//				SequenceNode mNode=kvp.Value.OrderBy(x=>x.startTime).ToList().Find(y=>((passedTime-y.startTime)/y.duration)<0.0f);
//				if(mNode != null)
//					mNode.UndoAction();
//			}
//		}

		public void Play(){
			sequenceEnd = 0;
			stop = false;
			pause = false;
			passedTime = 0;

			foreach (SequenceChannel channel in _channels)
			foreach (SequenceNode node in channel.nodes) {
				if(sequenceEnd< (node.startTime+node.duration)){
					sequenceEnd=node.startTime+node.duration;
				}
			}
			time=Time.time+sequenceEnd;
		}

		public void Pause(){
			pause = true;	
			pauseTime = Time.time;
		}

		public void UnPause(){
			if (pause) {
				time += Time.time - pauseTime;
			}
			pause = false;

		}

		public void Restart(){
			Stop (false);
			Play ();
		}

	

		public void Stop(bool forward){
//			stop = true;
//			nodes=nodes.OrderBy(x=>x.startTime).ToList();
//			if (!forward) {
//				nodes.Reverse ();
//				passedTime=0;
//			}
//			
//			foreach (SequenceNode node in nodes) {
//				
//				node.CompleteTween(forward);
//			}	
//
//			if (!forward) {
//				foreach (SequenceNode node in nodes) {
//					node.UpdateTween (passedTime);		
//				}
//				
//				foreach (var kvp in GetGroupTargets()) {
//					SequenceNode mNode=kvp.Value.OrderBy(x=>x.startTime).ToList().Find(y=>((passedTime-y.startTime)/y.duration)<0.0f);
//					if(mNode != null)
//						mNode.UndoAction();
//				}			
//			}
		}

//		private void OnGUI(){
//			foreach (SequenceNode node in nodes) {
//				node.DoOnGUI();		
//			}
//		}

//		private Dictionary<GameObject,List<SequenceNode>> GetGroupTargets(){
//			Dictionary<GameObject,List<SequenceNode>> targets= new Dictionary<GameObject, List<SequenceNode>>();
//			foreach (SequenceNode node in nodes) {
//				if(!targets.ContainsKey(node.target)){
//					targets.Add(node.target,new List<SequenceNode>(){node});
//				}else{
//					targets[node.target].Add(node);
//				}
//			}
//			return targets;
//		}
//
		public enum SequenceWrap{
			Once,
			PingPong,
			Loop,
			ClampForever
		}
	}
}