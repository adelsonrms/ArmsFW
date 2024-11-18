namespace app.core.Comum.Enums
{
    public static class Enuns
    {




        public enum eSexo
        {
            Masculino = 1,
            Feminino = 2
        }

        public enum eStatus
        {
            Ativo = 1,
            Inativo = 2,
            Bloqueado = 3,
            Demitido = 4,
            Suspenso = 5,
            Ferias = 6,
            Afastado = 7
        }

        public enum eTipoContato
        {
            EmailPessoal,
            EmailProffisiconal,
            TelResidencial,
            TelComercial,
            TelCelular
        }

        public enum eTipoContrato
        {
            Trabalho,
            Projeto
        }

        public enum eSituacao
        {
            Ativo,
            Inativo,
            Todos
        }

        public enum eTipoPessoa
        {
            PF,
            PJ
        }

        public enum Ambiente
        {
            RemotoProducao = 1,
            Dev_LocalDB = 2,
            Dev_LocalHost = 3,
            Outro = 4,
        }

    }
}