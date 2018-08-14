namespace Diplomata.Interfaces
{
  /// <summary>
  /// The interface that implements the events loops to the complete dialogue
  /// </summary>
  public interface ITalk
  {
    void OnStart();
    void OnEnd();
  }
}
