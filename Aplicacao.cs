using ArmsFW.Services.Shared.Settings;
using ArmsFW.Services.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Recupera informações sobre o arquivo executando no momento
/// </summary>
public static class Aplicacao
{
    public const string SchemaPadrao = "dbo";
    public static string Arquivo => $@"{Path.GetFileName(Assembly.GetExecutingAssembly().Location)}";
    public static string NomeDoArquivo => $@"{Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location)}";
    public static string Versao => $@"{Assembly.GetExecutingAssembly().GetName().Version}";
    public static string Diretorio => $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
    public static string Executavel => Assembly.GetExecutingAssembly().Location;
    public static DateTime DataDePublicacao => File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
    public static Assembly Assembly => Assembly.GetExecutingAssembly();

    public static object DiretorioStore { get; set; }

    public static async Task<List<Menu>> CarregarMenu(List<Menu> menu)
    {
        try
        {
            //Retorna um enumerable de Perfis de Menu
            var perfis = new List<MenuPerfil>();//await new ORMEngineDapper<MenuPerfil>().Rep.QuerySelect($"select distinct p.Id, mp.MenuId  from tpa.Perfil p  left join tpa.MenuPerfil mp on p.Id = mp.PerfilId and mp.ativo = 1 where '{nomePerfil}' like '%<perfil>' + p.Nome + '</perfil>%'");

            var todos = menu;

            //Somente os menus Ativos
            var menus_ativos = todos.Where(mn => mn.Ativo).ToList();

            //Menus vinculado ao perfil
            var menus_do_perfil = (from m in menus_ativos
                                   join p in perfis on m.Id equals p.MenuId
                                   group m by m into tabela
                                   select tabela.Key
                        ).ToList<Menu>();

            //O filtro acima esta sendo ignorada.
            menus_do_perfil = todos;

            //Popula os Submenus do menu pai
            menus_do_perfil.ToList().ForEach(m =>
            {
                if (m.Roles == null) m.Roles = new List<string>();
                if (m.ParentMenuId != 0) m.Parent = menus_ativos.First(p => p.Id == m.ParentMenuId);

                var submenus = (from sm in menus_ativos.Where(x => x.ParentMenuId == m.Id) join M in menus_do_perfil on sm.Id equals M.Id select sm).OrderBy(o => o.Ordem);

                if (submenus.Count() > 0) m.SubMenus.AddRange(submenus);
            });

            return await Task.FromResult(menus_do_perfil.OrderBy(o => o.Ordem).ToList());
        }
        catch
        {
            return await Task.FromResult(new List<Menu>());
        }
    }

    /// <summary>
    /// Carrega as informações que serão usadas na estrutura do menu. 
    /// Pode ser vir de um DB ou de um arquivo json
    /// </summary>
    /// <param name="nomePerfil"></param>
    /// <returns></returns>
    public static async Task<List<Menu>> CarregarMenu(string perfil)
    {
        try
        {
            var retorno_menu = await CarregarMenu(Config.Default.Carregar().Menu);
            return await Task.FromResult(retorno_menu);
        }
        catch
        {
            return await Task.FromResult(new List<Menu>());
        }
    }
    public static string Saudacao
    {
        get
        {
            if (DateTime.Now.Hour < 12) return "Bom dia !";
            if (DateTime.Now.Hour < 18) return "Bom tarde !";
            return "Boa noite !";
        }
    }

}

public class UID
{
    public UID()
    {
        uid = Guid.NewGuid().ToString();
        dtCriacao = DateTime.Now;
    }

    public DateTime dtCriacao { get; private set; }
    public string uid { get; private set; }
}