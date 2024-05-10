using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmsFW.Services.Azure
{
	public class UserRequest
    {
        public string Id { get; set; }
        public dynamic User { get; set; }
        public dynamic Funcionario { get; set; }
        public string Photo { get; set; }
        public string Manager { get; set; }
    }

    public class CriarContaResponse
    {
        public UserResponse response;
        public User conta;
        public string acao;
        public string id;
    }

    public class UserResponse
    {
        public string Id { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string acao { get; set; }
        public string senhaTemporaria { get; set; }

        public bool valido => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(email);
        public string msg { get; set; }

    }

    /// <summary>
    /// Estrurua da classe de um User do MSGraph na versão V1
    /// </summary>
    public class UserV1 : GraphEntity
	{
		public string PreferredName { get; set; }

		public string MySite { get; set; }

		public ProfilePhoto Photo { get; set; }

		public DateTimeOffset? Birthday { get; set; }

		public string MobilePhone { get; set; }

		public string MailNickname { get; set; }

		public string Mail { get; set; }

		public string JobTitle { get; set; }

		public IEnumerable<string> ImAddresses { get; set; }

		public string GivenName { get; set; }

		public string DisplayName { get; set; }

		public string Department { get; set; }

		public string CompanyName { get; set; }

		public string City { get; set; }

		public IEnumerable<string> BusinessPhones { get; set; }

		public bool? AccountEnabled { get; set; }

		public string Country { get; set; }

		public string AboutMe { get; set; }

		public string UserPrincipalName { get; set; }

		public string UsageLocation { get; set; }

		public string Surname { get; set; }

		public string StreetAddress { get; set; }

		public string State { get; set; }

		public string OfficeLocation { get; set; }

		public string PostalCode { get; set; }

		public string LegalAgeGroupClassification { get; set; }

		public string AgeGroup { get; set; }

		public string UserType { get; set; }
		public int Status { get; set; }

        public DateTimeOffset? deletedDateTime { get; set; }
		public DateTimeOffset? createdDateTime { get; set; }

		public override string ToString()
		{
			return "Usuario : " + DisplayName + " (" + Mail + ") - " + base.Id;
		}
	}

    public class PasswordProfile
    {
        public bool forceChangePasswordNextSignIn { get; set; }
        public string password { get; set; }
    }

    public class AssignedLicence
    {
        public string SkuId { get; set; }
        public string Nome { get; set; }

        public override string ToString()
        {
            return $"{SkuId} - {Nome}";
        }
    }

    public class AssignedPlan
    {
        public DateTime AssignedDateTime { get; set; }
        public string CapabilityStatus { get; set; }
        public string Service { get; set; }
        public string ServicePlanId { get; set; }
    }

    public class Identity
    {
        public string SignInType { get; set; }
        public string Issuer { get; set; }
        public string IssuerAssignedId { get; set; }
    }

    public class OnPremisesExtensionAttributes
    {
        public object ExtensionAttribute1 { get; set; }
        public object ExtensionAttribute2 { get; set; }
        public object ExtensionAttribute3 { get; set; }
        public object ExtensionAttribute4 { get; set; }
        public object ExtensionAttribute5 { get; set; }
        public object ExtensionAttribute6 { get; set; }
        public object ExtensionAttribute7 { get; set; }
        public object ExtensionAttribute8 { get; set; }
        public object ExtensionAttribute9 { get; set; }
        public object ExtensionAttribute10 { get; set; }
        public object ExtensionAttribute11 { get; set; }
        public object ExtensionAttribute12 { get; set; }
        public object ExtensionAttribute13 { get; set; }
        public object ExtensionAttribute14 { get; set; }
        public object ExtensionAttribute15 { get; set; }
    }

    public class ProvisionedPlan
    {
        public string CapabilityStatus { get; set; }
        public string ProvisioningStatus { get; set; }
        public string Service { get; set; }
    }

    public class GraphUser: GraphEntity
    {
        public override string ToString() => $"{base.Id} - {DisplayName} ({Mail})";

        public DateTime? DeletedDateTime { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public bool AccountEnabled { get; set; }
        public List<string> BusinessPhones { get; set; }
        public string City { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string Department { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeId { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string MailNickname { get; set; }
        public string MobilePhone { get; set; }
        public string OfficeLocation { get; set; }
        public List<string> OtherMails { get; set; }
        public object PasswordPolicies { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public PasswordProfile PasswordProfile { get; set; }
        public List<AssignedLicence> AssignedLicenses { get; set; }
    }
    /// <summary>
    /// Estrurua da classe de um User do MSGraph na versão beta (dez/2021)
    /// </summary>
    public class User : GraphUser
    {
        public override string ToString()
        {
            return $"{base.Id} - {DisplayName} ({Mail})";
        }

        public bool TemLicenca => this.AssignedLicenses?.Count>0;
        public string Photo { get; set; }
        public int Status => (AccountEnabled ? 1 : 0);
        public List<string> ObtemNomeDasLicencas()
        {
            if (!TemLicenca)
            {
                return new List<string>();
            }

            return this.AssignedLicenses.ConvertAll<string>(x => x.Nome);
        }
    }
}
