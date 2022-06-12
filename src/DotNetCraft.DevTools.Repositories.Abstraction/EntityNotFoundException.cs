using System;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public class EntityNotFoundException: Exception
    {
        public EntityNotFoundException(string msg): base(msg)
        {
        }
    }
}
