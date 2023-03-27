﻿using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MTGApplication
{
  public static class AppConfig
  {
    private static IConfigurationRoot configurationRoot;

    public static string CompanyName { get; set; }

    /// <summary>
    /// Initializes the App configurations using appsettings.json
    /// </summary>
    public static void Initialize()
    {
      var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
      configurationRoot = builder.Build();

      CompanyName = GetCompanyName();
    }

    /// <summary>
    /// Returns company's name from the appsettings.json
    /// </summary>
    private static string GetCompanyName()
    {
      var config = configurationRoot.GetSection("AppInformation").GetChildren();
      return config.First(x => x.Key == "Company").Value;
    }
  }
}
