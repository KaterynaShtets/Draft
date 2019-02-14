using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tut20.Models;


namespace WebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Reading> Readings { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Solving> Solvings { get; set; }
        public DbSet<Mark> Marks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
