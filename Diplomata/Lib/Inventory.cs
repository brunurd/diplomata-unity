using UnityEngine;

namespace DiplomataLib {

    [System.Serializable]
    public class Item {
        public int id;
        public DictLang[] name;
        public DictLang[] description;
        public string imagePath = string.Empty;

        [System.NonSerialized]
        public Texture2D image;

        [System.NonSerialized]
        public bool have;

        [System.NonSerialized]
        public bool discarded;

        public Item(int id) {
            this.id = id;

            foreach (Language language in Diplomata.preferences.languages) {
                name = ArrayHandler.Add(name, new DictLang(language.name, "[ Edit to change this name ]"));
                description = ArrayHandler.Add(description, new DictLang(language.name, ""));
            }
        }

        public static Item Find(Item[] items, int itemId) {

            foreach (Item item in items) {
                if (item.id == itemId) {
                    return item;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class Inventory {
        public Item[] items = new Item[0];
    }

}