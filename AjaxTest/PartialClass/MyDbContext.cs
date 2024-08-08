using Microsoft.EntityFrameworkCore;

namespace AjaxTest.Models  //這邊要更新namespace 
{
    public partial class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) //讀設定檔內的連線資訊，判斷參數設定過了嗎
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) //設定檔路徑(.NET程式都放在AppDomain的基準路徑)
                    .AddJsonFile("appsettings.json")  //設定檔名稱
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyDb")); //依據名稱取值
            }
        }
    }
}