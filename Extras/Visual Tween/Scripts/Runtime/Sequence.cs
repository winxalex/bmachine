using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System.Linq;
using UnityEngine.Events;

namespace VisualTween
{
		public class Sequence : MonoBehaviour
		{

				//events 


//
				public event UnityAction<SequenceNode> SequenceNodeStart {
						add {
								//this.channels.ForEach (chn => chn.nodes.ForEach (nd => nd.onStart.AddListener(value)));
					
								foreach (SequenceChannel channel in this.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStart.AddListener (value);
						}
						remove {
								foreach (SequenceChannel channel in this.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStart.RemoveListener (value);
						}
				}
				//
				public event UnityAction<SequenceNode> SequenceNodeStop {
						add {
								foreach (SequenceChannel channel in this.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStop.AddListener (value);
				
				
						}
						remove {
								foreach (SequenceChannel channel in this.channels)
										foreach (SequenceNode node in channel.nodes)
												node.onStop.AddListener (value);
						}
				}

				[SerializeField]
				List<SequenceChannel>
						_channels;

				public List<SequenceChannel> channels {
						get {
								if (_channels == null)
										_channels = new List<SequenceChannel> ();
								return _channels;
						}
				}

				public SequenceNode selectedNode;
				public SequenceWrap wrap = SequenceWrap.ClampForever;
				public bool playOnStart = true;
				public bool isPlaying;
				public bool lastPlayState;


				/// <summary>
				/// The start time.in global time space (Time.time or EditorApplication.timeSinceStartup)
				/// </summary>
				double _startTime;

				public double startTime {
						get {
								return _startTime;
						}
				}

				/// <summary>
				/// The end time in global time space (Time.time or EditorApplication.timeSinceStartup)
				/// </summary>
				public double _timeAtEnd;

				public double endTime {
						get {
								return _timeAtEnd;
						}
				}

				float __duration;

				/// <summary>
				/// Gets the duration.
				/// </summary>
				/// <value>The duration.</value>
				public float duration {
						get {
								return __duration;
						}
				}

				public bool playForward;
				private bool stop = true;
				private bool pause;
				private float timeAtPause;

				private void Start ()
				{
						if (playOnStart) {
								Play ();			
						}
				}

				double _passedTime;

				public double passedTime {
						get {
								return _passedTime;
						}

				}


				/// <summary>
				/// Updates the sequence.
				/// </summary>
				/// <param name="t">T.</param>
				public void UpdateSequence (double t)
				{
						if (pause || stop) {
								return;			
						}
			

			
						if (t > _timeAtEnd) {
								switch (wrap) {
								case SequenceWrap.PingPong:
										_timeAtEnd = t + __duration;
										playForward = !playForward;
										break;
								case SequenceWrap.Once:
										Stop (false);
										break;
								case SequenceWrap.ClampForever:
										Stop (true);
										break;
								case SequenceWrap.Loop:
										Restart ();
										break;
								}			
						}
			
						_passedTime = t - _startTime;
			
						foreach (SequenceChannel channel in this.channels)
								foreach (SequenceNode node in channel.nodes) {
										node.UpdateNode (_passedTime);		
								}
				}

				void Update ()
				{
						if (pause || stop) {
								return;			
						}


					
						UpdateSequence (Time.time);
			

				}

				public void Play (double t)
				{
						__duration = calcDuration ();
					
						stop = false;
						pause = false;
						_passedTime = 0;

						this._startTime = t;
						this._timeAtEnd = t + __duration;
				}

				public void Play ()
				{
						__duration = calcDuration ();
						stop = false;
						pause = false;
						_passedTime = 0;

						_startTime = Time.time;
						_timeAtEnd = _startTime + __duration;

				}

				public void Pause ()
				{
						pause = true;	
						timeAtPause = Time.time;
				}

				public void Pause (float t)
				{
						pause = true;	
						timeAtPause = t;
				}

				public void UnPause (float t)
				{
						if (pause) {
								//extend endTime cos of time in pause
								float pauseDuration = t - timeAtPause;
								this._timeAtEnd += pauseDuration; //shift endTime for pauseDuration
								this._startTime += pauseDuration; //shift startTime for pauseDuration
						}
						pause = false;
			
				}

				public void UnPause ()
				{
						if (pause) {
								_timeAtEnd += Time.time - timeAtPause;
						}
						pause = false;

				}

				public void Restart ()
				{
						Stop (false);
						Play ();
				}

				public void Stop (bool forward)
				{
						stop = true;
//			nodes=nodes.OrderBy(x=>x.startTime).ToList();
//			if (!forward) {
//				nodes.Reverse ();
//				passedTime=0;
//			}

						foreach (SequenceChannel channel in channels)
								foreach (SequenceNode node in channel.nodes) {

										node.Stop ();
								}	

//						if (!forward) {
//								foreach (SequenceChannel channel in channels)
//										foreach (SequenceNode node in channel.nodes) {
//												node.UpdateNode (_passedTime);		
//										}
				
//				foreach (var kvp in GetGroupTargets()) {
//					SequenceNode mNode=kvp.Value.OrderBy(x=>x.startTime).ToList().Find(y=>((passedTime-y.startTime)/y.duration)<0.0f);
//					if(mNode != null)
//						mNode.UndoAction();
//				}			
//						}
				}



				/// <summary>
				/// Value of Final Node end time in local sequence time space
				/// </summary>
				/// <value>The end time.</value>
				float calcDuration ()
				{
						float duration = 0f;
			
						foreach (SequenceChannel channel  in this.channels)
								foreach (SequenceNode node in channel.nodes) {
										if (duration < (node.startTime + node.duration)) {
												duration = node.startTime + node.duration;
										}
								}
			
						return duration;
			
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
				public enum SequenceWrap
				{
						Once,
						PingPong,
						Loop,
						ClampForever
				}
		}
}