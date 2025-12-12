using System;
using System.Collections.Generic;
using ElleMapper;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Tbl_author> Tbl_authors { get; set; }
    public DbSet<Tbl_blog> Tbl_blogs { get; set; }
}
