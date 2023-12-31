﻿using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Implementation
{
    public class AdoContextGroupFactory : IAdoContextGroupFactory
    {
        private readonly IAdoContextOptionsRegistry adoContextOptionsRegistry;

        public AdoContextGroupFactory(IAdoContextOptionsRegistry adoContextOptionsRegistry = null)
        {
            this.adoContextOptionsRegistry = adoContextOptionsRegistry;
        }

        public IAdoContextGroup CreateContextGroup(AdoScopeOptions adoScopeOptions)
        {
            return new AdoContextGroup(adoContextOptionsRegistry, adoScopeOptions);
        }
    }
}
