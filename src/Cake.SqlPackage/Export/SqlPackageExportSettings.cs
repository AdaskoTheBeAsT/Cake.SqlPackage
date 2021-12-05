﻿namespace Cake.SqlPackage
{
    /// <summary>
    /// Contains settings used by <see cref="SqlPackageExportSettings" />.
    /// </summary>
    public class SqlPackageExportSettings : SqlPackageSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPackageExportSettings"/> class.
        /// </summary>
        public SqlPackageExportSettings()
        {
            _action = SqlPackageAction.Export;
        }
    }
}
