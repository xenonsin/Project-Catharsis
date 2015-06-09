using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Catharsis
{
    public class GameInputEvent
    {
        public static int NONE = 0;

        //Movement
        public static int MOVE_RIGHT = 1;
        public static int MOVE_LEFT = 2;
        public static int MOVE_FORWARD = 4;
        public static int MOVE_BACKWARD = 8;

        //Mouse
        public static int ATTACK = 16;
        public static int SPECIAL = 32;

        //Skills
        public static int SKILL_ONE = 64;
        public static int SKILL_TWO = 128;
        public static int SKILL_THREE = 256;
        public static int SKILL_FOUR = 512;
        public static int SKILL_FIVE = 1024;

    }

}