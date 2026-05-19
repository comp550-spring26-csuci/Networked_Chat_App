using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangePresenceStatusToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Manual SQL to handle the conversion of PresenceStatus from Text to Integer
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users"" 
                  ALTER COLUMN ""PresenceStatus"" TYPE integer 
                  USING (CASE 
                    WHEN ""PresenceStatus"" = 'Active' THEN 1 
                    WHEN ""PresenceStatus"" = 'Online' THEN 1 
                    WHEN ""PresenceStatus"" = 'Custom' THEN 2 
                    ELSE 0 
                  END);"
            );

            // 2. This keeps your CustomStatusText changes (Max length 100)
            migrationBuilder.AlterColumn<string>(
                name: "CustomStatusText",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert PresenceStatus back to text
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""PresenceStatus"" TYPE text;");

            migrationBuilder.AlterColumn<string>(
                name: "CustomStatusText",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}