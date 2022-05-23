using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public class GuestbooksDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BsTest"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        public List<Guestbooks> GetDataList()
        {

            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql 語法
            string sql = @" SELECT * FROM Guestbooks; ";
            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                // 開啟資料庫連線
                conn.Open();
                // 執行Sql 指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                // 取得Sql 資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) // 獲得下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks();
                    Data.Id = Convert.ToInt32(dr["Id"]);
                    Data.Name = dr["Name"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    // 確定此則留言是否回覆，且不允許空白
                    // 因C# 是強型別語言，所以轉換時Datetime 型態不允許存取null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime
                        (dr["ReplyTime"]);
                    }
                    DataList.Add(Data);
                } // while結束
            }
            catch (Exception e)
            {
                // 丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                // 關閉資料庫連線
                conn.Close();
            }
            return DataList
            ;
        }

        #region 新增資料
        // 新增資料方法
        public void InsertGuestbooks(Guestbooks newData)
        {
            //Sql 新增語法
            // 設定新增時間為現在
            string sql = $@" INSERT INTO Guestbooks(Name,Content,CreateTime)
                                VALUES ( '{newData.Name}','{newData.Content}','{DateTime.Now:yyyy/MM/dd HH:mm:ss}' ) ";
            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                // 開啟資料庫連線
                conn.Open();
                // 執行Sql 指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                // 丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                // 關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 查詢一筆資料
        // 藉由編號取得單筆資料的方法
        public Guestbooks GetDataById(int Id)
        {
            Guestbooks Data = new Guestbooks();
            //Sql 語法
            string sql = $@" SELECT * FROM Guestbooks WHERE Id = {Id}; ";
            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                conn.Open(); // 開啟資料庫連線
                SqlCommand cmd = new SqlCommand(sql, conn); // 執行Sql 指令
                SqlDataReader dr = cmd.ExecuteReader(); // 取得Sql 資料
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Name = dr["Name"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                // 確定此則留言是否回覆，且不允許空白
                if (!string.IsNullOrWhiteSpace
                (dr["Reply"].ToString()))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime
                    (dr["ReplyTime"]);
                }
            }
            catch (Exception e)
            {
                // 查無資料
                Data = null;
            }
            finally
            {
                // 關閉資料庫連線
                conn.Close();
            }
            // 回傳根據編號所取得的資料
            return Data
            ;
        }
        #endregion

        #region 修改留言
        // 修改留言方法
        public void UpdateGuestbooks(Guestbooks UpdateData)
        { //Sql 修改語法
            string sql = $@" UPDATE Guestbooks SET Name = '{ UpdateData.Name}', Content = '{UpdateData.Content}' where Id = {UpdateData.Id} ";
            try // 確保程式不會因執行錯誤而整個中斷
            {
                conn.Open(); // 開啟資料庫連線
                SqlCommand cmd = new SqlCommand(sql, conn); // 執行Sql 指令
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString()); // 丟出錯誤
            }
            finally
            {
                conn.Close(); // 關閉資料庫連線
            }
        }
        #endregion

        #region 檢查相關
        // 修改資料判斷的方法
        public bool CheckUpdate(int Id)
        {
            // 根據Id 取得要修改的資料
            Guestbooks Data = GetDataById(Id);
            // 判斷並回傳( 判斷是否有資料以及是否有回覆)
            return (Data != null && Data.ReplyTime == null);
        }
        #endregion

        #region 回覆留言
        // 回覆留言方法
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            //Sql 修改語法; 設定回覆時間為現在
            string sql = $@" UPDATE Guestbooks SET Reply = '{ReplyData.Reply}', ReplyTime = '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE Id = {ReplyData.Id}; ";
            try // 確保程式不會因執行錯誤而整個中斷
            {
                conn.Open(); // 開啟資料庫連線
                SqlCommand cmd = new SqlCommand(sql, conn); // 執行Sql 指令
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString()); // 丟出錯誤
            }
            finally
            {
                conn.Close(); // 關閉資料庫連線
            }
        }
        #endregion
    }
}