using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Infrastructure.Database
{
    /// <summary>
    /// A custom value generator for SQL Server that generates UUIDv7 (RFC 4122) GUIDs optimized for SQL Server's uniqueidentifier sorting order.
    /// </summary>
    public class SqlServerUuidV7Generator : ValueGenerator<Guid>
    {
        // Tells EF Core that this is a permanent database ID, not a placeholder
        public override bool GeneratesTemporaryValues => false;

        public override Guid Next(EntityEntry entry)
        {
            // 1. Generate standard RFC UUIDv7 (Timestamp is at the front)
            Guid standardV7 = Guid.CreateVersion7();

            // 2. Read the bytes
            Span<byte> rfcBytes = stackalloc byte[16];
            standardV7.TryWriteBytes(rfcBytes, bigEndian: true, out _);

            // 3. Permute bytes so the 48-bit timestamp moves to the back (Bytes 10-15)
            // This targets SQL Server's unique uniqueidentifier sorting order.
            Span<byte> sqlBytes = stackalloc byte[16];
            rfcBytes[0..6].CopyTo(sqlBytes[10..]);
            rfcBytes[6..8].CopyTo(sqlBytes[8..]);
            rfcBytes[8..10].CopyTo(sqlBytes[6..]);
            rfcBytes[10..12].CopyTo(sqlBytes[4..]);
            rfcBytes[12..16].CopyTo(sqlBytes[0..]);

            return new Guid(sqlBytes, bigEndian: false);
        }
    }
}
