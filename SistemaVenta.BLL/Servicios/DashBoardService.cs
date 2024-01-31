using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Models;

namespace SistemaVenta.BLL.Servicios
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<Producto> _productoRepositorio;
        private readonly IMapper _mapper;

        public DashBoardService(IVentaRepository ventaRepository, IGenericRepository<Producto> productoRepositorio, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _productoRepositorio = productoRepositorio;
            _mapper = mapper;
        }

        private IQueryable<Venta> retornarVentas (IQueryable<Venta> tablaVentas,int restarCantidadDias)
        {
            DateTime? ultimaFecha= tablaVentas.OrderByDescending(v => v.FechaRegistro).Select(f => f.FechaRegistro).First();

            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);

            return tablaVentas.Where(v => v.FechaRegistro.Value.Date >= ultimaFecha.Value.Date);
        }

        private async Task<int> TotalVentasUltimaSemana()
        {
            int total = 0;

            IQueryable<Venta> ventas =await _ventaRepository.Listar();

            if(ventas.Count() > 0)
            {
                var tablaVentas = retornarVentas(ventas, -7);

                total = tablaVentas.Count();
            }

            return total;
        }

        private async Task<string> TotalIngresosUltimaSemana()
        {
            decimal total = 0;
            IQueryable<Venta> ventas= await _ventaRepository.Listar();

            if(ventas.Count() > 0)
            {
                var tablaVentas= retornarVentas(ventas,7);

                total = tablaVentas.Select(v => v.Total).Sum(v => v.Value);
            }

            return Convert.ToString(total, new CultureInfo("es-AR"));
        }

        private async Task<int> totalProductos()
        {
            IQueryable<Producto> productos = await _productoRepositorio.Listar();

            int total = productos.Count();

            return total;
        }

        private async Task<Dictionary<string, int>> ventasUltimaSemana()
        {
            Dictionary<string,int> resultado= new Dictionary<string,int>();

            IQueryable<Venta> ventas= await _ventaRepository.Listar();

            if(ventas.Count() > 0)
            {
                var tablaVenta= retornarVentas(ventas,-7);

                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
            }

            return resultado;
        }

        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDashBoard= new DashBoardDTO();

            try
            {
                vmDashBoard.TotalVentas= await TotalVentasUltimaSemana();
                vmDashBoard.TotalProductos= await totalProductos();
                vmDashBoard.TotalIngresos = await TotalIngresosUltimaSemana();

                List<VentaSemanaDTO> listaVentaSemana= new List<VentaSemanaDTO>();

                foreach(KeyValuePair<string,int> item in await ventasUltimaSemana())
                {
                    listaVentaSemana.Add(new VentaSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }

                vmDashBoard.VentasUltimaSemana= listaVentaSemana;


            }
            catch (Exception)
            {

                throw;
            }

            return vmDashBoard;
        }
    }
}
