using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HFFitnessApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityLevel",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DietaryPreferences",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FitnessGoal",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FoodPreferences",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HealthConditions",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InjuryHistory",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SleepPatterns",
                table: "HealthData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "DietaryPreferences",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "FitnessGoal",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "FoodPreferences",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "HealthConditions",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "InjuryHistory",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "HealthData");

            migrationBuilder.DropColumn(
                name: "SleepPatterns",
                table: "HealthData");
        }
    }
}
