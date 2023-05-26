using System.ComponentModel;

namespace Web.DataBase.Enum
{
    public enum DbEnumType
    {
        [Description("SqlServerConnection")]
        sqlserver=1,
        [Description("OracleConnection")]
        oracle =2,
        [Description("MysqlConnection")]
        mysql =3,
        [Description("SqlLiteConnection")]
        sqllite =4,
        [Description("AccessConnection")]
        access =5
    }
}
