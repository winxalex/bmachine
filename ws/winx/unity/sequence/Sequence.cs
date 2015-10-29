using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System;

namespace ws.winx.unity.sequence
{
	public class Sequence : ScriptableObject
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

		public static EventComparer EVENT_COMPARER = new EventComparer ();
		
		//events 
		public ws.winx.unity.sequence.SequenceEvent OnStart = new ws.winx.unity.sequence.SequenceEvent ();
		public ws.winx.unity.sequence.SequenceEvent OnEnd = new ws.winx.unity.sequence.SequenceEvent ();
//				[NonSerialized]
//				public ws.winx.unity.sequence.SequenceEvent
//						eventSelected;

//				public event UnityAction<SequenceNode> SequenceNodeStart {
//						add {
//								//this.channels.ForEach (chn => chn.nodes.ForEach (nd => nd.onStart.AddListener(value)));
//					
//								foreach (SequenceChannel channel in this.channels)
//										foreach (SequenceNode node in channel.nodes)
//												node.onStart.AddListener (value);
//						}
//						remove {
//								foreach (SequenceChannel channel in this.channels)
//										foreach (SequenceNode node in channel.nodes)
//												node.onStart.RemoveListener (value);
//						}
//				}
//				//
//				public event UnityAction<SequenceNode> SequenceNodeStop {
//						add {
//								foreach (SequenceChannel channel in this.channels)
//										foreach (SequenceNode node in channel.nodes)
//												node.onStop.AddListener (value);
//				
//				
//						}
//						remove {
//								foreach (SequenceChannel channel in this.channels)
//										foreach (SequenceNode node in channel.nodes)
//												node.onStop.RemoveListener (value);
//						}
//				}

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
		public SequenceNode
			nodeSelected;
		[NonSerialized]
		public SequenceChannel
			channelSelected;
		public SequenceWrap wrap = SequenceWrap.ClampForever;
		public bool playOnStart = true;
		public int frameRate = 30;
		public Vector2 scale;
		int _eventCurrentIndex;

		/// <summary>
		/// The end time in global time space (Time.time or EditorApplication.timeSinceStartup)
		/// </summary>
		double _timeAtEnd;
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

		public bool testBool;


		/// <summary>
		/// The start time.in global time space (Time.time or EditorApplication.timeSinceStartup)
		/// </summary>
		double _timeStart;

		public double timeStart {
			get {
				if (!Application.isPlaying) {//refresh timeStart value in editor
					_timeStart = double.MaxValue;

					foreach (SequenceChannel channel in channels) {
						if (channel.nodes [0].timeStart < _timeStart)
							_timeStart = channel.nodes [0].timeStart;
					}


					_timeStart = _timeStart == double.MaxValue ? 0 : _timeStart;
				}

				return _timeStart;
			}
		}

		public double timeEnd {
			get {
				return timeStart + duration;
			}
		}

		float __duration = float.NaN;

		/// <summary>
		/// Gets the duration. (from node with the lowest starting time to node with the highest ending time)
		/// </summary>
		/// <value>The duration.</value>
		public float duration {
			get {
				if (float.IsNaN (__duration) || !Application.isPlaying)
					__duration = calcDuration ();//TODO make onChannels and onChannel change event and only then recalculate duration and timeStart


				return __duration;
			}
		}
				
		bool _playForward = true;

		public bool playForward {
			get {
				return _playForward;
			}
		}

		bool _stop = true;
		bool _pause;
		double _timeLast;
		private float _timeAtPause;
		public double timeCurrent;

		private void Start ()
		{
			if (playOnStart) {
				Play ();			
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
		///  This is not Behaviour Update
		/// It's expected to be ticked from outside
		/// </summary>
		/// <param name="t">/// The "time" in global time space (Time.time or EditorApplication.timeSinceStartup)</param>
		public void UpdateSequence (double t)
		{

		



			if (_pause || _stop) {
				return;			
			}


						
			
			//Debug.Log ("Time current:" + timeCurrent+"Time.time"+Time.time+" _timeAtEnd:"+_timeAtEnd);

			//Debug.Log ("Time current:" + timeCurrent+"ticki:"+t+" _timeAtEnd:"+_timeAtEnd+" _timeStart:"+timeStart+" duration:"+duration);
			
			if (t > _timeAtEnd) {
				switch (wrap) {
				case SequenceWrap.PingPong:
										//_timeAtEnd = t + __duration;
										//playForward = !playForward;
					Debug.Log ("Sequence>SequenceWrap.PingPong not tested, not finished");
					break;
				case SequenceWrap.Once:
					//Debug.Log ("Sequence>SequenceWrap.Once not tested, not finished");

					///////////////////////////// Update Nodes ////////////////////////

					if(_playForward)
						timeCurrent=timeStart;
					else
						timeCurrent=timeEnd;



					foreach (SequenceChannel channel in this.channels)
						foreach (SequenceNode node in channel.nodes) {
								
								node.GoTO(timeCurrent);
								
						}

					Stop ();

					if(OnEnd!=null)
					OnEnd.Invoke (this);
										
										
					break;
				case SequenceWrap.ClampForever:
					Stop ();

					if (_playForward)
						timeCurrent = timeEnd;
					else
						timeCurrent = timeStart;

					if(OnEnd!=null)
					OnEnd.Invoke (this);
										
					break;
				case SequenceWrap.Loop:

					if(OnEnd!=null)
						OnEnd.Invoke (this);

					if (_playForward)
						timeCurrent = timeStart;
					else
						timeCurrent = timeEnd;

					PlayBy (t, _playForward);

				
				//	Debug.LogWarning ("Sequence>SequenceWrap.Loop not tested, not finished");
					break;
				}	

								
			} else {

				double timeCurrentBeforeUpdate = timeCurrent;
			
				//update time
				if (_playForward)
					timeCurrent += t - _timeLast;//dt
								else
					timeCurrent -= t - _timeLast;


				//Debug.Log("Time:"+timeCurrent);

					
				/////////  Dispatch events  ///////////
				int eventsNum = events.Count;	
				double eventTime = 0;
				for (int i=_eventCurrentIndex; i<eventsNum; i++) {
					eventTime = events [i].time;
					if (timeCurrent > eventTime && timeCurrentBeforeUpdate < eventTime) {
						//Debug.Log("event at time:"+timeCurrent+ "set to fire "+eventTime);
						events [i].Invoke (this);
						_eventCurrentIndex = i;
					}
				}
				/////////////////////////////////////////
				
				_timeLast = t;
					

				////////////////////////// Update Nodes ////////////////////////
				foreach (SequenceChannel channel in this.channels)
					foreach (SequenceNode node in channel.nodes) {

						node.UpdateNode (timeCurrent);		
					}
			}
			///////////////////////////////////////////////////////////////////


			



		}

			

			

				
		/// <summary>
		/// This is not Behaviour Update
		/// It's expected to be ticked from outside
		/// </summary>
		public void LateUpdate ()
		{
			foreach (SequenceChannel channel in this.channels)
				foreach (SequenceNode node in channel.nodes) {
					node.LateUpdateNode (timeCurrent);		
				}
		}



	


		/// <summary>
		/// Play the sequence BY
		/// </summary>
		/// <param name="t">global "time"  (Time.time or EditorApplicaiton.timeSinceStartUp).</param>
		public void PlayBy (double t, bool forward=true)
		{
			this._playForward = forward;

			StopRecording ();

			Stop ();
						
			foreach (SequenceChannel channel in channels)
				channel.Reset ();

			events.Sort (EVENT_COMPARER);
				
			//prevent timeCurrent going outside 
			timeCurrent = Mathf.Min ((float)timeCurrent, (float)timeStart + duration);
			timeCurrent = Mathf.Max ((float)timeCurrent, (float)timeStart);

			_eventCurrentIndex = 0;
			_isPlaying = true;
			_stop = false;
			_pause = false;
			_timePassed = 0;

			this._timeLast = t;

			if (forward)
				this._timeAtEnd = t + timeStart + duration - timeCurrent;
			else
				this._timeAtEnd = t + timeCurrent - timeStart;


			//dispatch Start Event
			if (OnStart != null)
				OnStart.Invoke (this);
		}


		/// <summary>
		/// 
		/// Play the  sequence  at specified time.
		/// !!! USE THIS FUNCTION FOR SEQUENCE PLAY
		/// </summary>
		/// <param name="t">T is local sequence time.</param>
		/// <param name="forward">If set to <c>true</c> forward.</param>
		public void Play (double t=0, bool forward=true)
		{
						
			timeCurrent = t;
						
			PlayBy (Time.time, forward);

		}

		/// <summary>
		/// Pause this instance.
		/// !!! USE THIS FUNCTION FOR SEQUENCE RUNTIME PAUSE
		/// </summary>
		public void Pause ()
		{
			Pause (Time.time);
		}


		/// <summary>
		/// Pause the specified t.
		/// </summary>
		/// <param name="t">T is global time Time.time or EditorApplication.timeSinceStartup</param>
		public void Pause (float t)
		{
			_pause = true;	
			_timeLast = t;
			_isPlaying = false;
		}


		/// <summary>
		/// Unpause.
		/// </summary>
		/// <param name="t">T is global time Time.time or EditorApplication.timeSinceStartup</param>
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


		/// <summary>
		/// Unpause.
		/// </summary>
		public void UnPause ()
		{
			UnPause (Time.time);

		}

		/// <summary>
		/// Restart the specified t.
		/// </summary>
		/// <param name="t">T.</param>
		public void Restart (double t)
		{
			Stop ();
			PlayBy (t);
		}
		
		public void Restart ()
		{
			Stop ();
			Play ();
		}

		public void Stop ()
		{
			_stop = true;

			_isPlaying = false;


			foreach (SequenceChannel channel in channels)
				foreach (SequenceNode node in channel.nodes) {
					if (node.isRunning)
						node.Stop ();

										
				}	


			//Debug.Log ("Sequence>Stop " + _isPlaying);

						
		}

		public void Record ()
		{
			_isRecording = true;

			__duration = calcDuration ();

			Stop ();
		}

		public void StopRecording ()
		{

			_isRecording = false;



						

		}

		/// <summary>
		/// Value of Final Node's EndTime in local sequence time space
		/// </summary>
		/// <value>The end time.</value>
		float calcDuration ()
		{
						
						
			double timeEnd = 0;

			SequenceNode nodeLast;
			foreach (SequenceChannel channel  in this.channels) {
								
				nodeLast = channel.nodes [channel.nodes.Count - 1];
				if (timeEnd < nodeLast.timeStart + nodeLast.duration)
					timeEnd = nodeLast.timeStart + nodeLast.duration;

			}
			
						
			
			return (float)(timeEnd - timeStart);
			
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