using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormsProyect.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _TagName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagID);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    TopicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _TopicName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.TopicID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    NoForm = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.NoForm);
                    table.ForeignKey(
                        name: "FK_Forms_Topics_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topics",
                        principalColumn: "TopicID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Forms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllowedUsers",
                columns: table => new
                {
                    NoForm = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedUsers", x => new { x.NoForm, x.UserId });
                    table.ForeignKey(
                        name: "FK_AllowedUsers_Forms_NoForm",
                        column: x => x.NoForm,
                        principalTable: "Forms",
                        principalColumn: "NoForm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllowedUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NoForm = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comments_Forms_NoForm",
                        column: x => x.NoForm,
                        principalTable: "Forms",
                        principalColumn: "NoForm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormFilled",
                columns: table => new
                {
                    NoFilledForm = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NoForm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFilled", x => x.NoFilledForm);
                    table.ForeignKey(
                        name: "FK_FormFilled_Forms_NoForm",
                        column: x => x.NoForm,
                        principalTable: "Forms",
                        principalColumn: "NoForm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormFilled_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormTags",
                columns: table => new
                {
                    NoForm = table.Column<int>(type: "int", nullable: false),
                    TagID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTags", x => new { x.NoForm, x.TagID });
                    table.ForeignKey(
                        name: "FK_FormTags_Forms_NoForm",
                        column: x => x.NoForm,
                        principalTable: "Forms",
                        principalColumn: "NoForm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "TagID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    LikeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NoForm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.LikeID);
                    table.ForeignKey(
                        name: "FK_Likes_Forms_NoForm",
                        column: x => x.NoForm,
                        principalTable: "Forms",
                        principalColumn: "NoForm",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    IDQuest = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoForm = table.Column<int>(type: "int", nullable: false),
                    _Type = table.Column<int>(type: "int", nullable: false),
                    TitleQ = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescrQ = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    _Show = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.IDQuest);
                    table.ForeignKey(
                        name: "FK_Questions_Forms_NoForm",
                        column: x => x.NoForm,
                        principalTable: "Forms",
                        principalColumn: "NoForm",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnsweredQuestions",
                columns: table => new
                {
                    IDQuest = table.Column<int>(type: "int", nullable: false),
                    NoFilledForm = table.Column<int>(type: "int", nullable: false),
                    QuestT1 = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    QuestT2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuestT3 = table.Column<int>(type: "int", nullable: true),
                    QuestT4 = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnsweredQuestions", x => new { x.IDQuest, x.NoFilledForm });
                    table.ForeignKey(
                        name: "FK_AnsweredQuestions_FormFilled_NoFilledForm",
                        column: x => x.NoFilledForm,
                        principalTable: "FormFilled",
                        principalColumn: "NoFilledForm");
                    table.ForeignKey(
                        name: "FK_AnsweredQuestions_Questions_IDQuest",
                        column: x => x.IDQuest,
                        principalTable: "Questions",
                        principalColumn: "IDQuest");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowedUsers_UserId",
                table: "AllowedUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnsweredQuestions_NoFilledForm",
                table: "AnsweredQuestions",
                column: "NoFilledForm");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_NoForm",
                table: "Comments",
                column: "NoForm");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFilled_NoForm",
                table: "FormFilled",
                column: "NoForm");

            migrationBuilder.CreateIndex(
                name: "IX_FormFilled_UserId",
                table: "FormFilled",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_TopicID",
                table: "Forms",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UserId",
                table: "Forms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTags_TagID",
                table: "FormTags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_NoForm",
                table: "Likes",
                column: "NoForm");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_NoForm",
                table: "Questions",
                column: "NoForm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedUsers");

            migrationBuilder.DropTable(
                name: "AnsweredQuestions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "FormTags");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "FormFilled");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
