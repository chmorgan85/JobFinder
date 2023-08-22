using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobFinder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_AspNetUsers_UserId",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_UserId",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Job");

            migrationBuilder.CreateTable(
                name: "ApplicationUserJob",
                columns: table => new
                {
                    SavedJobsID = table.Column<int>(type: "int", nullable: false),
                    SavingUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserJob", x => new { x.SavedJobsID, x.SavingUsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserJob_AspNetUsers_SavingUsersId",
                        column: x => x.SavingUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserJob_Job_SavedJobsID",
                        column: x => x.SavedJobsID,
                        principalTable: "Job",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserJob_SavingUsersId",
                table: "ApplicationUserJob",
                column: "SavingUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserJob");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Job",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Job_UserId",
                table: "Job",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_AspNetUsers_UserId",
                table: "Job",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
