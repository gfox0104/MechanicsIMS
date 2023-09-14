using Intuit.Ipp.OAuth2PlatformClient;
using MJC.common;
using MJC.common.components;
using MJC.model;
using MJC.qbo;
using QuickBooksSharp.Entities;
using System.Data;
using System.Linq;
using static MJC.model.AccountingModel;

namespace MJC.forms
{
    public partial class AccountingDetail : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private FComboBox AccountType = new FComboBox("Account Type:", 250);
        private FComboBox SaveAccountUnder = new FComboBox("Save Account Under:", 250);
        private FComboBox TaxFormSection = new FComboBox("Tax form section:", 250);
        private FInputBox AccountName = new FInputBox("Account Name:", 250);
        private FInputBox AcctNum = new FInputBox("AcctNum:", 250);
        private FInputBox Description = new FInputBox("Description:", 250);
        private FGroupLabel BalanceLabel = new FGroupLabel("Starting date and opening balance");
        private FDateTime StartingDate = new FDateTime("Starting Date:", 250);
        private FInputBox Balance = new FInputBox("Balance:", 250);

        private int accountingId = 0;
        List<model.Accounting> TotalAccountList = new List<model.Accounting>();
        List<model.Accounting> AcctList = new List<model.Accounting>();
        List<SubAcctType> TotalTaxTypeList = new List<SubAcctType>();

        public AccountingDetail() : base("Account Details", "Manage details of an Accounting")
        {
            InitializeComponent();
            _initBasicSize();
            this.KeyDown += (s, e) => Form_KeyDown(s, e);

            HotkeyButton[] hkButtons = new HotkeyButton[1] { hkPreviousScreen };
            _initializeHKButtons(hkButtons);

            InitInputBox();
        }

        private void InitInputBox()
        {
            List<model.Accounting> accountingList = Session.accountingModelObj.LoadAccountingList();
            List<model.Accounting> acctTypeList = new List<model.Accounting>();
            acctTypeList.Add(new model.Accounting { Id = 0, Name = "Income" });
            acctTypeList.Add(new model.Accounting { Id = 1, Name = "Expenses"});
            acctTypeList.Add(new model.Accounting { Id = 2, Name = "Banks"});
            acctTypeList.Add(new model.Accounting { Id = 3, Name = "Assets"});
            acctTypeList.Add(new model.Accounting { Id = 4, Name = "Credit cards"});
            acctTypeList.Add(new model.Accounting { Id = 5, Name = "Liabilites"});
            acctTypeList.Add(new model.Accounting { Id = 6, Name = "Equity"});

            List<model.Accounting> acctDetailTypeList = new List<model.Accounting>();
            acctDetailTypeList.Add(new model.Accounting { Id = -1, Name = "Income", AccountType = AccountTypeEnum.Income });
            acctDetailTypeList.Add(new model.Accounting { Id = -2, Name = "Other Income", AccountType = AccountTypeEnum.OtherIncome });
            acctDetailTypeList.Add(new model.Accounting { Id = -3, Name = "Cost Of Goods Sold", AccountType = AccountTypeEnum.CostofGoodsSold });
            acctDetailTypeList.Add(new model.Accounting { Id = -4, Name = "Expenses", AccountType = AccountTypeEnum.Expense });
            acctDetailTypeList.Add(new model.Accounting { Id = -5, Name = "Other Expenses", AccountType = AccountTypeEnum.OtherExpense });
            acctDetailTypeList.Add(new model.Accounting { Id = -6, Name = "Bank Accounts", AccountType = AccountTypeEnum.Bank });
            acctDetailTypeList.Add(new model.Accounting { Id = -7, Name = "Accounts Receivable", AccountType = AccountTypeEnum.AccountsReceivable });
            acctDetailTypeList.Add(new model.Accounting { Id = -8, Name = "Other Current Assets", AccountType = AccountTypeEnum.OtherCurrentAsset});
            acctDetailTypeList.Add(new model.Accounting { Id = -9, Name = "Fixed Assets", AccountType = AccountTypeEnum.FixedAsset });
            acctDetailTypeList.Add(new model.Accounting { Id = -10, Name = "Other Assets", AccountType = AccountTypeEnum.OtherAsset });
            acctDetailTypeList.Add(new model.Accounting { Id = -11, Name = "Credit Cards", AccountType = AccountTypeEnum.CreditCard });
            acctDetailTypeList.Add(new model.Accounting { Id = -12, Name = "Accounts Payable", AccountType = AccountTypeEnum.AccountsPayable });
            acctDetailTypeList.Add(new model.Accounting { Id = -13, Name = "Other Current Liabilites", AccountType = AccountTypeEnum.OtherCurrentLiability });
            acctDetailTypeList.Add(new model.Accounting { Id = -14, Name = "Long term Liabilites", AccountType = AccountTypeEnum.LongTermLiability });
            acctDetailTypeList.Add(new model.Accounting { Id = -15, Name = "Equity", AccountType = AccountTypeEnum.Equity });
            
            int index = 0;

            foreach (model.Accounting accountData in accountingList)
            {
                int colonCount = accountData.FullyQualifiedName.Split(":").Length - 1;
                string prefixStr = AddSpace(colonCount);
                string tempStr = prefixStr + accountData.Name;
                accountingList[index].Name = tempStr;
                index++;
            }

            List<model.Accounting> IncomeAccountingList = accountingList.Where(item => item.AccountType.Equals("Income")).ToList();
            List<model.Accounting> OtherIncomeAccountingList = accountingList.Where(item => item.AccountType.Equals("OtherIncome")).ToList();
            List<model.Accounting> CostOfGoodsAccountingList = accountingList.Where(item => item.AccountType.Equals("CostofGoodsSold")).ToList();
            List<model.Accounting> ExpensesAccountingList = accountingList.Where(item => item.AccountType.Equals("Expense")).ToList();
            List<model.Accounting> OtherExpensesAccountingList = accountingList.Where(item => item.AccountType.Equals("OtherExpense")).ToList();
            List<model.Accounting> BankAccountingList = accountingList.Where(item => item.AccountType.Equals("Bank")).ToList();
            List<model.Accounting> AccountsReceivableAccountingList = accountingList.Where(item => item.AccountType.Equals("AccountsReceivable")).ToList();
            List<model.Accounting> OtherCurrentAssetsAccountingList = accountingList.Where(item => item.AccountType.Equals("OtherCurrentAsset")).ToList();
            List<model.Accounting> FixedAssetsAccountingList = accountingList.Where(item => item.AccountType.Equals("FixedAsset")).ToList();
            List<model.Accounting> OtherAssetsAccountingList = accountingList.Where(item => item.AccountType.Equals("OtherCurrentAsset")).ToList();
            List<model.Accounting> CreditCardsAccountingList = accountingList.Where(item => item.AccountType.Equals("CreditCard")).ToList();
            List<model.Accounting> AccountsPaylableAccountingList = accountingList.Where(item => item.AccountType.Equals("AccountsPayable")).ToList();
            List<model.Accounting> OtherCurrentLiabilityAccountingList = accountingList.Where(item => item.AccountType.Equals("OtherCurrentLiability")).ToList();
            List<model.Accounting> LongTermLiabilityAccountingList = accountingList.Where(item => item.AccountType.Equals("LongTermLiability")).ToList();
            List<model.Accounting> EquityAccountingList = accountingList.Where(item => item.AccountType.Equals("Equity")).ToList();

            TotalAccountList.Add(acctDetailTypeList[0]);
            TotalAccountList.AddRange(IncomeAccountingList);
            TotalAccountList.Add(acctDetailTypeList[1]);
            TotalAccountList.AddRange(OtherIncomeAccountingList);
            TotalAccountList.Add(acctDetailTypeList[2]);
            TotalAccountList.AddRange(CostOfGoodsAccountingList);
            TotalAccountList.Add(acctDetailTypeList[3]);
            TotalAccountList.AddRange(ExpensesAccountingList);
            TotalAccountList.Add(acctDetailTypeList[4]);
            TotalAccountList.AddRange(OtherExpensesAccountingList);
            TotalAccountList.Add(acctDetailTypeList[5]);
            TotalAccountList.AddRange(BankAccountingList);
            TotalAccountList.Add(acctDetailTypeList[6]);
            TotalAccountList.AddRange(AccountsReceivableAccountingList);
            TotalAccountList.Add(acctDetailTypeList[7]);
            TotalAccountList.AddRange(OtherCurrentAssetsAccountingList);
            TotalAccountList.Add(acctDetailTypeList[8]);
            TotalAccountList.AddRange(FixedAssetsAccountingList);
            TotalAccountList.Add(acctDetailTypeList[9]);
            TotalAccountList.AddRange(OtherAssetsAccountingList);
            TotalAccountList.Add(acctDetailTypeList[10]);
            TotalAccountList.AddRange(CreditCardsAccountingList);
            TotalAccountList.Add(acctDetailTypeList[11]);
            TotalAccountList.AddRange(AccountsPaylableAccountingList);
            TotalAccountList.Add(acctDetailTypeList[12]);
            TotalAccountList.AddRange(OtherCurrentLiabilityAccountingList);
            TotalAccountList.Add(acctDetailTypeList[13]);
            TotalAccountList.AddRange(LongTermLiabilityAccountingList);
            TotalAccountList.Add(acctDetailTypeList[14]);
            TotalAccountList.AddRange(EquityAccountingList);
            AcctList = TotalAccountList;

            List<dynamic> FormComponents = new List<dynamic>();
            AccountType.GetComboBox().Size = new Size(400, 20);
            AccountType.GetComboBox().DataSource = acctTypeList;
            AccountType.GetComboBox().ValueMember = "Id";
            AccountType.GetComboBox().DisplayMember = "Name";
            AccountType.GetComboBox().SelectedIndexChanged += new EventHandler(AcctTypeChanged);

            FormComponents.Add(AccountType);
            SaveAccountUnder.GetComboBox().DataSource = AcctList;
            SaveAccountUnder.GetComboBox().ValueMember = "Id";
            SaveAccountUnder.GetComboBox().DisplayMember = "Name";
            SaveAccountUnder.GetComboBox().SelectedIndexChanged += new EventHandler(SaveAccountUnderChanged);
            SaveAccountUnder.GetComboBox().Size = new Size(400, 20);
            FormComponents.Add(SaveAccountUnder);
            TaxFormSection.GetComboBox().Size = new Size(400, 20);
            TaxFormSection.GetComboBox().Enabled = false;
            TaxFormSection.GetComboBox().DisplayMember = "taxType";
            TaxFormSection.GetComboBox().ValueMember = "acctSubType";
            FormComponents.Add(TaxFormSection);

            AcctNum.GetTextBox().Size = new Size(400, 20);
            FormComponents.Add(AcctNum);
            Description.GetTextBox().Size = new Size(400, 20);
            FormComponents.Add(Description);
            _addFormInputs(FormComponents, 750, 150, 500, 50);

            List<dynamic> FormComponents1 = new List<dynamic>();
            FormComponents1.Add(BalanceLabel);
            FormComponents1.Add(StartingDate);
            FormComponents1.Add(Balance);
            BalanceLabel.GetLabel().Visible = false;
            StartingDate.GetLabel().Visible = false;
            StartingDate.GetDateTimePicker().Visible = false;
            Balance.GetLabel().Visible = false;
            Balance.GetTextBox().Visible = false;
            _addFormInputs(FormComponents1, 750, 400, 500, 50);

            foreach (AccountSubTypeEnum subAcctType in Enum.GetValues(typeof(AccountSubTypeEnum)))
            {
                string m_str = subAcctType.GetStringValue();


        //             public string acctType { get; set; }
        //public AccountSubTypeEnum acctSubType { get; set; }
        //public string taxType { get; set; }
        //public SubAcctType(string _acctType, AccountSubTypeEnum _acctSubType, string _taxType)
        //{
        //    acctType = _acctType;
        //    acctSubType = _acctSubType;
        //    taxType = _taxType;

            if (subAcctType == AccountSubTypeEnum.DiscountsRefundsGiven)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Income", AccountSubTypeEnum.DiscountsRefundsGiven, "Discounts/Refunds Given"));
                } else if (subAcctType == AccountSubTypeEnum.NonProfitIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Income", AccountSubTypeEnum.NonProfitIncome, "Non-Profit Income"));
                } else if (subAcctType == AccountSubTypeEnum.OtherPrimaryIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Income", AccountSubTypeEnum.OtherPrimaryIncome, "Other Primary Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.SalesOfProductIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Income", AccountSubTypeEnum.SalesOfProductIncome, "Sales of Product Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.ServiceFeeIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Income", AccountSubTypeEnum.ServiceFeeIncome, "Service/Fee Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.UnappliedCashPaymentIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Income", AccountSubTypeEnum.UnappliedCashPaymentIncome, "Unapplied Cash Payment Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.DividendIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherIncome", AccountSubTypeEnum.DividendIncome, "Divided Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.InterestEarned)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherIncome", AccountSubTypeEnum.InterestEarned, "Interest Earned"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherInvestmentIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherIncome", AccountSubTypeEnum.OtherInvestmentIncome, "Other Investment Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherMiscellaneousIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherIncome", AccountSubTypeEnum.OtherMiscellaneousIncome, "Other Miscellaneous Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.TaxExemptInterest)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherIncome", AccountSubTypeEnum.TaxExemptInterest, "Tax-Exempt Interest"));
                }
                else if (subAcctType == AccountSubTypeEnum.CostOfLaborCos)
                {
                    TotalTaxTypeList.Add(new SubAcctType("CostofGoodsSold", AccountSubTypeEnum.CostOfLaborCos, "Cost of labor - COS"));
                }
                else if (subAcctType == AccountSubTypeEnum.EquipmentRentalCos)
                {
                    TotalTaxTypeList.Add(new SubAcctType("CostofGoodsSold", AccountSubTypeEnum.EquipmentRentalCos, "Equipment Rental - COS"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherCostsOfServiceCos)
                {
                    TotalTaxTypeList.Add(new SubAcctType("CostofGoodsSold", AccountSubTypeEnum.OtherCostsOfServiceCos, "Other Costs of Services - COS"));
                }
                else if (subAcctType == AccountSubTypeEnum.ShippingFreightDeliveryCos)
                {
                    TotalTaxTypeList.Add(new SubAcctType("CostofGoodsSold", AccountSubTypeEnum.ShippingFreightDeliveryCos, "Shipping, Freight & Delivery - COS"));
                }
                else if (subAcctType == AccountSubTypeEnum.SuppliesMaterialsCogs)
                {
                    TotalTaxTypeList.Add(new SubAcctType("CostofGoodsSold", AccountSubTypeEnum.SuppliesMaterialsCogs, "Supplies & Materials"));
                }
                else if (subAcctType == AccountSubTypeEnum.AdvertisingPromotional)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.AdvertisingPromotional, "Advertising/Promotional"));
                }
                else if (subAcctType == AccountSubTypeEnum.Auto)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.Auto, "Auto"));
                }
                else if (subAcctType == AccountSubTypeEnum.BadDebts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.BadDebts, "Bad Debts"));
                }
                else if (subAcctType == AccountSubTypeEnum.BankCharges)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.BankCharges, "Bank Charges"));
                }
                else if (subAcctType == AccountSubTypeEnum.CharitableContributions)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.CharitableContributions, "Charitable Contributions"));
                }
                else if (subAcctType == AccountSubTypeEnum.Communications)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.Communications, "Communication"));
                }
                else if (subAcctType == AccountSubTypeEnum.CostOfLabor)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.CostOfLabor, "Cost of Labor"));
                }
                else if (subAcctType == AccountSubTypeEnum.DuesSubscriptions)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.DuesSubscriptions, "Dues & subscriptions"));
                }
                else if (subAcctType == AccountSubTypeEnum.Entertainment)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.Entertainment, "Entertainment"));
                }
                else if (subAcctType == AccountSubTypeEnum.EntertainmentMeals)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.EntertainmentMeals, "Entertainment Meals"));
                }
                else if (subAcctType == AccountSubTypeEnum.EquipmentRental)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.EquipmentRental, "Equipment Rental"));
                }
                else if (subAcctType == AccountSubTypeEnum.FinanceCosts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.FinanceCosts, "Finance costs"));
                }
                else if (subAcctType == AccountSubTypeEnum.Insurance)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.Insurance, "Insurance"));
                }
                else if (subAcctType == AccountSubTypeEnum.InterestPaid)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.InterestPaid, "Interest Paid"));
                }
                else if (subAcctType == AccountSubTypeEnum.LegalProfessionalFees)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.LegalProfessionalFees, "Legal & Professional Fees"));
                }
                else if (subAcctType == AccountSubTypeEnum.OfficeGeneralAdministrativeExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.OfficeGeneralAdministrativeExpenses, "Office/General Administrative Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.OfficeGeneralAdministrativeExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.OtherBusinessExpenses, "Office/General Administrative Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherBusinessExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.OtherBusinessExpenses, "Other Business Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherMiscellaneousServiceCost)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.OtherMiscellaneousServiceCost, "Other Miscellaneous Service Cost"));
                }
                else if (subAcctType == AccountSubTypeEnum.PayrollExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.PayrollExpenses, "Payroll Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.PayrollTaxExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.PayrollTaxExpenses, "Payroll Tax Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.PayrollWageExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.PayrollWageExpenses, "Payroll Wage Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.PromotionalMeals)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.PromotionalMeals, "Promotional Meals"));
                }
                else if (subAcctType == AccountSubTypeEnum.RentOrLeaseOfBuildings)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.RentOrLeaseOfBuildings, "Rent or Lease of Buildings"));
                }
                else if (subAcctType == AccountSubTypeEnum.RepairMaintenance)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.RepairMaintenance, "Repair & Maintenance"));
                }
                else if (subAcctType == AccountSubTypeEnum.ShippingFreightDelivery)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.ShippingFreightDelivery, "Shipping, Freight & Delivery"));
                }
                else if (subAcctType == AccountSubTypeEnum.SuppliesMaterials)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.SuppliesMaterials, "Supplies & Materials"));
                }
                else if (subAcctType == AccountSubTypeEnum.PayrollTaxExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.PayrollTaxExpenses, "Tax expense"));
                }
                else if (subAcctType == AccountSubTypeEnum.TaxesPaid)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.TaxesPaid, "Taxes Paid"));
                }
                else if (subAcctType == AccountSubTypeEnum.Travel)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.Travel, "Travel"));
                }
                else if (subAcctType == AccountSubTypeEnum.TravelMeals)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.TravelMeals, "Travel Meals"));
                }
                else if (subAcctType == AccountSubTypeEnum.UnappliedCashBillPaymentExpense)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.UnappliedCashBillPaymentExpense, "Unapplied Cash Bill Payment Expense"));
                }
                else if (subAcctType == AccountSubTypeEnum.Utilities)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Expenses", AccountSubTypeEnum.Utilities, "Utilities"));
                }
                else if (subAcctType == AccountSubTypeEnum.Amortization)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.Amortization, "Amortization"));
                }
                else if (subAcctType == AccountSubTypeEnum.Depreciation)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.Depreciation, "Depreciation"));
                }
                else if (subAcctType == AccountSubTypeEnum.ExchangeGainOrLoss)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.ExchangeGainOrLoss, "Exchange Gain Or Loss"));
                }
                else if (subAcctType == AccountSubTypeEnum.GasAndFuel)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.GasAndFuel, "Gas And Fuel"));
                }
                else if (subAcctType == AccountSubTypeEnum.HomeOffice)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.HomeOffice, "Home Office"));
                }
                else if (subAcctType == AccountSubTypeEnum.HomeownerRentalInsurance)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.HomeownerRentalInsurance, "Home owner Rental Insurance"));
                }
                else if (subAcctType == AccountSubTypeEnum.MortgageInterestHomeOffice)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.MortgageInterestHomeOffice, "Mortgage Interest Home Office"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherHomeOfficeExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.OtherHomeOfficeExpenses, "Other Home Office Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherMiscellaneousExpense)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.OtherMiscellaneousExpense, "Other Miscellaneous Expense"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherVehicleExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.OtherVehicleExpenses, "Other Vehicle Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.ParkingAndTolls)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.ParkingAndTolls, "Parking and Tolls"));
                }
                else if (subAcctType == AccountSubTypeEnum.PenaltiesSettlements)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.PenaltiesSettlements, "Penalties & Settlements"));
                }
                else if (subAcctType == AccountSubTypeEnum.PropertyTaxHomeOffice)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.PropertyTaxHomeOffice, "Property Tax Home Office"));
                }
                else if (subAcctType == AccountSubTypeEnum.RentAndLeaseHomeOffice)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.PropertyTaxHomeOffice, "Rent And Lease Home Office"));
                }
                else if (subAcctType == AccountSubTypeEnum.RepairsAndMaintainceHomeOffice)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.RepairsAndMaintainceHomeOffice, "Repairs And Maintaince Home Office"));
                }
                else if (subAcctType == AccountSubTypeEnum.UtilitiesHomeOffice)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.UtilitiesHomeOffice, "Utilities Home Office"));
                }
                else if (subAcctType == AccountSubTypeEnum.Vehicle)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.Vehicle, "Vehicle"));
                }
                else if (subAcctType == AccountSubTypeEnum.VehicleInsurance)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.VehicleInsurance, "Vehicle Insurance"));
                }
                else if (subAcctType == AccountSubTypeEnum.VehicleLease)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.VehicleLease, "Vehicle Lease"));
                }
                else if (subAcctType == AccountSubTypeEnum.VehicleLoan)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.VehicleLoan, "Vehicle Loan"));
                }
                else if (subAcctType == AccountSubTypeEnum.VehicleLoanInterest)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.VehicleLoanInterest, "Vehicle Loan Interest"));
                }
                else if (subAcctType == AccountSubTypeEnum.VehicleRegistration)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.VehicleRegistration, "Vehicle Registration"));
                }
                else if (subAcctType == AccountSubTypeEnum.VehicleRepairs)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.VehicleRepairs, "Vehicle Repairs"));
                }
                else if (subAcctType == AccountSubTypeEnum.WashAndRoadServices)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherExpenses", AccountSubTypeEnum.WashAndRoadServices, "Wash And Road Services"));
                }
                else if (subAcctType == AccountSubTypeEnum.Checking)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.Checking, "Checking"));
                }
                else if (subAcctType == AccountSubTypeEnum.CashOnHand)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.CashOnHand, "Cash On Hand"));
                }
                else if (subAcctType == AccountSubTypeEnum.MoneyMarket)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.MoneyMarket, "Money Market"));
                }
                else if (subAcctType == AccountSubTypeEnum.RentsHeldInTrust)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.RentsHeldInTrust, "Rents Held In Trust"));
                }
                else if (subAcctType == AccountSubTypeEnum.Savings)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.Savings, "Savings"));
                }
                else if (subAcctType == AccountSubTypeEnum.TrustAccounts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.TrustAccounts, "Trust account"));
                }
                else if (subAcctType == AccountSubTypeEnum.TrustAccounts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Banks", AccountSubTypeEnum.TrustAccounts, "Trust account"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccountsReceivable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("AccountsReceivable", AccountSubTypeEnum.AccountsReceivable, "Accounts Receivable(A/R)"));
                }
                else if (subAcctType == AccountSubTypeEnum.AllowanceForBadDebts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.AllowanceForBadDebts, "Allowance For Bad Debts"));
                }
                else if (subAcctType == AccountSubTypeEnum.DevelopmentCosts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.DevelopmentCosts, "Development Costs"));
                }
                else if (subAcctType == AccountSubTypeEnum.EmployeeCashAdvances)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.EmployeeCashAdvances, "Employee Cash Advances"));
                }
                else if (subAcctType == AccountSubTypeEnum.Inventory)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.Inventory, "Inventory"));
                }
                else if (subAcctType == AccountSubTypeEnum.Investment_MortgageRealEstateLoans)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.Investment_MortgageRealEstateLoans, "Investment Mortgage Real EstateLoans"));
                }
                else if (subAcctType == AccountSubTypeEnum.Investment_TaxExemptSecurities)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.Investment_TaxExemptSecurities, "Investment - Tax-Exempt Securities"));
                }
                else if (subAcctType == AccountSubTypeEnum.Investment_USGovernmentObligations)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.Investment_USGovernmentObligations, "Investment - U.S.Government Obligations"));
                }
                else if (subAcctType == AccountSubTypeEnum.Investment_Other)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.Investment_Other, "Investment - Other"));
                }
                else if (subAcctType == AccountSubTypeEnum.LoansToOfficers)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.LoansToOfficers, "Loans To Officers"));
                }
                else if (subAcctType == AccountSubTypeEnum.LoansToOthers)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.LoansToOthers, "Loans To Others"));
                }
                else if (subAcctType == AccountSubTypeEnum.LoansToStockholders)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.LoansToStockholders, "Loans To Stockholders"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherCurrentAssets)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.OtherCurrentAssets, "Other Current Assets"));
                }
                else if (subAcctType == AccountSubTypeEnum.PrepaidExpenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.PrepaidExpenses, "Prepaid Expenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.Retainage)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.Retainage, "Retainage"));
                }
                else if (subAcctType == AccountSubTypeEnum.UndepositedFunds)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentAssets", AccountSubTypeEnum.UndepositedFunds, "Undeposited Funds"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccumulatedAmortization)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.AccumulatedAmortization, "Accumulated s Amortization"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccumulatedDepletion)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.AccumulatedDepletion, "Accumulated Depletion"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccumulatedDepreciation)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.AccumulatedDepreciation, "Accumulated Depreciation"));
                }
                else if (subAcctType == AccountSubTypeEnum.Buildings)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.Buildings, "Buildings"));
                }
                else if (subAcctType == AccountSubTypeEnum.DepletableAssets)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.DepletableAssets, "Depletable Assets"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetComputers)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetComputers, "Fixed Asset Computers"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetCopiers)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetCopiers, "Fixed Asset Copiers"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetFurniture)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetFurniture, "Fixed Asset Furniture"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetOtherToolsEquipment)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetOtherToolsEquipment, "Fixed Asset Other Tools Equipment"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetPhone)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetPhone, "Fixed Asset Phone"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetPhotoVideo)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetPhotoVideo, "Fixed Asset Photo Video"));
                }
                else if (subAcctType == AccountSubTypeEnum.FixedAssetSoftware)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FixedAssetSoftware, "Fixed Asset Software"));
                }
                else if (subAcctType == AccountSubTypeEnum.FurnitureAndFixtures)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.FurnitureAndFixtures, "Furniture And Fixtures"));
                }
                else if (subAcctType == AccountSubTypeEnum.IntangibleAssets)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.IntangibleAssets, "Intangible Assets"));
                }
                else if (subAcctType == AccountSubTypeEnum.Land)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.Land, "Land"));
                }
                else if (subAcctType == AccountSubTypeEnum.LeaseholdImprovements)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.LeaseholdImprovements, "Lease hold Improvements"));
                }
                else if (subAcctType == AccountSubTypeEnum.MachineryAndEquipment)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.MachineryAndEquipment, "Machinery And Equipment"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherFixedAssets)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.OtherFixedAssets, "Other Fixed Assets"));
                }
                else if (subAcctType == AccountSubTypeEnum.Vehicles)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.Vehicles, "Vehicles"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccumulatedAmortizationOfOtherAssets)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.AccumulatedAmortizationOfOtherAssets, "Accumulated Amortization Of Other Assets"));
                }
                else if (subAcctType == AccountSubTypeEnum.Goodwill)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.Goodwill, "Goodwill"));
                }
                else if (subAcctType == AccountSubTypeEnum.LeaseBuyout)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.LeaseBuyout, "Lease Buyout"));
                }
                else if (subAcctType == AccountSubTypeEnum.Licenses)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.Licenses, "Licenses"));
                }
                else if (subAcctType == AccountSubTypeEnum.OrganizationalCosts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.OrganizationalCosts, "Organizational Costs"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherLongTermAssets)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.OtherLongTermAssets, "Other Long-Term Assets"));
                }
                else if (subAcctType == AccountSubTypeEnum.SecurityDeposits)
                {
                    TotalTaxTypeList.Add(new SubAcctType("FixedAssets", AccountSubTypeEnum.SecurityDeposits, "Security Deposits"));
                }
                else if (subAcctType == AccountSubTypeEnum.CreditCard)
                {
                    TotalTaxTypeList.Add(new SubAcctType("CreditCard", AccountSubTypeEnum.CreditCard, "Credit Card"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccountsPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("AccountsPayable", AccountSubTypeEnum.AccountsPayable, "Account Payable(A/P)"));
                }
                else if (subAcctType == AccountSubTypeEnum.DeferredRevenue)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.DeferredRevenue, "Deferred Revenue"));
                }
                else if (subAcctType == AccountSubTypeEnum.DirectDepositPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.DirectDepositPayable, "Direct Deposit Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.FederalIncomeTaxPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.FederalIncomeTaxPayable, "Federal Income Tax Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.InsurancePayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.InsurancePayable, "Insurance Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.LineOfCredit)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.LineOfCredit, "Line Of Credit"));
                }
                else if (subAcctType == AccountSubTypeEnum.LoanPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.LoanPayable, "Loan Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherCurrentLiabilities)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.OtherCurrentLiabilities, "Other Current Liabilities"));
                }
                else if (subAcctType == AccountSubTypeEnum.PayrollClearing)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.PayrollClearing, "Payroll Clearing"));
                }
                else if (subAcctType == AccountSubTypeEnum.PayrollTaxPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.PayrollTaxPayable, "Payroll Tax Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.PrepaidExpensesPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.PrepaidExpensesPayable, "Prepaid Expenses Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.RentsInTrustLiability)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.RentsInTrustLiability, "Rents in trust - Liability"));
                }
                else if (subAcctType == AccountSubTypeEnum.SalesTaxPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.SalesTaxPayable, "Sales Tax Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.GlobalTaxSuspense)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.GlobalTaxSuspense, "Global Tax Suspense"));
                }
                else if (subAcctType == AccountSubTypeEnum.StateLocalIncomeTaxPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.StateLocalIncomeTaxPayable, "State/Local Income Tax Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.TrustAccounts)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.TrustAccounts, "Trust Accounts"));
                }
                else if (subAcctType == AccountSubTypeEnum.Gratuity)
                {
                    TotalTaxTypeList.Add(new SubAcctType("OtherCurrentLiabilities", AccountSubTypeEnum.Gratuity, "Undistributed Tips"));
                }
                else if (subAcctType == AccountSubTypeEnum.NotesPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("LongTermLiabilites", AccountSubTypeEnum.NotesPayable, "Notes Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.OtherLongTermLiabilities)
                {
                    TotalTaxTypeList.Add(new SubAcctType("LongTermLiabilites", AccountSubTypeEnum.OtherLongTermLiabilities, "Other Long Term Liabilities"));
                }
                else if (subAcctType == AccountSubTypeEnum.ShareholderNotesPayable)
                {
                    TotalTaxTypeList.Add(new SubAcctType("LongTermLiabilites", AccountSubTypeEnum.ShareholderNotesPayable, "Share holder Notes Payable"));
                }
                else if (subAcctType == AccountSubTypeEnum.AccumulatedAdjustment)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.AccumulatedAdjustment, "Accumulated Adjustment"));
                }
                else if (subAcctType == AccountSubTypeEnum.CommonStock)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.CommonStock, "Common Stock"));
                }
                else if (subAcctType == AccountSubTypeEnum.EstimatedTaxes)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.EstimatedTaxes, "Estimated Taxes"));
                }
                else if (subAcctType == AccountSubTypeEnum.Healthcare)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.Healthcare, "Health Insurance Premium"));
                }
                else if (subAcctType == AccountSubTypeEnum.HealthSavingsAccountContributions)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.HealthSavingsAccountContributions, "Health Savings Account Contributions"));
                }
                else if (subAcctType == AccountSubTypeEnum.OpeningBalanceEquity)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.OpeningBalanceEquity, "Opening Balance Equity"));
                }
                else if (subAcctType == AccountSubTypeEnum.OwnersEquity)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.OwnersEquity, "Owners Equity"));
                }
                else if (subAcctType == AccountSubTypeEnum.PaidInCapitalOrSurplus)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PaidInCapitalOrSurplus, "Paid-In Capital Or Surplus"));
                }
                else if (subAcctType == AccountSubTypeEnum.PartnerContributions)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PartnerContributions, "Partner Contributions"));
                }
                else if (subAcctType == AccountSubTypeEnum.PartnerDistributions)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PartnerDistributions, "Partner Distributions"));
                }
                else if (subAcctType == AccountSubTypeEnum.PartnerDistributions)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PartnerDistributions, "Partner Distributions"));
                }
                else if (subAcctType == AccountSubTypeEnum.PartnersEquity)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PartnersEquity, "Partner's Equity"));
                }
                else if (subAcctType == AccountSubTypeEnum.PersonalExpense)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PersonalExpense, "Personal Expense"));
                }
                else if (subAcctType == AccountSubTypeEnum.PersonalIncome)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PersonalIncome, "Personal Income"));
                }
                else if (subAcctType == AccountSubTypeEnum.PreferredStock)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.PreferredStock, "Preferred Stock"));
                }
                else if (subAcctType == AccountSubTypeEnum.RetainedEarnings)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.RetainedEarnings, "Retained Earnings"));
                }
                else if (subAcctType == AccountSubTypeEnum.TreasuryStock)
                {
                    TotalTaxTypeList.Add(new SubAcctType("Equity", AccountSubTypeEnum.TreasuryStock, "Treasury Stock"));
                }
            }
        }


        public void setDetails(int id)
        {
            // Income, Expenses, Banks, Assets, Credit cards, Liabilities, Equtiy
            //accountingModelObj.LoadAccountingList();

            //UserName.GetTextBox().Text = user.username;
            //Password.GetTextBox().Text = user.password;

            //this.userId = user.id;
            //pesmOrder.GetCheckBox().Checked = user.permissionOrders;
            //pesmInventory.GetCheckBox().Checked = user.permissionInventory;
            //pesmReceivable.GetCheckBox().Checked = user.permissionReceivables;
            //pesmSetting.GetCheckBox().Checked = user.permissionSetting;
            //pesmUser.GetCheckBox().Checked = user.permissionUsers;
            //pesmQBLink.GetCheckBox().Checked = user.permissionQuickBooks;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveAccounting(sender, e);
                }
                else if (result == DialogResult.No)
                {
                    _navigateToPrev(sender, e);
                }
            }
        }

        private string AddSpace(int count)
        {
            string prefixStr = "    ";
            while (count > 0)
            {
                prefixStr = prefixStr + "    ";
                count--;
            }
            return prefixStr;
        }

        private void AcctTypeChanged(object sender, EventArgs e)
        {
            var selectedId = AccountType.GetComboBox().SelectedValue;
            int acctTypeId = int.Parse(selectedId.ToString());
            List<model.Accounting> tempAcctList = new List<model.Accounting>();
            tempAcctList.Clear();
            BalanceLabel.GetLabel().Visible = false;
            StartingDate.GetLabel().Visible = false;
            StartingDate.GetDateTimePicker().Visible = false;
            Balance.GetLabel().Visible = false;
            Balance.GetTextBox().Visible = false;
            switch (acctTypeId)
            {
                case 0:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("Income")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("OtherIncome")).ToList());
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                case 1:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("CostofGoodsSold")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("Expense")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("OtherExpense")).ToList());
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                case 2:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("Bank")).ToList());
                    BalanceLabel.GetLabel().Visible = true;
                    StartingDate.GetLabel().Visible = true;
                    StartingDate.GetDateTimePicker().Visible = true;
                    Balance.GetLabel().Visible = true;
                    Balance.GetTextBox().Visible = true;
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                case 3:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("AccountsReceivable")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("OtherCurrentAsset")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("FixedAsset")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("OtherCurrentAsset")).ToList());
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                case 4:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("CreditCard")).ToList());
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                case 5:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("AccountsPayable")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("OtherCurrentLiability")).ToList());
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("LongTermLiability")).ToList());
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                case 6:
                    tempAcctList.AddRange(TotalAccountList.Where(item => item.AccountType.Equals("Equity")).ToList());
                    SaveAccountUnder.GetComboBox().DataSource = tempAcctList;
                    break;
                default:
                    break;
            }
        }

        private void SaveAccountUnderChanged(object sender, EventArgs e)
        {
            model.Accounting accountant = SaveAccountUnder.GetComboBox().SelectedItem as model.Accounting;
            TaxFormSection.GetComboBox().Enabled = true;
            AccountTypeEnum acctType1 = accountant.AccountType;
            string acctType = acctType1.ToString();
            List<SubAcctType> taxFormList = new List<SubAcctType>();
            if (acctType.Equals("Income"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("Income")).ToList();
            }
            else if (acctType.Equals("OtherIncome"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("OtherIncome")).ToList();
            }
            else if (acctType.Equals("CostofGoodsSold"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("CostofGoodsSold")).ToList();
            }
            else if (acctType.Equals("Expense"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("CostofGoodsSold")).ToList();
            }
            else if (acctType.Equals("OtherExpenses"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("Expenses")).ToList();
            }
            else if (acctType.Equals("Bank"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("Banks")).ToList();
            }
            else if (acctType.Equals("AccountsReceivable"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("AccountsReceivable")).ToList();
            }
            else if (acctType.Equals("OtherCurrentAsset"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("OtherCurrentAssets")).ToList();
            }
            else if (acctType.Equals("FixedAsset"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("FixedAsset")).ToList();
            }
            else if (acctType.Equals("OtherAsset"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("OtherAsset")).ToList();
            }
            else if (acctType.Equals("CreditCard"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("CreditCard")).ToList();
            }
            else if (acctType.Equals("AccountsPayable"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("AccountsPayable")).ToList();
            }
            else if (acctType.Equals("OtherCurrentLiability"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("OtherCurrentLiabilities")).ToList();
            }
            else if (acctType.Equals("LongTermLiability"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("LongTermLiabilites")).ToList();
            }
            else if (acctType.Equals("Equity"))
            {
                taxFormList = TotalTaxTypeList.Where(item => item.acctType.Equals("Equity")).ToList();
            }

            TaxFormSection.GetComboBox().DataSource = taxFormList;
        }

        private void SaveAccounting(object sender, KeyEventArgs e)
        {
            model.Accounting accountant = SaveAccountUnder.GetComboBox().SelectedItem as model.Accounting;
            AccountTypeEnum acctType = accountant.AccountType;
            SubAcctType subAcctType = TaxFormSection.GetComboBox().SelectedItem as SubAcctType;
            string taxForm = subAcctType.acctSubType.ToString();
            string accountName = AccountName.GetTextBox().Text.ToString();
            string acctNum = AccountName.GetTextBox().Text.ToString();
            string desc = Description.GetTextBox().Text.ToString();

            DateTime balanceDate = StartingDate.GetDateTimePicker().Value;
            decimal balanceAmount = decimal.Parse(Balance.GetTextBox().Text);

            //QboApiService qboClient = new QboApiService();
            //qboClient.CreateAccounting(accountName, acctNum, acctType, subAcctType);
        }
    }
}
