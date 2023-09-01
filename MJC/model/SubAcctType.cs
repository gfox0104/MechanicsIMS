using QuickBooksSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.model
{
    public class SubAcctType
    {
        public string acctType { get; set; }
        public AccountSubTypeEnum acctSubType { get; set; }
        public string taxType { get; set; }
        
        public SubAcctType(string _acctType, AccountSubTypeEnum _acctSubType, string _taxType ) { 
            acctType = _acctType;
            acctSubType = _acctSubType;
            taxType = _taxType;
        }
    }
}
