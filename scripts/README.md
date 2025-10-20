# Add a migration
scripts/ef.sh add InitialCreate -e Development

# Update database to latest
scripts/ef.sh update -e Development

# Update database to a specific migration
scripts/ef.sh update SomeMigrationName -e Development

# List migrations
scripts/ef.sh list

# Generate SQL script (idempotent)
scripts/ef.sh script -i -e Production

# Remove last migration (if not applied)
scripts/ef.sh remove

# Drop database (force)
scripts/ef.sh drop

# Show DbContext info
scripts/ef.sh dbinfo

# Options
-e, --env: sets ASPNETCORE_ENVIRONMENT (e.g., Development, Production)
-c, --context: override DbContext (e.g., -c My.Namespace.MyDbContext)
-v, --verbose: verbose EF output
-h, --help: usage

