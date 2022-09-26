﻿// <auto-generated />
using System;
using DocumentsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DocumentsApi.Migrations
{
    [DbContext(typeof(DocumentsContext))]
    partial class DocumentsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("DocumentsApi.V1.Infrastructure.ClaimEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("ApiCreatedBy")
                        .IsRequired()
                        .HasColumnName("api_created_by")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("DocumentId")
                        .HasColumnName("document_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("RetentionExpiresAt")
                        .HasColumnName("retention_expires_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ServiceAreaCreatedBy")
                        .HasColumnName("service_area_created_by")
                        .HasColumnType("text");

                    b.Property<Guid?>("TargetId")
                        .HasColumnName("target_id")
                        .HasColumnType("uuid");

                    b.Property<string>("UserCreatedBy")
                        .HasColumnName("user_created_by")
                        .HasColumnType("text");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnName("valid_until")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("DocumentId");

                    b.ToTable("claims");
                });

            modelBuilder.Entity("DocumentsApi.V1.Infrastructure.DocumentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("text");

                    b.Property<long>("FileSize")
                        .HasColumnName("file_size")
                        .HasColumnType("bigint");

                    b.Property<string>("FileType")
                        .HasColumnName("file_type")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UploadedAt")
                        .HasColumnName("uploaded_at")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("documents");
                });

            modelBuilder.Entity("DocumentsApi.V1.Infrastructure.ClaimEntity", b =>
                {
                    b.HasOne("DocumentsApi.V1.Infrastructure.DocumentEntity", "Document")
                        .WithMany("Claims")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
