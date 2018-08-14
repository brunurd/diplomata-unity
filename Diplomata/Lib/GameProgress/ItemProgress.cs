namespace DiplomataLib
{
  [Serializable]
  public class ItemProgress
  {
    public uint id;
    public bool have;
    public bool discarded;

    public ItemProgress() {}

    public ItemProgress(int id, bool have, bool discarded)
    {
      this.id = (uint) id;
      this.have = have;
      this.discarded = discarded;
    }
  }
}