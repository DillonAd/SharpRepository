using NUnit.Framework;
namespace SharpRepository.Tests.Integration.Infrastructure
{
    public class SqlServerContainer : ContainerBase
    {
        public SqlServerContainer() 
            : base("microsoft/mssql-server-linux", "latest", "sr-sql-server", 1433) { }
    }
}
