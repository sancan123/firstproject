using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

using ShoppingMallSys.Models;

namespace ShoppingMallSys.DataBase
{
    public class DaoDbContext: DbContext
    {

        public DaoDbContext()
        {

        }

        public DaoDbContext(DbContextOptions<DaoDbContext> options) : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseOracle("Data Source=(DESCRIPTION =(ADDRESS =(PROTOCOL = TCP)(HOST =localhost)(PORT =1521))(CONNECT_DATA = (SERVICE_NAME =orcl.szclou.com)));User ID=NETCORE;Password=123456;Persist Security Info=False");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Author>();
        //    base.OnModelCreating(modelBuilder);
        //}

        //public virtual DbSet<Author> Author { get; set; }

        //该处定义你要映射到数据库中的表
        //格式固定
        public DbSet<Student> Student { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //判断当前数据库是Oracle 需要手动添加Schema(DBA提供的数据库账号名称)
            if (this.Database.IsOracle())
            {
                modelBuilder.HasDefaultSchema("NETCORE");
            }
            base.OnModelCreating(modelBuilder);
        }

    }
}
