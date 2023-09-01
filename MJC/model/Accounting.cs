using QuickBooksSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.model
{
    public class Accounting
    {
        public int Id { get; set; }
        public string FullyQualifiedName { get; set; }
        public string Name { get; set; }
        public string? AcctNum { get; set; }
        public string? Description { get; set; }
        public AccountTypeEnum AccountType { get; set; }
        public decimal? CurrentBalance { get; set; }
        public bool? SubAccount { get; set; }
        public int? ParentId { get; set; }
        public string? SubAcctType { get; set; }
        public string SyncToken { get; set; }
        public bool? Active { get; set; }
        public string? QboId { get; set; }
    }
}
