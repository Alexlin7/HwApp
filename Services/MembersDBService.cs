using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HwApp1410931031.Models;
using Microsoft.Ajax.Utilities;

namespace HwApp1410931031.Services
{
    public class MembersDBService
    {
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BsTest"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 註冊

        public void Register(Members newMember)
        {
            // hash password
            newMember.Password = HashPassword(newMember.Password);

            //SQL 
            string sql = $@" INSERT INTO Members 
                                (Account,Password,Name,Email,AuthCode,IsAdmin) 
                                VALUES ('{newMember.Account}',
                                        '{newMember.Password}',
                                        '{newMember.Name}',
                                        '{newMember.Email}',
                                        '{newMember.AuthCode}',
                                        '0') " ;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region HashPassword

        public string HashPassword(string Password)
        {
            string saltkey = "yfahf93ya479ya98@!9h03jaw099gu04h0892fk29-j0g";
            string saltAndPassword = String.Concat(Password, saltkey);
            SHA256CryptoServiceProvider sha256hasher = new SHA256CryptoServiceProvider();
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            byte[] HashDate = sha256hasher.ComputeHash(PasswordData);
            string Hashresult = Convert.ToBase64String(HashDate);
            return Hashresult;
        }
        #endregion

        #region 查詢一筆資料
        // 藉由帳號來查詢
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();

            string sql = $@" select * from Members where Account = '{Account} ";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }

            return Data;
        }
        #endregion

        #region 帳號重複註冊確認

        public bool AccountCheck(string Account)
        {
            Members Data = GetDataByAccount(Account);
            return Data == null;
        }
        #endregion

        #region 信箱驗證

        public string EmailValidate(string Account, string AuthCode)
        {
            Members validateMember = GetDataByAccount(Account);
            string ValidateStr = string.Empty;
            if (ValidateStr != null)
            {
                if (validateMember.AuthCode == AuthCode)
                {
                    string sql = $@" update Members set AuthCode = '{string.Empty}' where Account = '{Account}'";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message.ToString());
                    }
                    finally
                    {
                        conn.Close();
                    }

                    ValidateStr = "帳號驗證成功，現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再註冊";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或註冊";
            }

            return ValidateStr;
        }
        #endregion


    }
}