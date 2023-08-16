using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetworkInfrastructure.Web.Migrations
{
    public partial class InitialDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetworkAsset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    BackupType = table.Column<int>(type: "int", nullable: false),
                    Monitoring = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Limitation = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ServiceOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OsType = table.Column<int>(type: "int", nullable: false),
                    LocationName = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirewallWindows = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NetBios = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FolderShare = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Mcafee = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activation = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SplunkAgent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    GroupPolicy = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AccessLists = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FtdDatacenter = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FtdInterne = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Sophos = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Waf = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Asr = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ServerPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dns = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2023, 8, 12, 12, 50, 56, 281, DateTimeKind.Local).AddTicks(9248)),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkAsset", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetworkAsset");
        }
    }
}
