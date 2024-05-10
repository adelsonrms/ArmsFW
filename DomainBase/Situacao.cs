using System;
using ArmsFW.Lib.Web.Json;

namespace ArmsFW
{
    public enum eSituacaoLancamento
    {
        Criado = 0, Liberado = 1, Aprovado = 2, Bloqueado = 3, Fechado = 4,
        Excluido = 5,
        Restaurado = 6
    }
    public class Situacao
    {
        public eSituacaoLancamento? st_status { get; set; }

        public DateTime? dt_alteracao { get; set; }

        public string id_usuario { get; set; }

        public string info { get; set; }
        public string flags { get; set; }
        public int? id_solicitacao { get; set; }
        public bool TemBloqueio { get; private set; }
        public Situacao()
        {
            dt_alteracao = DateTime.Now;

        }
        public Situacao(string infoBloqueio)
        {
            try
            {
                if (string.IsNullOrEmpty(infoBloqueio)) return;

                if (infoBloqueio.IsJson())
                {
                    var obj = JSON.JsonToObject<Situacao>(infoBloqueio);

                    if (obj != null)
                    {
                        this.st_status = obj.st_status;
                        this.dt_alteracao = obj.dt_alteracao;
                        this.id_usuario = obj.id_usuario;
                        this.info = obj.info;
                        this.flags = obj.flags;
                        this.id_solicitacao = obj.id_solicitacao;
                        this.TemBloqueio = true;
                    }
                }
                else
                {
                    if (infoBloqueio.Contains("|"))
                    {
                        //Se nao, faz a leitura antiga
                        string[] arrInfo = infoBloqueio.Split("|".ToCharArray());
                        if (arrInfo.Length == 3)
                        {
                            this.st_status = arrInfo[0] == "1" ? eSituacaoLancamento.Bloqueado : eSituacaoLancamento.Liberado;
                            if (DateTime.TryParse(arrInfo[1], out var dt)) this.dt_alteracao = dt;
                            this.id_usuario = arrInfo[2];
                            this.TemBloqueio = true;
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
