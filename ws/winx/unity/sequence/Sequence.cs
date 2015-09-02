using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System.Linq;
using UnityEngine.Events;
using System;

namespace ws.winx.unity.sequence
{
		public class Sequence : MonoBehaviour
		{

		//
		// Nested Types
		//
		public class EventComparer : IComparer<SequenceEvent>
		{
			#region IComparer implementation

			public int Compare (SequenceEvent animationEvent, SequenceEvent animationEvent2)
			{

				//float time = (float)animationEvent.timeNormalized.Value;
				//float time2 = (float)animationEvent2.timeNormalized.Value;
				
				float time = (float)animationEvent.time;
				float time2 = (float)animationEvent2.time;
				if (time != time2) {
					return (int)Mathf.Sign (time - time2);
				}
				int hashCode = animationEvent.GetHashCode ();
				int hashCode2 = animationEvent2.GetHashCode ();
				return hashCode - hashCode2;
			}

			#endregion


		}

		public static EventComparer EVENT_COMPARER=new EventComparer();
		
		//events 
		public ws.winx.unity.sequence.SequenceEvent OnStart = new ws.winx.unity.sequence.SequenceEvent ();
				public ws.winx.unity.sequence.SequenceEvent OnEnd = new ws.winx.unity.sequence.SequenceEvent ();

				[NonSerialized]
				public ws.winx.unity.sequence.SequenceEvent eventSelected;

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
												node.onStop.RemoveListener (value);
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

				[SerializeField]
				List<SequenceEvent>
					_events;


				public List<SequenceEvent> events {
					get {
						if (_events == null)
							_events = new List<SequenceEvent> ();
						return _events;
					}
				}

				[NonSerialized]
				public SequenceNode nodeSelected;

				[NonSerialized]
				public SequenceChannel channelSelected;

				public SequenceWrap wrap = SequenceWrap.ClampForever;
				public bool playOnStart = true;
				public int frameRate = 30;
				public Vector2 scale;


				int _eventCurrentIndex;
				bool _isRecording;
				
				public bool isRecording {
						get {
								return _isRecording;
						}
				}

				bool _isPlaying;

				public bool isPlaying {
						get {
								return _isPlaying;
						}
				}

				


				/// <summary>
				/// The start time.in global time space (Time.time or EditorApplication.timeSinceStartup)
				/// </summary>
				double _timeStart;

				public double timeStart {
						get {
								return _timeStart;
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

				float __duration = float.NaN;

				/// <summary>
				/// Gets the duration.
				/// </summary>
				/// <value>The duration.</value>
				public float duration {
						get {
								if (float.IsNaN (__duration))
										__duration = calcDuration ();


								return __duration;
						}
				}
				
				public bool playForward;
				bool _stop = true;
				bool _pause;
				double _timeLast;
				private float _timeAtPause;
				public double timeCurrent;

				private void Start ()
				{
						if (playOnStart) {
								PlayAt ();			
						}
				}

				double _timePassed;

				public double timePassed {
						get {
								return _timePassed;
						}

				}


				/// <summary>
				/// Updates the sequence.
				/// </summary>
				/// <param name="t">/// The "time" in global time space (Time.time or EditorApplication.timeSinceStartup)</param>
				public void UpdateSequence (double t)
				{
						if (_pause || _stop) {
								return;			
						}
			
						//Debug.Log ("Time current:" + timeCurrent+"Time.time"+Time.time+" _timeAtEnd:"+_timeAtEnd);
			
						if (t > _timeAtEnd) {
								switch (wrap) {
								case SequenceWrap.PingPong:
										//_timeAtEnd = t + __duration;
										//playForward = !playForward;
										Debug.Log ("Sequence>SequenceWrap.PingPong not tested, not finished");
										break;
								case SequenceWrap.Once:
										Debug.Log ("Sequence>SequenceWrap.Once not tested, not finished");
										Stop (false);
										
										
										
										break;
								case SequenceWrap.ClampForever:
										Stop (true);
										OnEnd.Invoke (this);
										break;
								case SequenceWrap.Loop:
										//Restart (t);
										Debug.Log ("Sequence>SequenceWrap.Loop not tested, not finished");
										break;
								}	

								timeCurrent = this.duration;
						} else {

				double timeCurrentBeforeUpdate=timeCurrent;
			
								//update time
								timeCurrent += t - _timeLast;//dt



					
				/////////  Dispatch events  ///////////
				int eventsNum=events.Count;	
				double eventTime=0;
				for(int i=_eventCurrentIndex;i<eventsNum;i++)
				{
					eventTime=events[i].time;
					if(timeCurrent> eventTime && timeCurrentBeforeUpdate<eventTime)
					{
						//Debug.Log("event at time:"+timeCurrent+ "set to fire "+eventTime);
						events[i].Invoke(this);
						_eventCurrentIndex=i;
					}
				}

				
				_timeLast = t;
					
								foreach (SequenceChannel channel in this.channels)
										foreach (SequenceNode node in channel.nodes) {
												node.UpdateNode (timeCurrent);		
										}
						}
				}


				public void LateUpdateSequence(){

					///
					foreach (SequenceChannel channel in this.channels)
					foreach (SequenceNode node in channel.nodes) {
						node.LateUpdateNode (timeCurrent);		
					}

				}


				/// <summary>
				/// Update this instance.
				/// !!! Maybe move to Fixed update
				/// </summary>
				void Update ()
				{
						if (_pause || _stop) {
								return;			
						}


					
						UpdateSequence (Time.time);
						//LateUpdateSequence ();

				}


				void LateUpdate(){
					LateUpdateSequence ();
				}



	


				/// <summary>
				/// Play the sequence
				/// </summary>
				/// <param name="t">global "time"  (Time.time or EditorApplicaiton.timeSinceStartUp).</param>
				public void Play (double t)
				{
						StopRecording ();

						events.Sort (EVENT_COMPARER);
				
						__duration = calcDuration ();

						//prevent
						timeCurrent = Mathf.Min ((float)timeCurrent, __duration);

						_eventCurrentIndex=0;
						_isPlaying = true;
						_stop = false;
						_pause = false;
						_timePassed = 0;

						this._timeLast = t;
						this._timeAtEnd = t + __duration - timeCurrent;

						//dispatch Start
				
						OnStart.Invoke (this);
				}


				/// <summary>
				/// Play the  sequence  at specified time.
				/// </summary>
				/// <param name="t">T is local sequence time.</param>
				public void PlayAt (double t=0)
				{
						

						timeCurrent = t;
						
						Play (Time.time);

				}

				public void Pause ()
				{
						Pause (Time.time);
				}

				public void Pause (float t)
				{
						_pause = true;	
						_timeLast = t;
						_isPlaying = false;
				}

				public void UnPause (float t)
				{
						if (_pause) {
								//extend endTime cos of time in pause
								double pauseDuration = t - _timeLast;
								this._timeAtEnd += pauseDuration; //shift endTime for pauseDuration
								_timeLast = t;
								
						}

						_pause = false;
						_isPlaying = true;
			
				}

				public void UnPause ()
				{
						UnPause (Time.time);

				}

				public void Restart (double t)
				{
						Stop (false);
						Play (t);
				}
		
				public void Restart ()
				{
						Stop (false);
						PlayAt ();
				}

				public void Stop (bool forward)
				{
						_stop = true;

						_isPlaying = false;


						foreach (SequenceChannel channel in channels)
								foreach (SequenceNode node in channel.nodes) {
										if (node.isRunning)
												node.Stop ();
								}	

						//TODO handle forward=false loop and stuff
				


					

						//Debug.Log ("Sequence>Stop " + _isPlaying);

						
				}

				public void Record ()
				{
						_isRecording = true;

						__duration = calcDuration ();

						Stop (playForward);
				}

				public void StopRecording ()
				{

						_isRecording = false;



						

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




				public enum SequenceWrap
				{
						Once,
						PingPong,
						Loop,
						ClampForever
				}
		}
}