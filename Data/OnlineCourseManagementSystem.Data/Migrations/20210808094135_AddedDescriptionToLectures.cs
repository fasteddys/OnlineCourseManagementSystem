﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineCourseManagementSystem.Data.Migrations
{
    public partial class AddedDescriptionToLectures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Lectures",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lectures");
        }
    }
}
