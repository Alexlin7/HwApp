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
                                (Account,Password,Name,Image,Email,AuthCode,IsAdmin) 
                                VALUES ('{newMember.Account}',
                                        '{newMember.Password}',
                                        '{newMember.Name}',
                                        '{newMember.Image}',
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

            string sql = $@" select * from Members where Account = '{Account}' ";

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
                Console.WriteLine(e);
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

        #region 登入確認

        public string LoginCheck(string Account, string Password)
        {
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember != null)
            {
                if (String.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未通過Email認證，請去收信";
                }
            }
            return "查無此帳號，請註冊";
        }
        #endregion

        #region 密碼確認

        public bool PasswordCheck(Members CheckMember, string Password)
        {
            return CheckMember.Password.Equals(HashPassword(Password));
        }
        #endregion

        #region 取得角色

        public string GetRole(string Account)
        {
            string Role = "user";
            Members LoginMember = GetDataByAccount(Account);
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin";
            }

            return Role;
        }
        #endregion

        #region 變更密碼

        public string ChangePassword(string Account, string Password, string newPassword)
        {
            Members LoginMember = GetDataByAccount(Account);
            if (PasswordCheck(LoginMember, Password))
            {
                LoginMember.Password = HashPassword(newPassword);
                string sql = $@" update Members set Password = '{LoginMember.Password}' where Account = '{Account}'";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    conn.Close();
                }

                return "密碼修改成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion

        #region 查詢一筆公開性資料
        public Members GetDatabyAccount(string Account)
        {
            Members Data = new Members();

            string sql = $@" select * from Members where Account = '{Account}' ";

            try
            {

                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read(); 
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Account = dr["Account"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 檢查圖片類型
        public bool CheckImage(string ContentType)
        {
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
            }
            return false;
        }
        #endregion
    }
}