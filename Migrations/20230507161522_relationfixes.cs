using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    public partial class relationfixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_AspNetUsers_UserId",
                table: "UserBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBooks",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserBooks_BookId",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserBooks_UserId",
                table: "UserBooks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserBooks");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserBooks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBooks",
                table: "UserBooks",
                columns: new[] { "BookId", "AppUser" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_AppUser",
                table: "UserBooks",
                column: "AppUser");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_AspNetUsers_AppUser",
                table: "UserBooks",
                column: "AppUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_AspNetUsers_AppUser",
                table: "UserBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBooks",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserBooks_AppUser",
                table: "UserBooks");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserBooks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserBooks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBooks",
                table: "UserBooks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_BookId",
                table: "UserBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_UserId",
                table: "UserBooks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_AspNetUsers_UserId",
                table: "UserBooks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
