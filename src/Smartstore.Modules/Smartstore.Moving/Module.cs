global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IO;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Threading;
global using System.Threading.Tasks;
global using FluentValidation;
global using Microsoft.EntityFrameworkCore;
global using Newtonsoft.Json;
global using Smartstore.Moving.Domain;
global using Smartstore.Web.Modelling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Smartstore.Core;
using Smartstore.Core.Messaging;
using Smartstore.Core.Widgets;
using Smartstore.Engine.Modularity;
using Smartstore.Http;
using Smartstore.Moving.Components;
using Smartstore.Moving.Migrations;

namespace Smartstore.Moving
{
    internal class Module : ModuleBase, IConfigurable, IWidget
    {
        public ILogger Logger { get; set; } = NullLogger.Instance;

        public RouteInfo GetConfigurationRoute()
            => new("List", "Video", new { area = "Admin" });

        public WidgetInvoker GetDisplayWidget(string widgetZone, object model, int storeId)
            => new ComponentWidgetInvoker(typeof(HomepageNewsViewComponent), null);

        public string[] GetWidgetZones()
            => new string[] { "home_page_after_tags" };
        
        public override async Task InstallAsync(ModuleInstallationContext context)
        {
            await TrySaveSettingsAsync<MovingSettings>();
            await ImportLanguageResourcesAsync();
        

            await base.InstallAsync(context);

            await TrySeedData(context);
        }

        private async Task TrySeedData(ModuleInstallationContext context)
        {
            

            try
            {
                var seeder = new NewsInstallationDataSeeder(context, Services.Resolve<IMessageTemplateService>(), Services.Resolve<IWidgetService>());
                await seeder.SeedAsync(Services.DbContext);
            }
            catch (Exception ex)
            {
                context.Logger.Error(ex, "NewsSampleDataSeeder failed.");
            }
        }

        public override async Task UninstallAsync()
        {
            await DeleteSettingsAsync<MovingSettings>();
            await DeleteLanguageResourcesAsync();

            await base.UninstallAsync();
        }
    }
}
