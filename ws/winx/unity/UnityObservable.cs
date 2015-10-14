#if UNI_RX

using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using UniRx.InternalUtil;



using UnityEngine;

namespace ws.winx.unity{

	using UnityEngine.Events;
	using System;
	using System.Collections.Generic;
	using UniRx;
	using UniRx.InternalUtil;
	using UnityEngine;
	using System.Reflection;
	using UniRx.Triggers;
	
	
	public static class  ObservableExtensionsEx{
		public static IDisposable DisposeWith(this IDisposable disposable, GameObject gameObject)
		{
			return gameObject.OnDestroyAsObservable().First().Subscribe(p => disposable.Dispose());
		}
		
		public static IDisposable AddListener (this IObservable<GameObject> source,UnityAction call)
		{
			
			return ((UnityObservable)source).Subscribe (call);
			
			
		}
		
		public static IDisposable AddListener<T1> (this IObservable<Tuple<T1,GameObject>> source,UnityAction<T1> call)
		{
			
			return ((UnityObservable<T1>)source).Subscribe (call);
			
		}
		
		
		public static IDisposable AddListener<T1,T2> (this IObservable<Tuple<T1,T2,GameObject>> source,UnityAction<T1,T2> call)
		{
			return ((UnityObservable<T1,T2>)source).Subscribe (call);
		}
		
		public static IDisposable AddListener<T1,T2,T3> (this IObservable<Tuple<T1,T2,T3,GameObject>> source,UnityAction<T1,T2,T3> call)
		{
			return ((UnityObservable<T1,T2,T3>)source).Subscribe (call);
			
		}
		
		
		public static IDisposable AddListener<T1,T2,T3,T4> (this IObservable<Tuple<T1,T2,T3,T4,GameObject>> source,UnityAction<T1,T2,T3,T4> call)
		{
			return ((UnityObservable<T1,T2,T3,T4>)source).Subscribe (call);
			
		}
		
		
	}
	
	[Serializable]
	public class UnityObservable:UnityEvent,IObservable<GameObject>
	{
		
		private Dictionary<UnityAction,IDisposable> __disposables;
		private Subject<GameObject> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable __disposer;
		
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<UnityAction, IDisposable> ();
			
			subject = new Subject<GameObject> ();
			
			__disposer = new CompositeDisposable ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction call)
		{
			Subscribe (call);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction call= System.Delegate.CreateDelegate (typeof(UnityAction), persistentTarget, methodInfo) as UnityAction;
					
					this.AddListener(call);
					//subject.Where(trg=>trg==null || trg==(call.Target as MonoBehaviour).gameObject).Subscribe<GameObject> (x => call ())
					
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (target);
			}
			
		}
		
		public new void Invoke ()
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext(null);
			}
			
		}
		
		public new void RemoveListener (UnityAction call)
		{
			if (__disposables.ContainsKey (call)) {
				IDisposable disposable=__disposables [call];
				__disposables.Remove(call);
				
				disposable.Dispose();
				
				Debug.Log("Observer "+call.Method.Name+" on target "+call.Target+" Removed");
				
			}
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			foreach (var d in __disposables)
				d.Value.Dispose ();
			
			__disposables.Clear ();
			
			__disposer.Dispose ();
		}
		
		
		
		public IDisposable Subscribe (UnityAction call)
		{
			IDisposable disposable = subject.Where(tgt=>tgt==null || tgt==(call.Target as MonoBehaviour).gameObject).Subscribe<GameObject> (x => call ());
			__disposables [call] = disposable;
			return disposable;
		}
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<GameObject> observer)
		{
			Debug.LogWarning ("Use Subscribe (UnityAction call)");
			return subject.Subscribe<GameObject> (observer.OnNext).AddTo(__disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	
	
	[Serializable]
	public abstract class UnityObservable<T1,T2,T3,T4>:UnityEvent<T1,T2,T3,T4>,IObservable<Tuple<T1,T2,T3,T4,GameObject>>
	{
		private Dictionary<UnityAction<T1,T2,T3,T4>,IDisposable> __disposables;
		private Subject<Tuple<T1,T2,T3,T4,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable __disposer;
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<UnityAction<T1,T2,T3,T4>, IDisposable> ();
			
			subject = new Subject<Tuple<T1,T2,T3,T4,GameObject>> ();
			
			__disposer = new CompositeDisposable ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1,T2,T3,T4> call)
		{
			Subscribe (call);
		}
		
		
		
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1,T2,T3,T4> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1,T2,T3,T4>), persistentTarget, methodInfo) as UnityAction<T1,T2,T3,T4>;
					
					this.AddListener(call);
					//subject.Where(trg=>trg.Item5==null || trg.Item5==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,T4,GameObject>> (x => call (x.Item1,x.Item2,x.Item3,x.Item4));
					
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, arg4, target));
			}
			
		}
		
		public new void Invoke (T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, arg4,(GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2,T3,T4> call)
		{
			if (__disposables.ContainsKey (call)) {
				IDisposable disposable=__disposables [call];
				__disposables.Remove(call);
				
				disposable.Dispose();
				
				Debug.Log("Observer "+call.Method.Name+" on target "+call.Target+" Removed");
				
			}
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			foreach (var d in __disposables)
				d.Value.Dispose ();
			
			
			__disposables.Clear ();
			
		}
		
		public IDisposable Subscribe (UnityAction<T1,T2,T3,T4> call)
		{
			//convert UnityAction<T1,T2,T3,T4> to Action<T1,T2,T3,T4>
			IDisposable disposable = subject.Where(tgt=>tgt.Item5==null || tgt.Item5==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,T4,GameObject>> (x => call (x.Item1,x.Item2,x.Item3,x.Item4));
			
			__disposables [call] = disposable;
			
			return disposable;
			
		}
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,T2,T3,T4,GameObject>> observer)
		{
			Debug.LogWarning ("Use Subscribe (UnityAction<T1,T2,T3,T4> call)");
			//Action<Tuple<T1,T2,T3,T4,GameObject>> action = observer.OnNext as Action<Tuple<T1,T2,T3,T4,GameObject>>;
			
			
			//UnityAction<Tuple<T1,T2,T3,T4,GameObject>> call=Delegate.CreateDelegate(typeof(UnityAction<Tuple<T1,T2,T3,T4,GameObject>>),action.Target,action.Method) as UnityAction<Tuple<T1,T2,T3,T4,GameObject>>;
			
			return subject.Subscribe(observer.OnNext).AddTo(__disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	[Serializable]
	public abstract class UnityObservable<T1,T2,T3>:UnityEvent<T1,T2,T3>,IObservable<Tuple<T1,T2,T3,GameObject>>
	{
		
		private Dictionary<UnityAction<T1,T2,T3>,IDisposable> __disposables;
		private Subject<Tuple<T1,T2,T3,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable __disposer;
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<UnityAction<T1,T2,T3>, IDisposable> ();
			
			subject = new Subject<Tuple<T1,T2,T3,GameObject>> ();
			
			__disposer=new CompositeDisposable();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1,T2,T3> call)
		{
			Subscribe (call);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1,T2,T3> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1,T2,T3>), persistentTarget, methodInfo) as UnityAction<T1,T2,T3>;
					//subject.Where(trg=>trg.Item4==null || trg.Item4==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,GameObject>> (x => call (x.Item1,x.Item2,x.Item3));
					AddListener(call);
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1, T2 arg2, T3 arg3)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, target));
			}
			
		}
		
		public new void Invoke (T1 arg1, T2 arg2, T3 arg3)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1, arg2, arg3, (GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2,T3> call)
		{
			if (__disposables.ContainsKey (call)) {
				IDisposable disposable=__disposables [call];
				__disposables.Remove(call);
				
				disposable.Dispose();
				
				Debug.Log("Observer "+call.Method.Name+" on target "+call.Target+" Removed");
				
			}
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			foreach (var d in __disposables)
				d.Value.Dispose ();
			
			__disposables.Clear ();
			
		}
		
		public IDisposable Subscribe (UnityAction<T1,T2,T3> call)
		{
			
			//convert UnityAction<T1,T2,T3> to Action<T1,T2,T3>
			IDisposable disposable = subject.Where(tgt=>tgt.Item4==null || tgt.Item4==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,T3,GameObject>> (x => call (x.Item1,x.Item2,x.Item3));
			
			__disposables [call] = disposable;
			
			return disposable;
			
		}
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,T2,T3,GameObject>> observer)
		{
			Debug.LogWarning ("Use Subscribe (UnityAction<T1,T2,T3> call)");
			return subject.Subscribe<Tuple<T1,T2,T3,GameObject>> (observer.OnNext).AddTo(__disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	[Serializable]
	public abstract class UnityObservable<T1,T2>:UnityEvent<T1,T2>,IObservable<Tuple<T1,T2,GameObject>>
	{
		
		
		
		
		private Dictionary<UnityAction<T1,T2>,IDisposable> __disposables;
		
		
		private Subject<Tuple<T1,T2,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		private CompositeDisposable __disposer;
		
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<UnityAction<T1,T2>, IDisposable> ();
			
			subject = new Subject<Tuple<T1,T2,GameObject>> ();
			
			__disposer = new CompositeDisposable ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1,T2> call)
		{
			Subscribe (call);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1,T2> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1,T2>), persistentTarget, methodInfo) as UnityAction<T1,T2>;
					//subject.Where(trg=>trg.Item3==null || trg.Item3==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,GameObject>> (x => call (x.Item1,x.Item2));
					AddListener(call);
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1, T2 arg2)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, arg2, target));
			}
			
		}
		
		public new void Invoke (T1 arg1, T2 arg2)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1, arg2, (GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1,T2> call)
		{
			if (__disposables.ContainsKey (call)) {
				IDisposable disposable=__disposables [call];
				__disposables.Remove(call);
				
				disposable.Dispose();
				
				Debug.Log("Observer "+call.Method.Name+" on target "+call.Target+" Removed");
				
			}
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			
			foreach (var d in __disposables)
				d.Value.Dispose ();
			
			__disposables.Clear ();
			
			__disposer.Dispose ();
			
		}
		
		public IDisposable Subscribe (UnityAction<T1,T2> call)
		{
			
			IDisposable disposable= subject.Where(tgt=>tgt.Item3==null || tgt.Item3==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,T2,GameObject>> (x => call (x.Item1,x.Item2));
			
			__disposables [call] = disposable;
			
			return disposable;
			
		}
		
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,T2,GameObject>> observer)
		{
			Debug.LogWarning ("Use Subscribe (UnityAction<T1,T2> call)");
			return subject.Subscribe<Tuple<T1,T2,GameObject>> (observer.OnNext).AddTo(__disposer).AddTo(__disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	
	
	[Serializable]
	public abstract class UnityObservable<T1>:UnityEvent<T1>,IObservable<Tuple<T1,GameObject>>
	{
		
		
		
		
		private Dictionary<UnityAction<T1>,IDisposable> __disposables;
		
		
		private Subject<Tuple<T1,GameObject>> subject;
		private bool persistentTargetsAreSubscribed;
		
		private CompositeDisposable __disposer;
		
		
		//
		// Constructors
		//
		public UnityObservable ():base()
		{
			
			__disposables = new Dictionary<UnityAction<T1>, IDisposable> ();
			
			subject = new Subject<Tuple<T1,GameObject>> ();
			
			__disposer = new CompositeDisposable ();
			
			//num of persistent is 0 here, so why SubscibeToPersistent is called in Invoke
		}
		
		public new void AddListener (UnityAction<T1> call)
		{
			Subscribe (call);
		}
		
		void SubscibeToPersistent ()
		{
			//check is some listners are subscribed thru editor
			if (!persistentTargetsAreSubscribed && this.GetPersistentEventCount () > 0) {
				//subscribe persistent obeservers to observer
				int persistentTargetsNumber = this.GetPersistentEventCount ();
				MethodInfo methodInfo;
				UnityEngine.Object persistentTarget;
				
				
				
				for (int i=0; i<persistentTargetsNumber; i++) {
					
					persistentTarget = this.GetPersistentTarget (i);
					methodInfo = persistentTarget.GetType ().GetMethod (this.GetPersistentMethodName (i));
					UnityAction<T1> call= System.Delegate.CreateDelegate (typeof(UnityAction<T1>), persistentTarget, methodInfo) as UnityAction<T1>;
					//subject.Where(trg=>trg.Item2==null || trg.Item2==(call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,GameObject>> (x => call (x.Item1));
					AddListener(call);
				}
				
				persistentTargetsAreSubscribed = true;
			}
			
			
			
		}
		
		public void InvokeOnTarget (GameObject target, T1 arg1)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				subject.OnNext (Tuple.Create (arg1, target));
			}
			
		}
		
		public new void Invoke (T1 arg1)
		{
			
			SubscibeToPersistent ();
			
			if (subject.HasObservers) {
				
				subject.OnNext (Tuple.Create (arg1,(GameObject)null));
			}
			
		}
		
		public new void RemoveListener (UnityAction<T1> call)
		{
			if (__disposables.ContainsKey (call)) {
				IDisposable disposable=__disposables [call];
				__disposables.Remove(call);
				
				disposable.Dispose();
				
				Debug.Log("Observer "+call.Method.Name+" on target "+call.Target+" Removed");
				
			}
			
			
		}
		
		public new void RemoveAllListeners ()
		{
			foreach (var d in __disposables)
				d.Value.Dispose ();
			
			__disposables.Clear ();
			
			//I hope disposer check if something was manually disposed before
			__disposer.Dispose ();
			
		}
		
		public IDisposable Subscribe (UnityAction<T1> call)
		{
			
			IDisposable disposable = subject.Where (tgt => tgt.Item2 == null || tgt.Item2 == (call.Target as MonoBehaviour).gameObject).Subscribe<Tuple<T1,GameObject>> (x => call (x.Item1));
			
			__disposables[call] = disposable;
			
			return disposable;
			
		}
		
		#region IObservable implementation
		public IDisposable Subscribe (IObserver<Tuple<T1,GameObject>> observer)
		{
			Debug.LogWarning ("Use Subscribe (UnityAction<T1> call)");
			return subject.Subscribe<Tuple<T1,GameObject>> (observer.OnNext).AddTo(__disposer);
		}
		#endregion	
		
		
		
		
		
		
		
		
	}
	
	
	
	
	
	
	





}
#endif
