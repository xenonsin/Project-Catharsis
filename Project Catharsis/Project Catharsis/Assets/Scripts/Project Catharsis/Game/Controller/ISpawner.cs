//API for a device that spawns things.

using System;

namespace Catharsis
{
    public interface ISpawner
    {
        //Start spawning
        void Start();

        //Stop spawning
        void Stop();
    }
}

