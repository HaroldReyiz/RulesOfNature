namespace ObserverPattern
{
	public interface IObserver
	{
		void OnNotify( int param );
	}
	public interface IObservable
	{
		void Subscribe( IObserver observer );
		void Unsubscribe( IObserver observer );
	}
}
