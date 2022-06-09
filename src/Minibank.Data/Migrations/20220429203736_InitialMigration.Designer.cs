﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minibank.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Minibank.Data.Migrations
{
    [DbContext(typeof(MinibankContext))]
    [Migration("20220429203736_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Minibank.Data.Accounts.AccountDbModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<double>("Balance")
                        .HasColumnType("double precision")
                        .HasColumnName("balance");

                    b.Property<DateTime>("ClosedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("closed_date");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_date");

                    b.Property<int>("Currency")
                        .HasColumnType("integer")
                        .HasColumnName("currency");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_accounts");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_accounts_user_id");

                    b.ToTable("accounts");
                });

            modelBuilder.Entity("Minibank.Data.TransfersHistory.TransferHistoryDbModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("amount");

                    b.Property<string>("FromAccountId")
                        .HasColumnType("text")
                        .HasColumnName("from_account_id");

                    b.Property<string>("ToAccountId")
                        .HasColumnType("text")
                        .HasColumnName("to_account_id");

                    b.HasKey("Id")
                        .HasName("pk_transfers");

                    b.HasIndex("FromAccountId")
                        .HasDatabaseName("ix_transfers_from_account_id");

                    b.HasIndex("ToAccountId")
                        .HasDatabaseName("ix_transfers_to_account_id");

                    b.ToTable("transfers");
                });

            modelBuilder.Entity("Minibank.Data.Users.UserDbModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Login")
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Minibank.Data.Accounts.AccountDbModel", b =>
                {
                    b.HasOne("Minibank.Data.Users.UserDbModel", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_accounts_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minibank.Data.TransfersHistory.TransferHistoryDbModel", b =>
                {
                    b.HasOne("Minibank.Data.Accounts.AccountDbModel", "FromAccount")
                        .WithMany("FromTransfers")
                        .HasForeignKey("FromAccountId")
                        .HasConstraintName("fk_transfers_accounts_from_account_id");

                    b.HasOne("Minibank.Data.Accounts.AccountDbModel", "ToAccount")
                        .WithMany("ToTransfers")
                        .HasForeignKey("ToAccountId")
                        .HasConstraintName("fk_transfers_accounts_to_account_id");

                    b.Navigation("FromAccount");

                    b.Navigation("ToAccount");
                });

            modelBuilder.Entity("Minibank.Data.Accounts.AccountDbModel", b =>
                {
                    b.Navigation("FromTransfers");

                    b.Navigation("ToTransfers");
                });

            modelBuilder.Entity("Minibank.Data.Users.UserDbModel", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
