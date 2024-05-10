//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Linq;
//using System.Reflection;

//namespace ArmsFW.Infra.Identity
//{
//    public static class ModelBuilderExtensions
//    {
//        public static void MapearTabelasIdentity(this ModelBuilder modelBuilder)
//        {
//            foreach (Type item in (from t in Assembly.GetExecutingAssembly().GetTypes()
//                                   where t.GetInterfaces().Any((Type gi) => gi.IsGenericType && gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
//                                   select t).ToList())
//            {
//                if (!item.ContainsGenericParameters)
//                {
//                    dynamic val = Activator.CreateInstance(item);
//                    modelBuilder.ApplyConfiguration(val);
//                }
//            }
//        }
//    }
//}