using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemUtil
{
    public static class MainSystemUtil
    {
        public enum InputTypes
        {
            Hiragana,
            Katakana,
            Alphabet,
            Number
        };

        public enum SendTarget
        {
            Chat,
            Window
        };

        public enum TrackDevice
        {
            HMD,
            Controller
        };
    }
}
