using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using FamilyRealtor.Models.DomainModels;

namespace FamilyRealtor.Models
{
	public class User : IdentityUser
	{
		public virtual Client? Client { get; set; }

		public int? ClientId { get; set; }

		[NotMapped]
		public IList<string>? RoleNames { get; set; }

		public string RoleString
		{
			get
			{
				string roleString = string.Empty;
				if (RoleNames == null || RoleNames.Count == 0) return roleString;

				foreach (string role in RoleNames)
				{
					roleString += role + ", ";
				}

				return roleString[..^2];
			}
		}
	}
}