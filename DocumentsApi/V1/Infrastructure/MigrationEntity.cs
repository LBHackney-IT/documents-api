using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentsApi.V1.Infrastructure
{
    [Table("__EFMigrationsHistory")]
    public class MigrationEntity
    {
        [Column("MigrationId")]
        public string Id { get; set; }

        [Column("ProductVersion")]
        public string EFVersion { get; set; }
    }
}
