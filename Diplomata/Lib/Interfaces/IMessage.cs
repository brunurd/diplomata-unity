namespace Diplomata.Interfaces
{
  /// <summary>
  /// The interface that implements any message box (text box) loop events
  /// </summary>
  public interface IMessage
  {
    void OnSubStart();
    void OnDubStart();
    void OnEnd();
  }
}
