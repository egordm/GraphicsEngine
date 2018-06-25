using System.Collections.Generic;
using OpenTK.Input;

namespace FruckEngine.Helpers {
    /// <summary>
    /// Input helper for more smooth input handling
    /// </summary>
    public static class InputHelper {
        private static Dictionary<Key, bool> ButtonStates = new Dictionary<Key, bool>();
        private static Dictionary<Key, bool> ClickListener = new Dictionary<Key, bool>();

        /// <summary>
        /// Click listener to check if button is clicken only once
        /// </summary>
        /// <param name="key"></param>
        public static void CreateClickListener(Key key) {
            if(!ButtonStates.ContainsKey(key)) ButtonStates.Add(key, false);
            if(!ClickListener.ContainsKey(key)) ClickListener.Add(key, false);
        }

        /// <summary>
        /// Update the different listeners
        /// </summary>
        /// <param name="state"></param>
        public static void Update(KeyboardState state) {
            // Update click listeners
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