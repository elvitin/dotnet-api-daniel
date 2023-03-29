using MySql.Data.MySqlClient;
using TrabalhoECommerceAPI.Models.Produto;

namespace TrabalhoECommerceAPI.Repository
{
    public class ProdutoRepository
    {

        WrapperMySQL _mysql = new();

        /*
SELECT * FROM vartechs13.CategoriaProduto;


Select p.*, c.CategoriaId,c.nome as CategoriaNome, u.nome as UnidadeNome from Produto p inner join Unidade u on u.UnidadeId = p.UnidadeId 
                inner join CategoriaProduto cp on cp.ProdutoId = p.ProdutoId
                inner join Categoria c on c.CategoriaId = cp.CategoriaId where p.Nome = "Kit Kat";
                
                
                
insert into CategoriaProduto values (2,1);

delete from CategoriaProduto where CategoriaId = 1 and ProdutoId =1 ;
Select p.*,c.CategoriaId,c.Nome as CategoriaNome, u.nome as UnidadeNome from Produto p inner join Unidade u on u.UnidadeId = p.UnidadeId
inner join CategoriaProduto cp on cp.ProdutoId = p.ProdutoId inner join Categoria c on c.CategoriaId = cp.CategoriaId 
where p.Nome like "P%"
      */
        public Produto ObterPorNome(String nome)
        {
            Produto p = null;
            try
            {
                _mysql.Comando.CommandText = $@"Select p.*,c.CategoriaId ,c.nome as CategoriaNome, u.nome as UnidadeNome from Produto p inner join Unidade u on u.UnidadeId = p.UnidadeId 
                inner join CategoriaProduto cp on cp.ProdutoId = p.ProdutoId
                inner join Categoria c on c.CategoriaId = cp.CategoriaId where p.Nome = @Nome";
                _mysql.Comando.Parameters.AddWithValue("@Nome", nome);
                _mysql.Abrir();
                var result = _mysql.Comando.ExecuteReader();
                if(result.Read())
                {
                    p = new Produto()
                    {
                        Id = (int)result["ProdutoId"],
                        Nome = result["Nome"].ToString(),
                        PrecoVenda = (Decimal)result["PrecoVenda"],
                        Estoque = (int)result["Estoque"],
                        Ativo = (bool)result["Ativo"],
                        Unidade = new()
                        {
                            Nome = result["UnidadeNome"].ToString(),
                            Id = (int)result["UnidadeId"]
                        },
                        Categorias = new()
                    };
                    p.Categorias.Add(new()
                    {
                        Nome = result["CategoriaNome"].ToString(),
                        Id = (int) result["CategoriaId"]
                    });

                    while(result.Read())
                    {
                        p.Categorias.Add(new()
                        {
                            Nome = result["CategoriaNome"].ToString(),
                            Id = (int)result["CategoriaId"]
                        });
                    }
                }
            }
            catch (Exception e) { }
            finally
            {
                _mysql.Fechar();
            }

            return p;
        }


        public bool Salvar(Produto produto)
        {
            bool sucesso = false;
            MySqlTransaction myTrans;
            _mysql.Abrir();
            myTrans = _mysql.Conexao.BeginTransaction();
            try
            {
                _mysql.Comando.Parameters.Clear();
                _mysql.Comando.CommandText = $@"insert into Produto (Nome,PrecoVenda,Estoque,Ativo,UnidadeId) values( @Nome , @PrecoVenda , @Estoque , @Ativo , @UnidadeId)";
                _mysql.Comando.Parameters.AddWithValue("@Nome", produto.Nome);
                _mysql.Comando.Parameters.AddWithValue("@PrecoVenda", produto.PrecoVenda);
                _mysql.Comando.Parameters.AddWithValue("@Estoque", produto.Estoque);
                _mysql.Comando.Parameters.AddWithValue("@Ativo", produto.Ativo);
                _mysql.Comando.Parameters.AddWithValue("@UnidadeId", produto.Unidade.Id);
                int linhaafetadas = _mysql.Comando.ExecuteNonQuery();
                sucesso = linhaafetadas > 0;

                if (sucesso)
                {
                 
                    produto.Id = (int)_mysql.Comando.LastInsertedId;
                    sucesso = SalvarCategorias(produto);
                }

                if (sucesso)
                    myTrans.Commit();
                else myTrans.Rollback();

            }
            catch (Exception e)
            {
                sucesso = false;
                Console.WriteLine("EXCEPTION: "+e.Message);
                myTrans.Rollback();

            }
            finally { _mysql.Fechar(); }

            return sucesso;
        }

        private bool SalvarCategorias(Produto p)
        {
            
            bool sucesso = false;
            int linhasafetadas = 0;
            
            foreach (Categoria c in p.Categorias)
            {
              
                _mysql.Comando.Parameters.Clear();
                _mysql.Comando.CommandText = $@"insert into CategoriaProduto values (@CategoriaId , @ProdutoId)";
                _mysql.Comando.Parameters.AddWithValue("@CategoriaId", c.Id);
                _mysql.Comando.Parameters.AddWithValue("@ProdutoId", p.Id);
                    linhasafetadas += _mysql.Comando.ExecuteNonQuery();
              
            }

            sucesso = linhasafetadas == p.Categorias.Count();

            return sucesso;
        }

        public Produto ObterPorId(int id)
        {
            Produto p = null;
            try
            {
                _mysql.Comando.CommandText = $@"Select p.*,c.CategoriaId ,c.nome as CategoriaNome, u.nome as UnidadeNome from Produto p inner join Unidade u on u.UnidadeId = p.UnidadeId 
                inner join CategoriaProduto cp on cp.ProdutoId = p.ProdutoId
                inner join Categoria c on c.CategoriaId = cp.CategoriaId where p.ProdutoId = {id}";

                _mysql.Abrir();
                var result = _mysql.Comando.ExecuteReader();
                if (result.Read())
                {
                    p = new Produto()
                    {
                        Id = (int)result["ProdutoId"],
                        Nome = result["Nome"].ToString(),
                        PrecoVenda = (Decimal)result["PrecoVenda"],
                        Estoque = (int)result["Estoque"],
                        Ativo = (bool)result["Ativo"],
                        Unidade = new()
                        {
                            Nome = result["UnidadeNome"].ToString(),
                            Id = (int)result["UnidadeId"]
                        },
                        Categorias = new()
                    };
                    p.Categorias.Add(new()
                    {
                        Nome = result["CategoriaNome"].ToString(),
                        Id = (int)result["CategoriaId"]
                    });

                    while (result.Read())
                    {
                        p.Categorias.Add(new()
                        {
                            Nome = result["CategoriaNome"].ToString(),
                            Id = (int)result["CategoriaId"]
                        });
                    }
                }
            }
            catch (Exception e) { }
            finally
            {
                _mysql.Fechar();
            }

            return p;
        }
        /*
        public bool ValidarCategorias(List<Categoria> cats)
        {
            bool sucesso = true;
            try
            {
                _mysql.Abrir();
                foreach(Categoria c in cats)
                {
                    _mysql.Comando.CommandText = $@"select count(*) from Categoria where CategoriaId = {c.Id}";
                    int existe =(int)  _mysql.Comando.ExecuteScalar();
                    if (existe == 0)
                    {
                        sucesso = false;
                        break;
                    }
                        
                }
              

            }
            catch(Exception e)
            { 
            }
            finally { _mysql.Fechar(); }

            return sucesso;
        }
        */
        public bool Excluir(int id)
        {
            bool sucesso = false;
            _mysql.Abrir();
            MySqlTransaction myTrans = _mysql.Conexao.BeginTransaction();
            try
            {
                _mysql.Comando.Parameters.Clear();
                if(ExcluirCategorias(id))
                {
                    _mysql.Comando.CommandText = $@"delete from Produto where ProdutoId = {id}";
                    int linhasafetadas = _mysql.Comando.ExecuteNonQuery();
                    if (linhasafetadas > 0)
                        sucesso = true;

                }

                if (!sucesso)
                    myTrans.Rollback();
                else
                    myTrans.Commit();
            }
            catch(Exception e) {
                sucesso = false;
                Console.WriteLine("EXCEPTION: " + e.Message);
                myTrans.Rollback();
            }
            finally { _mysql.Fechar(); }

            return sucesso;
        }

        private bool ExcluirCategorias(int id)
        {
            int linhasafetadas = 0;
            _mysql.Comando.CommandText = $@"delete from CategoriaProduto where ProdutoId = {id}";
             linhasafetadas = _mysql.Comando.ExecuteNonQuery();
            return linhasafetadas >0;
        }

        public List<Produto>Consultar(String nome)
        {
            List<Produto> lista = new List<Produto>();
            Produto p;
           
            try
            {
                _mysql.Comando.CommandText = $@"Select p.*, u.nome as UnidadeNome from Produto p inner join Unidade u on u.UnidadeId = p.UnidadeId 
                where p.Nome like @Nome";
                _mysql.Comando.Parameters.AddWithValue("@Nome",nome+"%");
                _mysql.Abrir();
                var result = _mysql.Comando.ExecuteReader();
                Console.WriteLine("executou result");
                while(result.Read())
                {
                    Console.WriteLine("Entrou no result");
                    p = new()
                    {
                        Id = (int)result["ProdutoId"],
                        Nome = result["Nome"].ToString(),
                        PrecoVenda = (decimal)result["PrecoVenda"],
                        Estoque = (int)result["Estoque"],
                        Ativo = (bool)result["Ativo"],
                        Unidade = new() { Id = (int)result["UnidadeId"], Nome = result["UnidadeNome"].ToString() },
                        Categorias = new()
                    };
                    lista.Add(p);
                    
                }
                _mysql.Fechar();
                
                foreach(Produto produto in lista)
                {
                    produto.Categorias = BuscarCategorias(produto.Id);
                }
            }
            catch(Exception e) {
                Console.WriteLine("EXCEPTION: " + e.Message);
              
            }
            finally { _mysql.Fechar(); }
            
            return lista;
        }

        private List<Categoria> BuscarCategorias(int id)
        {
            Console.WriteLine("Foi no buscarcategorias");
            List<Categoria> cats = new();
            _mysql.Comando.CommandText = $@"select c.CategoriaId , c.Nome as CategoriaNome from Categoria c inner join CategoriaProduto cp on cp.CategoriaId = c.CategoriaId where cp.ProdutoId = {id} ";
            _mysql.Abrir();
            var rs = _mysql.Comando.ExecuteReader();
            Console.WriteLine("executou rs");
            while(rs.Read())
            {
                Console.WriteLine("Entrou no read de categoriaas");
                cats.Add(new()
                {
                    Id = (int)rs["CategoriaId"],
                    Nome = rs["CategoriaNome"].ToString()
                });
            }
            _mysql.Fechar();
            return cats;
        }
    }


}
