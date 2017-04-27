using System;
using System.Collections.Generic;

namespace DiplomataLib {
    
    public class Events {
        protected List<Event> events = new List<Event>();
        
        public struct Event {
            public Action action1;
            public Func<bool> action2;

            public Event(Action action1, Func<bool> action2) {
                this.action1 = action1;
                this.action2 = action2;
            }
        }

        public void Add(Action action) {
            events.Add(new Event(action, null));
        }

        public void Add(Func<bool> action) {
            events.Add(new Event(null, action));
        }

        public int Count() {
            return events.Count;
        }

        public void Invoke() {
            foreach (Event e in events) {
                if (e.action1 != null) {
                    e.action1();
                }
            }
        }

        public void Invoke(int index) {
            events[index].action1();
        }
        
        public bool Check(int index) {
            return events[index].action2();
        }

        public bool CheckAll() {
            foreach (Event e in events) {
                if (e.action2 != null) {
                    if(!e.action2()) {
                        return false;
                    }
                }
            }

            return true;
        }
    }

}
