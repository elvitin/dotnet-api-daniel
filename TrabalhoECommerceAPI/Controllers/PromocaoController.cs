using Microsoft.AspNetCore.Mvc;
using TrabalhoECommerceAPI.Models.Produto;
using TrabalhoECommerceAPI.Services;
using TrabalhoECommerceAPI.ViewModels.Produto;

namespace TrabalhoECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocaoController : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult Gravar(PromocaoViewModel vm)
        {

            Promocao p = new()
            {
                Id = vm.Id,
                Preco = vm.Preco,
                Produto = new()
                {
                    Id = vm.ProdutoId
                }
            };
            PromocaoService ps = new PromocaoService();
            (bool sucesso, ValidationResult vr) = ps.Salvar(p);
            if (sucesso)
                return Ok(ValidationResult.Sucesso("Gravado com Sucesso"));
            else
                return BadRequest(vr);
        }

        [HttpDelete]
        [Route("[action]")]
        public IActionResult Excluir(int id)
        {
            return BadRequest();
        }

        public IActionResult Obter(int id)
        {
            return BadRequest();
        }
    }
}
