public interface IObserver
{
    public void OnNotify(ISubject subject, Actions action);
}

