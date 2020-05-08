// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    public class ProviderOptions
    {
        public const string DefaultSchema = "cap";

        /// <summary>
        /// Gets or sets the table name prefix to use when creating database objects.
        /// </summary>
        public string TableNamePrefix { get; set; } = DefaultSchema;

        /// <summary>
        /// Data version
        /// </summary>
        internal string Version { get; set; } = "v1";
    }
}