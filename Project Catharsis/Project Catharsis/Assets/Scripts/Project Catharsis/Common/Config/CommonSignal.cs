//This contains signals that are dispatched between multiple contexts.

using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace Catharsis
{
    //First Signal Being Called
    public class StartSignal : Signal{}

    //Input
    public class GameInputSignal : Signal<int> { }

    //Game
    public class GameStartSignal : Signal { }
    public class GameEndSignal : Signal { }
    //public class UpdateScoreSignal : Signal { }


}