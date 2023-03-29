using TrabalhoECommerceAPI.Models.Produto;

namespace TrabalhoECommerceAPI.Repository
{
    public class PromocaoRepository
    {
        WrapperMySQL _mysql = new ();


        public bool Salvar(Promocao promocao)
        {
            bool sucesso = false;
            Console.WriteLine(promocao.Id);
            try
            {
                if(promocao.Id == 0)
                    _mysql.Comando.CommandText = $@"insert into Promocao(Preco,ProdutoId) values (@Preco , @ProdutoId)";
                else
                {
                    _mysql.Comando.CommandText = $@"update Promocao set Preco = @Preco  where PromocaoId = @PromocaoId";
                    _mysql.Comando.Parameters.AddWithValue("@PromocaoId", promocao.Id);
                }
                _mysql.Comando.Parameters.AddWithValue("@Preco", promocao.Preco);
                _mysql.Abrir();
                int linhasafetadas = _mysql.Comando.ExecuteNonQuery();
                sucesso = linhasafetadas > 0;

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { _mysql.Fechar(); }
            return sucesso;
        }

        public bool Excluir(int id)
        {
            bool sucesso = false;
            try
            {
                _mysql.Comando.CommandText = $@"delete from Promocao where PromocaoId = {id}";
                _mysql.Abrir();
                int linhasafetadas = _mysql.Comando.ExecuteNonQuery();
                sucesso = linhasafetadas > 0;
            }
            catch(Exception e) { Console.WriteLine(e.Message); }
            finally { _mysql.Fechar(); }
            return sucesso;
        }
    }
}
