using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MTGApplication
{
  public static class AppConfig
  {
    private static IConfigurationRoot configurationRoot;

    public static string CompanyName { get; set; }

    public static void Init()
    {
      var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
      configurationRoot = builder.Build();

      CompanyName = GetCompanyName();
    }

    private static string GetCompanyName()
    {
      var config = configurationRoot.GetSection("AppInformation").GetChildren();
      return config.First(x => x.Key == "Company").Value;
    }
  }
}
