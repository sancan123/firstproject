using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingMallSys.Models
{
   
        [Table("STUDENT")]  //指定数据库对应表名
        public class Student
        {
            /// <summary>
            /// 学生学号
            /// </summary>
            [Key]  //主键
            [Column("USERID")] //指定数据库对应表栏位名称
            public string UserId { get; set; }

            /// <summary>
            /// 学生姓名
            /// </summary>
            [MaxLength()]
            [Column("NAME")]
            public string Name { get; set; }
        }
    
}
