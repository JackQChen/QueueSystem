using System.Configuration;
using Chloe;

namespace DAL
{
    public class Factory
    {
        static Factory _instance;

        public static Factory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Factory();
                return _instance;
            }
        }

        public DbContext CreateDbContext()
        {
            return this.CreateDbContext("MySQL", ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
        }

        public DbContext CreateDbContext(string dbType, string connString)
        {
            switch (dbType)
            {
                case "MySQL":
                    //暂时不用反射
                    //Activator.CreateInstance("");  
                    return new Chloe.MySql.MySqlContext(new MySqlConnectionFactory(connString));
            }
            return null;
        }
    }
}
