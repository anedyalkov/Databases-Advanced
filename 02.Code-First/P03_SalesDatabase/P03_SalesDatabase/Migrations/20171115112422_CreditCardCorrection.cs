using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace P03_SalesDatabase.Migrations
{
    public partial class CreditCardCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreaditCardNumber",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "CreditCardNumber",
                table: "Customers",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditCardNumber",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "CreaditCardNumber",
                table: "Customers",
                unicode: false,
                nullable: false,
                defaultValue: "");
        }
    }
}
