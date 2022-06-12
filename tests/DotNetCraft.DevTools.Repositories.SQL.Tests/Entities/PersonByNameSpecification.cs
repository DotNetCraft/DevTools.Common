﻿using System;
using System.Linq.Expressions;
using DotNetCraft.DevTools.Repositories.Abstraction;

namespace DotNetCraft.DevTools.Repositories.SQL.Tests.Entities
{
    internal class PersonByNameSpecification: IRepositorySpecification<Person>
    {
        private readonly string _name;

        public PersonByNameSpecification(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            _name = name;
        }

        public Expression<Func<Person, bool>> IsSatisfy()
        {
            return x => x.Name == _name;
        }
    }
}
