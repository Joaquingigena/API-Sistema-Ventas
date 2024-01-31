using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.Models;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        public readonly DbventaContext _dbContext;

        public VentaRepository(DbventaContext contexto) : base(contexto)
        {
            _dbContext = contexto;
        }

        public async Task<Venta> Registar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();

            //Creo la transaccion
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    //Recorro el detalle de la venta, que es una coleccion
                    foreach (DetalleVenta dv in modelo.DetalleVenta)
                    {
                        //Busco el producto del detalle
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();

                        //Le resto el stock
                        productoEncontrado.Stock = productoEncontrado.Stock - dv.Cantidad;

                        //Y modifico el producto en la base de datoss
                        _dbContext.Productos.Update(productoEncontrado);
                    }
                    //Guardo los cambios
                    await _dbContext.SaveChangesAsync();

                    //Busco el ultimo numero de la base datos, que incialmente commienzo en 0
                    NumeroDocumento correlativo = _dbContext.NumeroDocumentos.First();
                    //Le sumo uno+
                    correlativo.UltimoNumero++;
                    //Y le pongo la fecha actual
                    correlativo.FechaRegistro = DateTime.Now;

                    _dbContext.NumeroDocumentos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    int cantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidadDigitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    //00001
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - cantidadDigitos, cantidadDigitos);

                    modelo.NumeroDocumento = numeroVenta;

                    await _dbContext.Venta.AddAsync(modelo);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = modelo;

                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }

                return ventaGenerada;
            }
        }
    }
}
