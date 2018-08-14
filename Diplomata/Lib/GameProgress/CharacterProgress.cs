namespace DiplomataLib
{
  [Serializable]
  public class CharacterProgress
  {
    public string name;
    public byte influence;
    public ContextProgress[] contexts = new ContextProgress[0];

    public CharacterProgress() {}

    public CharacterProgress(string name, byte influence)
    {
      this.name = name;
      this.influence = influence;
    }
  }
}