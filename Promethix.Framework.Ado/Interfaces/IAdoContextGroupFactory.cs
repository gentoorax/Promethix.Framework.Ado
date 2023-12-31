﻿using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Implementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Interfaces
{
    public interface IAdoContextGroupFactory
    {
        IAdoContextGroup CreateContextGroup(AdoScopeOptions adoScopeOptions);
    }
}
