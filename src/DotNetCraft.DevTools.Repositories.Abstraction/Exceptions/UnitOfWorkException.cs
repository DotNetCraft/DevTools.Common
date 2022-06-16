using System;

namespace DotNetCraft.DevTools.Repositories.Abstraction.Exceptions
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException(string msg) : base(msg)
        {
        }
    }
}
