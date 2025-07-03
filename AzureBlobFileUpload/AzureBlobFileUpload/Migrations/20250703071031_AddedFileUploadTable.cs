using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureBlobFileUpload.Migrations
{
    /// <inheritdoc />
    public partial class AddedFileUploadTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileUpload",
                columns: table => new
                {
                    FileUploadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUploadOperation = table.Column<int>(type: "int", nullable: true),
                    FileUploadTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUpload", x => x.FileUploadID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileUpload");
        }
    }
}
