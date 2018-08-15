namespace Diplomata.Interfaces
{
  /// <summary>
  /// The interface to implement the choice loop events
  /// </summary>
  public interface IChoice
  {
    void OnStart();
    // void OnUpdate();
    // void OnEnd();
    // void OnChange();
    void OnChoose();
  }
}
