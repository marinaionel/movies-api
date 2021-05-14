﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoviesApi.Data;

namespace MoviesApi.Migrations
{
    [DbContext(typeof(MoviesContext))]
    [Migration("20210514110325_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MoviesApi.Core.Model.ReverseEngineering.Director", b =>
                {
                    b.Property<int?>("MovieId")
                        .HasColumnType("int")
                        .HasColumnName("movie_id");

                    b.Property<int?>("PersonId")
                        .HasColumnType("int")
                        .HasColumnName("person_id");

                    b.ToTable("directors", "moviesfile");
                });

            modelBuilder.Entity("MoviesApi.Core.Model.ReverseEngineering.Movie", b =>
                {
                    b.Property<int?>("Id")
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<long?>("Year")
                        .HasColumnType("bigint")
                        .HasColumnName("year");

                    b.ToTable("movies", "moviesfile");
                });

            modelBuilder.Entity("MoviesApi.Core.Model.ReverseEngineering.Person", b =>
                {
                    b.Property<long?>("Birth")
                        .HasColumnType("bigint")
                        .HasColumnName("birth");

                    b.Property<int?>("Id")
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.ToTable("people", "moviesfile");
                });

            modelBuilder.Entity("MoviesApi.Core.Model.ReverseEngineering.Rating", b =>
                {
                    b.Property<int?>("MovieId")
                        .HasColumnType("int")
                        .HasColumnName("movie_id");

                    b.Property<float?>("Rating1")
                        .HasColumnType("real")
                        .HasColumnName("rating");

                    b.Property<int?>("Votes")
                        .HasColumnType("int")
                        .HasColumnName("votes");

                    b.ToTable("ratings", "moviesfile");
                });

            modelBuilder.Entity("MoviesApi.Core.Model.ReverseEngineering.Star", b =>
                {
                    b.Property<int?>("MovieId")
                        .HasColumnType("int")
                        .HasColumnName("movie_id");

                    b.Property<int?>("PersonId")
                        .HasColumnType("int")
                        .HasColumnName("person_id");

                    b.ToTable("stars", "moviesfile");
                });
#pragma warning restore 612, 618
        }
    }
}
