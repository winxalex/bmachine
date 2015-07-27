using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VisualTween.Action;
using System.Linq;
using UnityEngine.Events;

namespace ws.winx.unity.sequence
{
		public class Sequence : MonoBehaviour
		{



				//events 
				public ws.winx.unity.sequence.SequenceEvent OnStart = new ws.winx.unity.sequence.SequenceEvent ();
				public ws.winx.unity.sequence.SequenceEvent OnEnd = new ws.winx.unity.sequence.SequenceEvent ();

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

				public SequenceNode selectedNode;
				public SequenceWrap wrap = SequenceWrap.ClampForever;
				public bool playOnStart = true;
				public int frameRate = 30;
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
			
								timeCurrent += t - _timeLast;//dt
								
								
							

								_timeLast = t;
					
								foreach (SequenceChannel channel in this.channels)
										foreach (SequenceNode node in channel.nodes) {
												node.UpdateNode (timeCurrent);		
										}
						}
				}

				void Update ()
				{
						if (_pause || _stop) {
								return;			
						}


					
						UpdateSequence (Time.time);
			

				}


				


				/// <summary>
				/// Play the sequence
				/// </summary>
				/// <param name="t">global "time"  (Time.time or EditorApplicaiton.timeSinceStartUp).</param>
				public void Play (double t)
				{
						StopRecording ();

				
						__duration = calcDuration ();

						//prevent
						timeCurrent = Mathf.Min ((float)timeCurrent, __duration);
						
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
				


					

						Debug.Log ("Sequence>Stop " + _isPlaying);

						
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