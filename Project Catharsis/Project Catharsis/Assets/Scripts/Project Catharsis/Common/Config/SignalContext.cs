using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
    //Creating this SignalContext allows us to use Signals instead of Commands
    //as the default implementation for strange.
    public class SignalContext : MVCSContext
    {
        //The Context views which inherits from this is placed as a component of a gameObject
        //so this constructor allows us to grab the gameObject that this is attached to
        public SignalContext(MonoBehaviour contextView) : base(contextView)
        {
            
        }

        //So this is where we unbind the default command binder and replace it with the
        //Signals implementation.
        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }

        //This launches the signal to start the game. This is the very first signal which is called 
        //and when the game runs. This requires a StartSignal : Signal{} declaration in a commonsignals file
        public override void Launch()
        {
            base.Launch();
            StartSignal startSignal = (StartSignal) injectionBinder.GetInstance<StartSignal>();
            startSignal.Dispatch();
        }

        //Ok so this scans the namespace specified in the string
        //then automatically creates bindings within that namespace.
        protected override void mapBindings()
        {
            base.mapBindings();
            implicitBinder.ScanForAnnotatedClasses(new string[] { "Catharsis" });
        }
    }
}