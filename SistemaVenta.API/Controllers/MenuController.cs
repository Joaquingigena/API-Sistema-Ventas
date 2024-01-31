using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DTO;
using SistemaVenta.API.Utilidad;
using SistemaVenta.BLL.Servicios;

namespace SistemaVenta.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }


        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista(int id)
        {
            var rsp = new Response<List<MenuDTO>>();

            try
            {
                rsp.Status = true;
                rsp.Value = await _menuService.Listar(id);

            }
            catch (Exception ex)
            {

                rsp.Status = false;
                rsp.Msj = ex.Message;
            }

            return Ok(rsp);
        }
    }
}
