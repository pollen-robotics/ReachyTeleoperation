using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TeleopReachy
{
    public static class ColorsManager
    {
        // Declare all colors used in the app
        public readonly static Color32 green = new Color32(32, 210, 9, 255);
        public readonly static Color32 orange = new Color32(227, 137, 68, 255);
        public readonly static Color32 red = new Color32(226, 67, 80, 255);
        public readonly static Color32 error_red = new Color32(226, 67, 80, 200);
        public readonly static Color32 white = new Color32(255, 255, 255, 255);
        public readonly static Color32 blue = new Color32(74, 174, 214, 255);
        public readonly static Color32 purple = new Color32(163, 74, 214, 255);
        public readonly static Color32 yellow = new Color32(140, 139, 37, 255);
        public readonly static Color32 black = new Color32(40, 40, 40, 255);
        public readonly static Color32 error_black = new Color32(40, 40, 40, 200);
        public readonly static Color32 black_transparent = new Color32(0, 0, 0, 220);
        public readonly static Color32 white_transparent = new Color32(255, 255, 255, 220);

        // Declare ColorBlocks to be used for buttons in order to indicate their activation state
        public static ColorBlock colorsActivated;
        public static ColorBlock colorsDeactivated;

        static ColorsManager()
        {
            colorsDeactivated = new ColorBlock();
            colorsDeactivated = ColorBlock.defaultColorBlock;
            colorsDeactivated.normalColor = new Color32(65, 65, 65, 255);
            colorsDeactivated.highlightedColor = new Color32(81, 81, 81, 255);
            colorsDeactivated.pressedColor = new Color32(60, 60, 60, 255);
            colorsDeactivated.selectedColor = new Color32(65, 65, 65, 255);
            colorsDeactivated.disabledColor = new Color32(200, 200, 200, 128);

            colorsActivated = new ColorBlock();
            colorsActivated = ColorBlock.defaultColorBlock;
            colorsActivated.normalColor = new Color32(119, 187, 185, 255);
            colorsActivated.highlightedColor = new Color32(139, 187, 185, 255);
            colorsActivated.pressedColor = new Color32(98, 176, 173, 255);
            colorsActivated.selectedColor = new Color32(119, 187, 185, 255);
            colorsActivated.disabledColor = new Color32(200, 200, 200, 128);
        }
    }
}
