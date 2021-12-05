﻿using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using Xunit;

namespace Cake.SqlPackage.UnitTests
{
    public sealed class SqlPackageImportRunnerTests
    {
        public sealed class TheRunMethod
        {
            private readonly SqlPackageImportFixture _fixture;

            public TheRunMethod()
            {
                _fixture = new SqlPackageImportFixture();
            }

            [Fact]
            public void Should_Throw_If_Settings_Is_Null()
            {
                // Given
                _fixture.Settings = null!;

                // When
                var result = Record.Exception(() => _fixture.Run());

                // Then
                Assert.IsType<ArgumentNullException>(result);
            }

            [Fact]
            public void Should_Throw_If_Sql_Package_Runner_Was_Not_Found()
            {
                // Given
                _fixture.GivenDefaultToolDoNotExist();

                // When
                var result = Record.Exception(() => _fixture.Run());

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("SqlPackage: Could not locate executable.", result?.Message);
            }

            [Theory]
            [InlineData("/bin/tools/SqlPackage/SqlPackage.exe", "/bin/tools/SqlPackage/SqlPackage.exe")]
            [InlineData("./tools/SqlPackage/SqlPackage.exe", "/Working/tools/SqlPackage/SqlPackage.exe")]
            public void Should_Use_Sql_Package_Runner_From_Tool_Path_If_Provided(string toolPath, string expected)
            {
                // Given
                _fixture.Settings.ToolPath = toolPath;
                _fixture.GivenSettingsToolPathExist();

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal(expected, result.Path.FullPath);
            }

            [Fact]
            public void Should_Find_Sql_Package_Runner_If_Tool_Path_Not_Provided()
            {
                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Working/tools/SqlPackage.exe", result.Path.FullPath);
            }

            [Fact]
            public void Should_Set_Working_Directory()
            {
                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Working", result.Process.WorkingDirectory.FullPath);
            }

            [Fact]
            public void Should_Throw_If_Process_Was_Not_Started()
            {
                // Given
                _fixture.GivenProcessCannotStart();

                // When
                var result = Record.Exception(() => _fixture.Run());

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("SqlPackage: Process was not started.", result?.Message);
            }

            [Fact]
            public void Should_Throw_If_Process_Has_A_Non_Zero_Exit_Code()
            {
                // Given
                _fixture.GivenProcessExitsWithCode(1);

                // When
                var result = Record.Exception(() => _fixture.Run());

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("SqlPackage: Process returned an error (exit code 1).", result?.Message);
            }

            [Fact]
            public void Should_Add_Action_If_Provided()
            {
                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import", result.Args);
            }

            [Fact]
            public void Should_Add_OutputPath_If_Provided()
            {
                // Given
                _fixture.Settings.OutputPath = "./artifacts";

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /OutputPath:\"/Working/artifacts\"", result.Args);
            }

            [Fact]
            public void Should_Add_Overwrite_Files_If_Provided()
            {
                // Given
                _fixture.Settings.OverwriteFiles = true;

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /OverwriteFiles:True", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Profile_If_Provided()
            {
                // Given
                _fixture.Settings.Profile = "./profile.pubxml";

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import", result.Args);
            }

            [Fact]
            public void Should_Add_Quiet_If_Provided()
            {
                // Given
                _fixture.Settings.Quiet = true;

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /Quiet:True", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Quiet_If_Not_Provided()
            {
                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Source_Connection_String_If_Provided()
            {
                // Given
                _fixture.Settings.SourceConnectionString =
                    "Data Source=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Source_File_If_Source_Connection_Provided()
            {
                // Given
                _fixture.Settings.SourceConnectionString =
                    "Data Source=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/SourceFile:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Not_Add_Source_Properties_If_Source_Connection_Provided()
            {
                // Given
                _fixture.Settings.SourceConnectionString =
                    "Data Source=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/SourceFile:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceDatabaseName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceEncryptConnection:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourcePassword:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceServerName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceTimeout:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceTrustServerCertificate:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceUser:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Add_Source_File_If_Provided()
            {
                // Given
                _fixture.Settings.SourceFile = "./sqlpublishprofile.pubxml";

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /SourceFile:\"/Working/sqlpublishprofile.pubxml\"", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Source_Properties_If_Provided()
            {
                // Given
                _fixture.Settings.SourceConnectionString =
                    "Data Source=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/SourceConnectionString:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceDatabaseName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceEncryptConnection:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourcePassword:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceServerName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceTimeout:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceTrustServerCertificate:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/SourceUser:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Add_Target_Connection_String_If_Provided()
            {
                // Given
                _fixture.Settings.TargetConnectionString =
                    "Data Target=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /TargetConnectionString:\"Data Target=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True\"", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Target_File_If_Target_Connection_Provided()
            {
                // Given
                _fixture.Settings.TargetConnectionString =
                    "Data Target=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/TargetFile:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Not_Add_Target_Properties_If_Target_Connection_Provided()
            {
                // Given
                _fixture.Settings.TargetConnectionString =
                    "Data Target=(LocalDB)\\v11.0;AttachDbFileName=|DataDirectory|\\DatabaseFileName.mdf;InitialCatalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/TargetFile:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetDatabaseName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetEncryptConnection:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetPassword:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetServerName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetTimeout:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetTrustServerCertificate:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetUser:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Not_Add_Target_File_If_Provided()
            {
                // Given
                _fixture.Settings.TargetFile = "./sqlpublishprofile.pubxml";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/TargetFile:\"/Working/sqlpublishprofile.pubxml\"", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Not_Add_Target_Connection_If_Target_File_Provided()
            {
                // Given
                _fixture.Settings.TargetFile = "./sqlpublishprofile.pubxml";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/TargetConnectionString:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Fact]
            public void Should_Add_Tenant_Id_Provided()
            {
                // Given
                _fixture.Settings.TenantId = "10";

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /TenantId:10", result.Args);
            }

            [Fact]
            public void Should_Add_Universal_Authentication_Provided()
            {
                // Given
                _fixture.Settings.UniversalAuthentication = true;

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /UniversalAuthentication:True", result.Args);
            }

            [Fact]
            public void Should_Not_Add_Target_Properties_If_Target_File_Provided()
            {
                // Given
                _fixture.Settings.TargetFile = "./sqlpublishprofile.pubxml";

                // When
                var result = _fixture.Run();

                // Then
                Assert.DoesNotContain("/TargetConnectionString:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetDatabaseName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetEncryptConnection:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetPassword:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetServerName:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetTimeout:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetTrustServerCertificate:", result.Args, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("/TargetUser:", result.Args, StringComparison.OrdinalIgnoreCase);
            }

            [Theory]
            [InlineData("CommandTimeout", "120")]
            [InlineData("CreateNewDatabase", "True")]
            [InlineData("TableData", "schema_name.table_identifier")]
            [InlineData("ScriptDatabaseOptions", "True")]
            public void Should_Add_Properties_If_Provided(string key, string value)
            {
                // Given
                _fixture.Settings.Properties.Add(key, value);

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal($"/Action:Import /p:{key}={value}", result.Args);
            }

            [Fact]
            public void Should_Add_Properties_After_Settings_If_Provided()
            {
                // Given
                _fixture.Settings.UniversalAuthentication = true;
                _fixture.Settings.Properties.Add("CommandTimeout", "120");

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal("/Action:Import /UniversalAuthentication:True /p:CommandTimeout=120", result.Args);
            }

            [Fact]
            public void Should_Use_Process_Settings_If_Provided()
            {
                // Given
                _fixture.ProcessSettings = new ProcessSettings { RedirectStandardOutput = true };

                // When
                var result = _fixture.Run();

                // Then
                Assert.Equal(_fixture.ProcessSettings, result.Process);
            }
        }
    }
}
