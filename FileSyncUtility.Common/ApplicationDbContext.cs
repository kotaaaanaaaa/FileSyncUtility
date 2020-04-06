using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileSyncUtility.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FileSyncUtility.Common
{
    public class ApplicationDbContext : DbContext
    {
        public static string ConnectionString { get; set; }
        
        public static string SqlFileBasePath { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        public void EnsureCreated()
        {
            var path = Path.Combine(SqlFileBasePath, "init");
            ExecuteSqlFiles(path, "*.sql");
        }


        public void Migrate()
        {
            var path = Path.Combine(SqlFileBasePath, "update");
            ExecuteSqlFiles(path, "*.sql");
        }

        private void ExecuteSqlFiles(string path, string pattern)
        {
            var sqlFiles = Directory.EnumerateFiles(path, pattern).ToList();

            using (var cmd = new SqliteCommand())
            using (var con = new SqliteConnection(ConnectionString))
            {
                con.Open();
                cmd.Connection = con;
                sqlFiles.ForEach(x =>
                {
                    var sql = File.ReadAllText(x);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                });
            }
        }

        public DbSet<SynchronizeItemEntity> SynchronizeItems { get; set; }
    }
}
