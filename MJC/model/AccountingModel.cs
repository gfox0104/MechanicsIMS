using MJC.config;
using QuickBooksSharp.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MJC.model
{
    public class AccountingModel : DbConnection
    {
        public struct TaxForm
        {
            public string acctType { get; set; }
            public AccountSubTypeEnum acctSubType { get; set; }
            public string taxType { get; set; }
            public TaxForm(string _acctType, AccountSubTypeEnum _acctSubType, string _taxType)
            {
                acctType = _acctType;
                acctSubType = _acctSubType;
                taxType = _taxType;
            }
        }

        public List<Accounting> LoadAccountingList()
        {
            List<Accounting> accountingList = new List<Accounting>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM Accountings ORDER BY id";

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = Convert.ToInt32(row["id"]);
                        string fullyQualifiedName = row["fullyQualifiedName"].ToString();
                        string name = row["name"].ToString();
                        string? acctNum = null;
                        if(!row.IsNull("acctNum"))
                            acctNum = row["acctNum"].ToString();
                        string? desc = null;
                        if (!row.IsNull("description"))
                            row["description"].ToString();
                        string? acctType = null;
                        if (!row.IsNull("accountType"))
                            acctType =row["accountType"].ToString();
                        bool? subAccount = null;
                        if (!row.IsNull("subAccount"))
                            subAccount = row.Field<bool>("subAccount");
                        decimal? currentBalance = null;
                        if (!row.IsNull("currentBalance"))
                            currentBalance = decimal.Parse(row["currentBalance"].ToString());
                        int? parentId = null;
                        if (!row.IsNull("parentId"))
                            parentId = int.Parse(row["parentId"].ToString());
                        string? subAcctType = null;
                        if (!row.IsNull("subAcctType"))
                            subAcctType = row["subAcctType"].ToString();
                        string syncToken = row["syncToken"].ToString();
                        bool active = row.Field<bool>("active");
                        string qboId = row["qboId"].ToString();

                        accountingList.Add(new Accounting { Id = id, FullyQualifiedName = fullyQualifiedName, Name = name, AcctNum = acctNum, Description = desc, AccountType = (AccountTypeEnum)Enum.Parse(typeof(AccountTypeEnum), acctType), SubAccount = subAccount, ParentId = parentId, SubAcctType = subAcctType, SyncToken = syncToken, Active = active, QboId = qboId, CurrentBalance = currentBalance});
                    }
                }

                return accountingList;
            }
        }

        public bool AddAccounting(string? fullyQualifiedName, string? name, string? acctNum, string? description, string? accountType, decimal? currentBalance, bool? subAccount, string? parentId, string? subAccountType, string syncToken, int active, int createdBy, int updatedBy, string? qboId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.Accountings (name, acctNum, description, accountType, currentBalance, subAccount, parentId, subAcctType, syncToken, active, createdBy, updatedBy, qboId, fullyQualifiedName) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14)";
                    if (name != null)
                        command.Parameters.AddWithValue("@Value1", name);
                    else command.Parameters.AddWithValue("@Value1", DBNull.Value);
                    if (acctNum != null)
                        command.Parameters.AddWithValue("@Value2", acctNum);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (description != null )
                        command.Parameters.AddWithValue("@Value3", description);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    command.Parameters.AddWithValue("@Value4", accountType);
                    if(currentBalance != null)
                        command.Parameters.AddWithValue("@Value5", currentBalance);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if(subAccount != null)
                        command.Parameters.AddWithValue("@Value6", subAccount);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (parentId != null)
                        command.Parameters.AddWithValue("@Value7", parentId);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    if(subAccountType != null)
                        command.Parameters.AddWithValue("@Value8", subAccountType);
                    else command.Parameters.AddWithValue("@Value8", DBNull.Value);
                    command.Parameters.AddWithValue("@Value9", syncToken);
                    command.Parameters.AddWithValue("@Value10", active);
                    command.Parameters.AddWithValue("@Value11", createdBy);
                    command.Parameters.AddWithValue("@Value12", updatedBy);
                    command.Parameters.AddWithValue("@Value13", qboId);
                    if (fullyQualifiedName != null)
                        command.Parameters.AddWithValue("@Value14", fullyQualifiedName);
                    else command.Parameters.AddWithValue("@Value14", DBNull.Value);
                    //int newId = (int)command.ExecuteScalar();
                    command.ExecuteScalar();
                    //MessageBox.Show("New Category is added successfully.");
                }

                return true;
            }
        }
    }
}
