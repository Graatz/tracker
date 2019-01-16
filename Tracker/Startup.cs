using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using Tracker.DTO;
using Tracker.Models;

[assembly: OwinStartupAttribute(typeof(Tracker.Startup))]
namespace Tracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<Track, TrackDTO>();
                cfg.CreateMap<TrackPoint, TrackPointDTO>();
                /* etc */
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add application services
        }
    }
}
