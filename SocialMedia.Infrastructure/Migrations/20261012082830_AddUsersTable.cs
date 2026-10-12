using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identifier of the user in this system (UUID v4)."),
                    clerk_user_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Identifier of the user in Clerk, taken from the sub claim of the session token."),
                    username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true, comment: "Username copied from Clerk. Null when the user has none."),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "First name copied from Clerk. Null when the user has none."),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "Last name copied from Clerk. Null when the user has none."),
                    avatar_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true, comment: "URL of the profile image copied from Clerk. Null when the user has none."),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "When the row was created, in UTC."),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "When the row was last changed, in UTC. Null when it has never been changed."),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, comment: "True when the user has been soft-deleted. Deleted users are hidden from queries."),
                    deleted_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "When the user was soft-deleted, in UTC. Null while the user is not deleted.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                },
                comment: "People who use the app. Sign-in is handled by Clerk; each row is the local copy of one Clerk user.");

            migrationBuilder.CreateIndex(
                name: "IX_users_clerk_user_id",
                table: "users",
                column: "clerk_user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
