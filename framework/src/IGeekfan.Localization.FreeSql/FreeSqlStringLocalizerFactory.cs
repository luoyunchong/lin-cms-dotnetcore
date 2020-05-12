// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using IGeekFan.Localization.FreeSql.Models;

namespace IGeekFan.Localization.FreeSql
{
    public class FreeSqlStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IFreeSql _db;

        public FreeSqlStringLocalizerFactory(IFreeSql db)
        {
            this._db = db;

        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new FreeSqlStringLocalizer(_db);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new FreeSqlStringLocalizer(_db);
        }
    }
}
