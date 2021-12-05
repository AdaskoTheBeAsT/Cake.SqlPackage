using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.SqlPackage
{
    /// <summary>
    /// Contains settings used by SqlPackage runners.
    /// </summary>
    /// <seealso cref="ToolSettings" />
    public class SqlPackageSettings : ToolSettings
    {
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        protected SqlPackageAction _action;
#pragma warning restore IDE1006

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPackageSettings"/> class.
        /// </summary>
        public SqlPackageSettings()
        {
            Properties = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Variables = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the action used to execute SqlPackage.
        /// </summary>
        public SqlPackageAction Action => _action;

        /// <summary>
        /// Gets or sets the file path where the output files are generated.
        /// </summary>
        public FilePath? OutputPath { get; set; }

        /// <summary>
        /// Gets or sets if sqlpackage.exe should overwrite existing files. Specifying false causes sqlpackage.exe to abort action if an existing file is encountered.
        /// </summary>
        public bool? OverwriteFiles { get; set; }

        /// <summary>
        /// Gets or sets the file path to a DAC Publish Profile. The profile defines a collection of properties and variables to use when generating outputs.
        /// </summary>
        public FilePath? Profile { get; set; }

        /// <summary>
        /// Gets or sets whether detailed feedback is suppressed.
        /// </summary>
        public bool? Quiet { get; set; }

        /// <summary>
        /// Gets or sets a valid SQL Server/Azure connection string to the source database. If this parameter is specified it shall be used exclusively of all other source parameters.
        /// </summary>
        public string? SourceConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the source database.
        /// </summary>
        public string? SourceDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets if SQL encryption should be used for the source database connection.
        /// </summary>
        public bool? SourceEncryptConnection { get; set; }

        /// <summary>
        /// Gets or sets a source file to be used as the source of action instead of a database. If this parameter is used, no other source parameter shall be valid. (short form /sf).
        /// </summary>
        public FilePath? SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the password to use to access the source database for SQL Server auth scenarios.
        /// </summary>
        public string? SourcePassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the server hosting the source database.
        /// </summary>
        public string? SourceServerName { get; set; }

        /// <summary>
        /// Gets or sets the timeout for establishing a connection to the source database in seconds.
        /// </summary>
        public int SourceTimeout { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether to use SSL to encrypt the source database connection and bypass walking the certificate chain to validate trust.
        /// </summary>
        public bool? SourceTrustServerCertificate { get; set; }

        /// <summary>
        /// Gets or sets the SQL Server user to use to access the source database for SQL Server auth scenarios.
        /// </summary>
        public string? SourceUser { get; set; }

        /// <summary>
        /// Gets or sets a valid SQL Server/Azure connection string to the target database. If this parameter is specified it shall be used exclusively of all other target parameters.
        /// </summary>
        public string? TargetConnectionString { get; set; }

        /// <summary>
        /// Gets or sets an override for the name of the database that is the target of sqlpackage.exe Action.
        /// </summary>
        public string? TargetDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets if SQL encryption should be used for the target database connection.
        /// </summary>
        public bool? TargetEncryptConnection { get; set; }

        /// <summary>
        /// Gets or sets a target file (i.e., a .dacpac files) to be used as the target of action instead of a database. If this parameter is used, no other target parameter shall be valid. This parameter shall be invalid for actions that only support database targets.
        /// </summary>
        public FilePath? TargetFile { get; set; }

        /// <summary>
        /// Gets or sets the password to use to access the target database for SQL Server auth scenarios.
        /// </summary>
        public string? TargetPassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the server hosting the target database.
        /// </summary>
        public string? TargetServerName { get; set; }

        /// <summary>
        /// Gets or sets the timeout for establishing a connection to the target database in seconds.
        /// </summary>
        public int TargetTimeout { get; set; }

        /// <summary>
        /// Gets or sets whether to use SSL to encrypt the target database connection and bypass walking the certificate chain to validate trust.
        /// </summary>
        public bool? TargetTrustServerCertificate { get; set; }

        /// <summary>
        /// Gets or sets the SQL Server user to use to access the target database for SQL Server auth scenarios.
        /// </summary>
        public string? TargetUser { get; set; }

        /// <summary>
        /// Gets or sets a name value pair for an action specific property; {PropertyName}={Value}. Refer to the help for a specific action to see that action's property names.
        /// </summary>
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Gets or sets the list of valid SQLCMD variables for rhe DACPAC file.
        /// </summary>
        public IDictionary<string, string> Variables { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether Universal Authentication should be used.
        /// </summary>
        public bool? UniversalAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        public string? TenantId { get; set; }
    }
}