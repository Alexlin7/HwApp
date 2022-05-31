using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using HwApp1410931031.Models;

namespace HwApp1410931031.Services
{
    public class GuestbooksDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BsTest"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql 語法
            string sql = string.Empty;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                // 有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }
            else
            {
                // 無搜尋條件時
                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }

            return DataList;
        }

        public List<Guestbooks> GetDataList(string Search)
        {


            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql 語法
            string sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                // 有搜尋條件時
                sql = $@" SELECT * FROM Guestbooks WHERE Account LIKE '%{Search}%' OR Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%'; ";
            }
            else
            {
                // 無搜尋條件時
                sql = @" SELECT * FROM Guestbooks; ";
            }

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
                    Guestbooks Data = new Guestbooks
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Account = dr["Account"].ToString(),
                        Content = dr["Content"].ToString(),
                        CreateTime = Convert.ToDateTime(dr["CreateTime"])
                    };
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
            string sql = $@" INSERT INTO Guestbooks(Account,Content,CreateTime)
                                VALUES ( '{newData.Account}',
                                         '{newData.Content}',
                                         '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' ) ";
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
            string sql = $@" SELECT * FROM Guestbooks m inner join Members d on m.Account = d.Account where Id = {Id} ";
            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                conn.Open(); // 開啟資料庫連線
                SqlCommand cmd = new SqlCommand(sql, conn); // 執行Sql 指令
                SqlDataReader dr = cmd.ExecuteReader(); // 取得Sql 資料
                dr.Read();
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Account = dr["Account"].ToString();
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

                Data.Member.Name = dr["Name"].ToString();
            }
            catch (Exception e)
            {
                // 查無資料
                Console.WriteLine(e);
                Data = null;
            }
            finally
            {
                // 關閉資料庫連線
                conn.Close();
            }
            // 回傳根據編號所取得的資料
            return Data;
        }
        #endregion

        #region 修改留言
        // 修改留言方法
        public void UpdateGuestbooks(Guestbooks UpdateData)
        { //Sql 修改語法
            string sql = $@" UPDATE Guestbooks SET Account = '{ UpdateData.Account}', Content = '{UpdateData.Content}' where Id = {UpdateData.Id} ";
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
            string sql = $@" UPDATE Guestbooks SET Reply = '{ReplyData.Reply}', ReplyTime = '{DateTime.Now:yyyy/MM/dd HH:mm:ss}' WHERE Id = {ReplyData.Id}; ";
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

        #region 刪除資料
        // 刪除資料方法
        public void DeleteGuestbooks(int Id)
        {
            //Sql 刪除語法
            // 根據Id 取得要刪除的資料
            string sql = $@" DELETE FROM Guestbooks WHERE Id = {Id}; ";
            try// 確保程式不會因執行錯誤而整個中斷
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

        // 無搜尋值的搜尋資料方法
        #region 設定最大頁數方法
        // 無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            // 計算列數
            int Row = 0;
            //Sql 語法
            string sql = $@" SELECT * FROM Guestbooks; ";
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
                    Row++;
                }
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
            // 計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            // 重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        // 有搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Search)
        {
            // 計算列數
            int Row = 0;
            //Sql 語法
            string sql = $@" SELECT * FROM Guestbooks WHERE  Content LIKE '%{Search}%' OR Reply LIKE '%{Search}%'; ";
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
                    Row++;
                }
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
            // 計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            // 重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion

        #region 搜尋資料方法
        // 無搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging)
        {
            // 宣告要回傳的搜尋資料為資料庫中的Guestbooks 資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql 語法
            string sql = $@" SELECT m.*,d.Name,d.IsAdmin from (select row_number() 
                        over(order by Id) as sort,* from Guestbooks ) m 
                        inner join Members d on m.Account = d.Account 
                        Where m.sort 
                        Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum}";
            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                // 開啟資料庫連線
                conn.Open();
                // 執行Sql 指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                // 取得Sql 資料
                SqlDataReader dr = cmd.ExecuteReader(); while (dr.Read()) // 獲得下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Account = dr["Account"].ToString(),
                        Content = dr["Content"].ToString(),
                        CreateTime = Convert.ToDateTime(dr["CreateTime"])
                    };
                    // 確定此則留言是否回覆，且不允許空白
                    // 因C# 是強型別語言，所以轉換時Datetime 型態不允許存取null
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }

                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                // 丟出錯誤
                throw new Exception(e.Message);
            }
            finally
            {
                // 關閉資料庫連線
                conn.Close();
            }
            // 回傳搜尋資料
            return DataList;
        }
        

        // 有搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging, string Search)
        {
            // 宣告要回傳的搜尋資料為資料庫中的Guestbooks 資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql 語法
            string sql =
                $@" SELECT m.*,d.Name,d.IsAdmin from (select row_number() 
                        over(order by Id) as sort,* from Guestbooks 
                        where Content like '%{Search}' or Reply like '%{Search}' ) m 
                        inner join Members d 
                        on m.Account = d.Account 
                        Where m.sort 
                        Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum}";
            // 確保程式不會因執行錯誤而整個中斷
            try
            {
                // 開啟資料庫連線
                conn.Open();
                // 執行Sql 指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                // 取得Sql 資料
                SqlDataReader dr = cmd.ExecuteReader(); while (dr.Read()) // 獲得下一筆資料直到沒有資料
                {
                    Guestbooks Data = new Guestbooks
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Account = dr["Account"].ToString(),
                        Content = dr["Content"].ToString(),
                        CreateTime = Convert.ToDateTime(dr["CreateTime"])
                    };
                    // 確定此則留言是否回覆，且不允許空白 (因C# 是強型別語言，所以轉換時Datetime 型態不允許存取null)
                    if (!dr["ReplyTime"].Equals(DBNull.Value))
                    {
                        Data.Reply = dr["Reply"].ToString();
                        Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                    }

                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString()); // 丟出錯誤
            }
            finally
            {
                conn.Close(); // 關閉資料庫連線
            }
            return DataList; // 回傳搜尋資料
        }
        #endregion


    }
}