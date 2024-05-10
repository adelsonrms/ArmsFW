using app.core.Domain.Interfaces;
using app.core.Domain.Request;
using app.core.Domain.Response;
using ArmsFW.Domain.Entities;
using ArmsFW.Infra.Data.Repositories;
using ArmsFW.Services;
using ArmsFW.Services.Shared;
using ArmsFW.Services.Shared.Pesquisa;
using System;
using System.Collections.Generic;

namespace app.core.Services
{
    public class UsuarioService : ServiceBase, IUsuarioService
	{
		//Injação do Repository
		public readonly IUsuarioRepository _db;

		public UsuarioService(IUsuarioRepository rep) : base(typeof(UsuarioService).FullName) => _db = rep;
        #region CRUD - Basico
        /// <summary>
        /// Retorna todos os items da base
        /// </summary>
        /// <returns></returns>
        public List<UsuarioLocal> Listar()
        {
            var lista = _db.Pesquisar(null).Registros;

            return lista;
        }
        /// <summary>
        /// Retorna a instancia de um item no objeto Result
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objeto Result contendo a instancia do item procurado na propriedade .Data</returns>
        public Result<UsuarioLocal> Obter(int id) => _db.ObterPorId(id);
        /// <summary>
        /// Cria um novo item
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public Result<UsuarioLocal> Criar(UsuarioLocal Usuario) => _db.Inserir(Usuario);
        /// <summary>
        /// Atualiza um item da base
        /// </summary>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        public Result<UsuarioLocal> Atualizar(UsuarioLocal Usuario) => _db.Alterar(Usuario);
        /// <summary>
        /// Exclui um item da base dado um ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<int> Excluir(int id) => _db.Excluir(_db.ObterPorId(id).Data);
        /// <summary>
        /// Retorna True se existe um item na base dado um ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Existe(int id) => _db.Existe(x => x.Id == id.ToString()); 
        #endregion
        /// <summary>
        /// Realiza a pesquisa passando um objeto de criterios
        /// </summary>
        /// <param name="pesquisaRequest"></param>
        /// <returns></returns>
        public Pesquisa<UsuarioResponse> Pesquisar(UsuarioRequest pesquisaRequest)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Retorna um usuario dado um ID ou um Email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UsuarioLocal GetUsuario(object id)
        {
			string sId = id?.ToString();
			return _db.PesquisarSingle(u => u.Id == sId || u.Email == sId);
		}
    }
}
