using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.Repositorios;

using SistemaVenta.IOC;
using SistemaVenta.Utility;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.BLL.Servicios;

namespace SistemaVenta.IOC
{
    public static class Dependencias
    {
        //Configuro la cadena de conexion 
        public static void inyectarDependencias(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbventaContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(configuration.GetConnectionString("cadenaSQL"));
            });

            services.AddTransient(typeof(IGenericRepository<>),typeof(GenericRepository<>));

            services.AddScoped<IVentaRepository,VentaRepository>();

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddScoped<IRolService,RolService>();
            services.AddScoped<ICategoriaService,CategoriaService>();
            services.AddScoped<IUsuarioService,UsuarioService>();
            services.AddScoped<IMenuService,MenuService>();
            services.AddScoped<IDashBoardService,DashBoardService>();
            services.AddScoped<IProductoService,ProductoService>();
            services.AddScoped<IVentaService,VentaService>();
        }
    }
}
