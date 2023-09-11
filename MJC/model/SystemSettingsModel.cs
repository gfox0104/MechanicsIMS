﻿using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Reflection.Emit;
using MJC.config;
using static System.Windows.Forms.AxHost;

namespace MJC.model
{
    public struct SystemSettings
    {
        public int? taxCodeId { get; set; }
        public string businessName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string ein { get; set; }
        public string targetPrinter { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public bool trainingEnabled { get; set; }
        public string businessDescription { get; set; }

    }

    public class SystemSettingsModel : DbConnection
    {
        public SystemSettings Settings { get; private set; }

        public bool LoadSettings()
        {
            // Default settings if no settings exist in database.
            Settings = new SystemSettings()
            {
                businessName = "DEFAULT COMPANY NAME"
            };

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select taxCodeId,companyName,description,address1,address2,city,state,zipcode,phone,fax,federalTaxNumber,trainingMode,targetPrinter,authorizationCode,accessToken,refreshToken
                                            from dbo.SystemSettings";

                    var reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        var taxCodeId = reader.GetValue(0) as int?;
                        var businessName = reader.GetString(1);
                        var description = reader.GetValue(2);
                        var address1 = reader.GetString(3);
                        var address2 = reader.GetString(4);
                        var city = reader.GetString(5);
                        var state = reader.GetString(6);
                        var zipCode = reader.GetString(7);
                        var phone = reader.GetString(8);
                        var fax = reader.GetString(9);
                        var ein = reader.GetString(10);
                        var training = reader.GetBoolean(11);
                        var targetPrinter = reader.GetString(12);
                        var authorizationCode = reader.GetValue(13)?.ToString();
                        var refreshToken = reader.GetValue(14)?.ToString();

                        Settings = new SystemSettings()
                        {
                            taxCodeId = taxCodeId,
                            businessName = businessName,
                            address1 = address1,
                            address2 = address2,
                            city = city,
                            state = state,
                            postalCode = zipCode,
                            phone = phone,
                            fax = fax,
                            ein = ein,
                            trainingEnabled = training,
                            targetPrinter = targetPrinter,
                            accessToken = authorizationCode,
                            refreshToken = refreshToken
                        };
                    }


                }
            }

            return true;
        }

        public bool SaveSetting(int? taxCodeId, string companyName, string description, string address1, string address2, string city, string state, string zipcode, string phone, string fax, string federalTaxNumber, bool trainingMode, string targetPrinter, string authorizationCode, string refreshToken)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"TRUNCATE TABLE dbo.SystemSettings";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO dbo.SystemSettings (companyName,description,address1,address2,city,state,zipcode,phone,fax,federalTaxNumber,trainingMode,targetPrinter,accessToken,refreshToken,taxCodeId) Values(@Value1,@description,@Value2,@Value3,@Value4,@Value5,@Value6,@Value7,@Value8,@Value9,@Value10,@Value11,@Value12,@Value13,@Value14)";
                    command.Parameters.AddWithValue("@Value1", companyName);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@Value2", address1);
                    command.Parameters.AddWithValue("@Value3", address2);
                    command.Parameters.AddWithValue("@Value4", city);
                    command.Parameters.AddWithValue("@Value5", state);
                    command.Parameters.AddWithValue("@Value6", zipcode);
                    command.Parameters.AddWithValue("@Value7", phone);
                    command.Parameters.AddWithValue("@Value8", fax);
                    command.Parameters.AddWithValue("@Value9", federalTaxNumber);
                    command.Parameters.AddWithValue("@Value10", trainingMode);
                    command.Parameters.AddWithValue("@Value11", targetPrinter);
                    command.Parameters.AddWithValue("@Value12", authorizationCode);
                    command.Parameters.AddWithValue("@Value13", refreshToken);
                    command.Parameters.AddWithValue("@Value14", taxCodeId != null ? taxCodeId : DBNull.Value );
                    command.ExecuteScalar();
                }

                return true;
            }
        }

    }
}