using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.Models;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IGenericRepository<DetalleVenta> _detalleVentaRepositorio;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepository, IGenericRepository<DetalleVenta> detalleVentaRepositorio, IMapper mapper)
        {
            _ventaRepository = ventaRepository;
            _detalleVentaRepositorio = detalleVentaRepositorio;
            _mapper = mapper;
        }
        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepository.Registar(_mapper.Map<Venta>(modelo));

                if (ventaGenerada.IdVenta == 0)
                    throw new TaskCanceledException("No se pudo registrar la venta");

                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch 
            {

                throw;
            }
        }
        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepository.Listar();
            var listResultado= new List<Venta>();

            try
            {
                if(buscarPor == "fecha")//Busqueda por fecha
                {
                    DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
                    DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

                    listResultado = await query.Where(v =>
                        v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                        v.FechaRegistro.Value.Date <= fecha_fin.Date
                    ).Include(dv => dv.DetalleVenta) //Incluyo el detalle de la venta
                    .ThenInclude(p => p.IdProductoNavigation) // incluyo el producto
                    .ToListAsync();

                }
                else //Busqueda por numerio de documento
                {
                    listResultado = await query.Where( v=> v.NumeroDocumento == numeroVenta 
                    ).Include(dv => dv.DetalleVenta) //Incluyo el detalle de la venta
                    .ThenInclude(p => p.IdProductoNavigation) // incluyo el producto
                    .ToListAsync();
                }
            }
            catch 
            {

                throw;
            }
            return _mapper.Map<List<VentaDTO>>(listResultado);
        }

      

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetalleVenta> query = await _detalleVentaRepositorio.Listar();
            var listaDetalle = new List<DetalleVenta>();

            try
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-AR"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-AR"));

                listaDetalle = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv =>
                        dv.IdVentaNavigation.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                        dv.IdVentaNavigation.FechaRegistro.Value.Date <= fecha_fin.Date
                    )
                    .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }

            return _mapper.Map<List<ReporteDTO>>(listaDetalle);
        }
    }
}
