// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.Globalization;
using IGeekfan.Localization.FreeSql.Models;
using Microsoft.Extensions.Localization;

namespace IGeekfan.Localization.FreeSql
{
    public class FreeSqlStringLocalizer<T> : IStringLocalizer<T>
    {
        private readonly IFreeSql _db;

        public FreeSqlStringLocalizer(IFreeSql db)
        {
            _db = db;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new FreeSqlStringLocalizer(_db);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
        {
            return _db.Select<LocalResource>()
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name)
                .ToList(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string GetString(string name)
        {
            return _db.Select<LocalResource>()
                .Include(r => r.Culture)
                .Where(r => r.Culture.Name == CultureInfo.CurrentCulture.Name&&r.Key==name)
                .First()?.Value;
        }
    }
}
