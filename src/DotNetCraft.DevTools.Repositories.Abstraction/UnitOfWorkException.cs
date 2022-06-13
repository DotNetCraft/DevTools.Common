using System;

namespace DotNetCraft.DevTools.Repositories.Abstraction
{
    public class UnitOfWorkException: Exception
    {
        public UnitOfWorkException(string msg): base(msg)
        {
        }
    }
}
