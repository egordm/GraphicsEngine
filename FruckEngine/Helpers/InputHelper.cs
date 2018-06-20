using System.Collections.Generic;
using OpenTK.Input;

namespace FruckEngine.Helpers {
    public static class InputHelper {
        private static Dictionary<Key, bool> ButtonStates = new Dictionary<Key, bool>();
        private static Dictionary<Key, bool> ClickListener = new Dictionary<Key, bool>();

        public static void CreateClickListener(Key key) {
            if(!ButtonStates.ContainsKey(key)) ButtonStates.Add(key, false);
            if(!ClickListener.ContainsKey(key)) ClickListener.Add(key, false);
        }

        public static void Update(KeyboardState state) {
            var keys = new List<Key>(ClickListener.Keys);
            foreach (var key in keys) {
                if (state[key]) {
                    ClickListener[key] = !ButtonStates[key];
                    ButtonStates[key] = true;
                } else {
                    ButtonStates[key] = false;
                    ClickListener[key] = false;
                }
            }
        }

        public static bool IsClicked(Key key) {
            return ClickListener[key];
        }
        
    }
}