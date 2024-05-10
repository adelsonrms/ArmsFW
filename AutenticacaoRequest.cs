namespace ArmsFW.Security
{
    public class AutenticacaoRequest
    {
        public eAuthType type { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string code { get; set; }
        public string source { get; set; }

        public override string ToString()
        {
            return $"Tipo : {type} | UserName : {type} | Password : {password} | Codigo (Token) : {(string.IsNullOrEmpty(code) ? "": code.Substring(0, 30))} ";
        }
    }
}