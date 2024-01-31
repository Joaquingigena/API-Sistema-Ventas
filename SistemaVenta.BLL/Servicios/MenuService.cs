using System;
using System.Collections.Generic;
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
    public class MenuService: IMenuService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepositorio;
        private readonly IGenericRepository<MenuRol> _menuRolRepositorio;
        private readonly IGenericRepository<Menu> _menuRepositorio;
        private readonly IMapper _mapper;

        public MenuService(IGenericRepository<Usuario> usuarioRepositorio, IGenericRepository<MenuRol> menuRolRepositorio, IGenericRepository<Menu> menuRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _menuRolRepositorio = menuRolRepositorio;
            _menuRepositorio = menuRepositorio;
            _mapper = mapper;
        }

        public async Task<List<MenuDTO>> Listar(int id)
        {
            IQueryable<Usuario> tbUsuario= await _usuarioRepositorio.Listar(u => u.IdUsuario == id);
            IQueryable<MenuRol> tbMenuRol = await _menuRolRepositorio.Listar();
            IQueryable<Menu> tbMenu= await _menuRepositorio.Listar();

            try
            {
                IQueryable<Menu> tbResultado= (from u in tbUsuario
                                               join mr in tbMenuRol on u.IdRol equals mr.IdRol
                                               join m in tbMenu on mr.IdMenu equals m.IdMenu
                                               select m).AsQueryable();

                var listaMenu= tbResultado.ToList();

                return _mapper.Map<List<MenuDTO>>(listaMenu);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
