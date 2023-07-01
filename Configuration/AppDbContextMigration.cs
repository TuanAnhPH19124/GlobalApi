using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlobalApi.Data;
using Microsoft.EntityFrameworkCore;

namespace GlobalApi.Configuration
{
    public static class AppDbContextMigration
    {
        public static void StartMigration(IApplicationBuilder app){
            using (var scopeService = app.ApplicationServices.CreateScope()){
                scopeService.ServiceProvider.GetService<AppDbContext>()?.Database.Migrate();
            }
        }
    }
}