using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_projekt.models
{
    public class User : ObservableModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Earnings { get; set; }
        public int? SharedAccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public virtual ICollection<PersonalAccount> PersonalAccounts { get; set; }
        public virtual SharedAccount SharedAccount { get; set; }
    }
}
