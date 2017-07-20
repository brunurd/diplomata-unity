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
        private int equipped = -1;

        public bool IsEquipped(int id) {
            if (id == equipped) {
                return true;
            }

            else {
                return false;
            }
        }

        public void Equip(int id) {
            for (int i = 0; i < items.Length; i++) {
                if (items[i].id == id) {
                    equipped = id;
                    break;
                }

                else if (i == items.Length - 1) {
                    equipped = -1;
                }
            }
        }

        public void Equip(string name, string language = "English") {

            foreach (Item item in items) {
                DictLang itemName = DictHandler.ContainsKey(item.name, language);

                if (itemName.value == name && itemName != null) {
                    Equip(item.id);
                    break;
                }
            }

            if (equipped == -1) {
                Debug.LogError("Cannot find the item \"" + name + "\" in " + language +
                    " in the inventory.");
            }
        }

    }

}