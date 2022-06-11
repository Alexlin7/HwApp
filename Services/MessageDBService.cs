using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using HwApp1410931031.Models;

namespace HwApp1410931031.Services
{
    public class MessageDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["BsTest"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢留言陣列資料
        //根據分頁以及搜尋來取得資料陣列的方法
        public List<Message> GetDataList(ForPaging Paging, int A_Id)
        {
            List<Message> DataList = new List<Message>();
            SetMaxPaging(Paging, A_Id);
            DataList = GetAllDataList(Paging, A_Id);
            return DataList;
        }
        #endregion

        #region 設定頁數
        public void SetMaxPaging(ForPaging Paging, int A_Id)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Message where A_Id = {A_Id}";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion

        #region 取得Message資料
        public List<Message> GetAllDataList(ForPaging paging, int A_Id)
        {
            //宣告要回傳的搜尋資料為資料庫中的Message資料表
            List<Message> DataList = new List<Message>();
            //Sql語法
            string sql = $@" select m.*, d.Name from (select row_number() over(order by M_Id) as sort,* from Message where A_Id = {A_Id}) m inner join Members d on m.Account = d.Account Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Message Data = new Message();
                    Data.M_Id = Convert.ToInt32(dr["M_Id"]);
                    Data.A_Id = Convert.ToInt32(dr["A_Id"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Content = dr["Content"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }
        #endregion

        #region 新增文章留言
        //新增文章留言方法
        public void InsertMessage(Message newData)
        {
            //取得最新一筆A_Id
            newData.M_Id = LastMessageFinder(newData.A_Id);
            //Sql新增語法
            //設定新增時間為現在
            string sql = $@" INSERT INTO Message(A_Id,M_Id,Account,Content,CreateTime)VALUES ( '{newData.A_Id}','{newData.M_Id}','{newData.Account}','{newData.Content}','{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' ) ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 計算目前留言最新一筆的M_Id
        public int LastMessageFinder(int A_Id)
        {
            //宣告要回傳的值
            int Id;
            //Sql查詢語法
            string sql = $@" select top 1 * from Message where A_Id = {A_Id} order by M_Id desc";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                //取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["M_Id"]);
            }
            catch (Exception e)
            {
                //沒資料時Id為0
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion

        #region 修改留言
        public void UpdateMessage(Message UpdateData)
        {
            //Sql修改語法
            string sql = $@" update Message set Content = '{UpdateData.Content}' where A_Id = {UpdateData.A_Id} and M_Id = {UpdateData.M_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 刪除留言
        public void DeleteMessage(int A_Id, int M_Id)
        {
            //Sql刪除語法
            string DeleteMessage = $@" Delete from Message where A_Id = {A_Id} and M_Id = {M_Id}";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(DeleteMessage, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

    }
}