using System.ComponentModel.DataAnnotations;

namespace ShoppingMallSys.Models
{
    public class Author
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(40)]
        public string Address { get; set; }

    }
}
