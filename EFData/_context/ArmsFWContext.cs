using Microsoft.EntityFrameworkCore;

namespace ArmsFW.Infra.Data.Contexts
{

    public partial class ArmsFWContext : SqlDbContext
	{
		public static ArmsFWContext Create()
		{
			return new ArmsFWContext(DbConfig.PegarConexaoConfig<ArmsFWContext>());
		}

		public ArmsFWContext(DbContextOptions<ArmsFWContext> options)
			: base(options)
		{
		}

		public ArmsFWContext()
			: base(DbConfig.PegarConexaoConfig<ArmsFWContext>())
		{
		}
	}


    public partial class MySqlDbContext : SqlDbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
            : base(options)
        {
        }
    }
}
