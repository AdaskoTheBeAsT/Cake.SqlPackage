using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

[assembly: InternalsVisibleTo("Cake.SqlPackage.UnitTests")]

namespace Cake.SqlPackage
{
    /// <summary>
    /// SqlPackage tool execution.
    /// </summary>
    /// <typeparam name="T"><see cref="SqlPackageSettings"/> type.</typeparam>
    internal abstract class SqlPackageRunner<T> : Tool<T>
        where T : SqlPackageSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPackageRunner{T}" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        protected SqlPackageRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
            : base(fileSystem, environment, processRunner, tools)
        {
            Environment = environment;
        }

        /// <summary>
        /// Gets or sets the Cake environment in context.
        /// </summary>
        public ICakeEnvironment Environment { get; set; }

        /// <summary>
        /// Runs SqlPackage with the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="processSettings">The ProcessSettings for SqlPackage.</param>
        public void Execute(T settings, ProcessSettings? processSettings = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Run(settings, BuildArguments(settings), processSettings, postAction: null);
        }

        protected abstract ProcessArgumentBuilder BuildArguments(T settings);

        /// <summary>
        /// Builds the common SqlPackage arguments.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><see cref="ProcessArgumentBuilder"/> instance.</returns>
        protected ProcessArgumentBuilder BuildSqlPackageArguments(T settings)
        {
            var properties = BuildProperties(settings);
            var builder = CopyArgumentsTo(properties);

            switch (settings.Action)
            {
                case SqlPackageAction.Publish:
                case SqlPackageAction.DeployReport:
                case SqlPackageAction.Script:
                    var variables = BuildVariables(settings);
                    CopyArgumentsTo(variables, builder);
                    break;

                case SqlPackageAction.Extract:
                case SqlPackageAction.DriftReport:
                case SqlPackageAction.Export:
                case SqlPackageAction.Import:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(settings), "Invalid Action.");
            }

            return builder;
        }

        /// <summary>
        /// Builds the <see cref="SqlPackageAction"/>.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><see cref="ProcessArgumentBuilder"/> instance.</returns>
        protected ProcessArgumentBuilder CreateBuilder(T settings)
        {
            var builder = new ProcessArgumentBuilder();

            builder.Append($"/Action:{settings.Action}");

            return builder;
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        /// <returns>The name of the tool.</returns>
        protected override string GetToolName()
        {
            return nameof(SqlPackage);
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>The tool executable name.</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "SqlPackage.exe", "sqlpackage" };
        }

        /// <summary>
        /// Copies the arguments to the specified builder.
        /// </summary>
        /// <param name="toCopy">To copy.</param>
        /// <param name="builder">The builder.</param>
        /// <returns><see cref="ProcessArgumentBuilder"/> instance.</returns>
        protected ProcessArgumentBuilder CopyArgumentsTo(ProcessArgumentBuilder toCopy, ProcessArgumentBuilder? builder = null)
        {
            if (builder == null)
            {
                builder = new ProcessArgumentBuilder();
            }

            toCopy.CopyTo(builder);

            return builder;
        }

        private ProcessArgumentBuilder BuildVariables(SqlPackageSettings settings)
        {
            var builder = new ProcessArgumentBuilder();

            if (settings.Variables.Count > 0)
            {
                foreach (var variable in settings.Variables)
                {
                    builder.Append($"/v:{variable.Key}={variable.Value}");
                }
            }

            return builder;
        }

        private ProcessArgumentBuilder BuildProperties(SqlPackageSettings settings)
        {
            var builder = new ProcessArgumentBuilder();

            if (settings.Properties.Count == 0)
            {
                return builder;
            }

            foreach (var property in settings.Properties)
            {
                builder.Append($"/p:{property.Key}={property.Value}");
            }

            return builder;
        }
    }
}