using System;
using Microsoft.Extensions.Localization;

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
