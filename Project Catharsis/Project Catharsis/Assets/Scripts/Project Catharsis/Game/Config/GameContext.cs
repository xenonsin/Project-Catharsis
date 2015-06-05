
using strange.extensions.context.impl;
using strange.extensions.pool.api;
using strange.extensions.pool.impl;
using UnityEngine;
using System.Collections;

namespace Catharsis
{
    public class GameContext : SignalContext
    {
        public GameContext(MonoBehaviour contextView) : base(contextView)
        {
        }

        //Here we map bindings to fulfill dependencies

        protected override void mapBindings()
        {
            base.mapBindings();

            //Mapping Input
            injectionBinder.Bind<IInput>().To<KeyboardInput>().ToSingleton();
          

            //injectionBinder.Bind<ISpawner>().To<ObstacleSpawner>().ToSingleton();

            //Pools
            //injectionBinder.Bind<IPool<GameObject>>()
            //    .To<Pool<GameObject>>()
            //    .ToSingleton()
            //    .ToName(GameElement.OBSTACLE_POOL);

            //Signals (That are not bound to commands)
            injectionBinder.Bind<GameStartedSignal>().ToSingleton();
            injectionBinder.Bind<GameInputSignal>().ToSingleton();

            //Commands
            //All Commands get mapped to a Signal that executes them.
            if (Context.firstContext == this)
            {
                //Here we bind the StartSignal to the game start command
                //which is found in the Controllers folder
                commandBinder.Bind<StartSignal>()
                    .To<GameStartCommand>()
                    .Once();
            }
           // commandBinder.Bind<DestroyPlayerSignal>().To<DestroyPlayerCommand>().Pooled();
            //commandBinder.Bind<CreateObstacleSignal>().To<CreateObstacleCommand>().Pooled();


            //Mediation
            //mediationBinder.Bind<PlayerView>().To<PlayerMediator>();
           // mediationBinder.Bind<ObstacleView>().To<ObstacleMediator>();
        }

        protected override void postBindings()
        {

            //IPool<GameObject> obstaclePool = injectionBinder.GetInstance<IPool<GameObject>>(GameElement.OBSTACLE_POOL);
            //obstaclePool.instanceProvider = new ResourceInstanceProvider("Obstacle", LayerMask.NameToLayer("obstacle"));
            //obstaclePool.inflationType = PoolInflationType.INCREMENT;
            base.postBindings();
        }
    }

}