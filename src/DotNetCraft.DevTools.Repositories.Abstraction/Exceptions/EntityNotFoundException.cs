using System;

namespace DotNetCraft.DevTools.Repositories.Abstraction.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string msg) : base(msg)
        {
        }
    }
}
