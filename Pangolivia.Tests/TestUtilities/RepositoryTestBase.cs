using System;
using Pangolivia.API.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;

namespace Pangolivia.Tests.TestUtilities;

/// <summary>
/// Base class for repository integration tests using a shared Sqlite in-memory database per test.
/// Ensures proper creation and disposal of the DbContext and underlying connection.
/// </summary>
public abstract class RepositoryTestBase : IDisposable
{
    protected readonly PangoliviaDbContext _context;
    private readonly SqliteConnection _connection;
    protected readonly ILogger _logger;

    protected RepositoryTestBase()
    {
        (_context, _connection) = TestDbFactory.CreateSqliteInMemoryDb();
        _logger = new Mock<ILogger>().Object;
    }

    public void Dispose()
    {
        TestDbFactory.Dispose(_context, _connection);
    }
}