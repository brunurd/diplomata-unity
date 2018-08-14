namespace Diplomata.Interfaces
{
  /// <summary>
  /// The interface that implements any message box (text box) loop events
  /// </summary>
  public interface IMessage
  {
    void OnStart();
    void OnUpdate();
    void OnEnd();
    void OnEveryLetter();
  }
}
