using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SHJXMESWCFServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class MESService : IMESService

    {
        // Token: 0x04000021 RID: 33
        private string login_user;

        // Token: 0x04000022 RID: 34
        private int _cur = 0;

        // Token: 0x04000023 RID: 35
        private DataTable _dtprocess = new DataTable();

        // Token: 0x04000024 RID: 36
        private DataTable _dtItems = new DataTable();

        // Token: 0x04000025 RID: 37
        private bool _islast = false;

        // Token: 0x04000026 RID: 38
        private string userno;

        // Token: 0x04000027 RID: 39
        private string _user;

        // Token: 0x04000028 RID: 40
        private string _duty;

        // Token: 0x04000029 RID: 41
        private string _scanner;

        // Token: 0x0400002A RID: 42
        private string userDptType;

        // Token: 0x0400002B RID: 43
        private string userDptName;

        // Token: 0x0400002C RID: 44
        private int _scanid;
        private bool btn_start;
        private bool btn_submit;
        private bool btn_retrieve;

        // Token: 0x0400002D RID: 45
        private int _kbid;

        // Token: 0x0400002E RID: 46
        private string _boardNo;

        // Token: 0x0400002F RID: 47
        private int _lineID;

        // Token: 0x04000030 RID: 48
        private string _statusname;

        // Token: 0x04000031 RID: 49
        private int _status;

        // Token: 0x04000032 RID: 50
        private int _next_status;

        // Token: 0x04000033 RID: 51
        private string _next_statusname;
        private string tb_Board;
        private string lb_operName;
        private string tb_cPro;
        private string tb_nPro;

        public string Err { get; private set; }
        public string Login_user { get => login_user; set => login_user = value; }
        public int Cur { get => _cur; set => _cur = value; }
        public DataTable Dtprocess { get => _dtprocess; set => _dtprocess = value; }
        public DataTable DtItems { get => _dtItems; set => _dtItems = value; }
        public bool Islast { get => _islast; set => _islast = value; }
        public string Userno { get => userno; set => userno = value; }
        public string User { get => _user; set => _user = value; }
        public string Duty { get => _duty; set => _duty = value; }
        public string Scanner { get => _scanner; set => _scanner = value; }
        public string UserDptType { get => userDptType; set => userDptType = value; }
       
        public string UserDptName { get => userDptName; set => userDptName = value; }
        public int Scanid { get => _scanid; set => _scanid = value; }
        public bool Btn_start { get => btn_start; set => btn_start = value; }
        public bool Btn_submit { get => btn_submit; set => btn_submit = value; }
        public bool Btn_retrieve { get => btn_retrieve; set => btn_retrieve = value; }
        public int Kbid { get => _kbid; set => _kbid = value; }
        public string BoardNo { get => _boardNo; set => _boardNo = value; }
        public int LineID { get => _lineID; set => _lineID = value; }
        public string Statusname { get => _statusname; set => _statusname = value; }
        public int Status { get => _status; set => _status = value; }
        public int Next_status { get => _next_status; set => _next_status = value; }
        public string Next_statusname { get => _next_statusname; set => _next_statusname = value; }
        public string Tb_Board { get => tb_Board; set => tb_Board = value; }
        public string Lb_operName { get => lb_operName; set => lb_operName = value; }
        public string Tb_cPro { get => tb_cPro; set => tb_cPro = value; }
        public string Tb_nPro { get => tb_nPro; set => tb_nPro = value; }

        public DataTable GetProcess(int id)
        {
            string sql = "select a.fInterID,fConfigID,fProTecID,fCode,fName from LKM_MCCPTEntry a left join lkm_CommonBill b on a.fProTecID=b.fInterID where fConfigID=" + id + "order by fOrders asc";
            return (DataTable)DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);

        }

        public DataTable GetSeries(string number)
        {
            string sql = "select * from vw_lines where ftype = 1 and  fitemcode in (' + number + ')";
            return (DataTable)DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);
        }

        public int HandleData(int type)
        {
            SqlParameter[] array = new SqlParameter[]
             {
                new SqlParameter("@kbID",SqlDbType.BigInt ),
                new SqlParameter("@curKBID", SqlDbType.BigInt),
                new SqlParameter("@bustype", SqlDbType.Int),
                new SqlParameter("@opertype", SqlDbType.Int),
                new SqlParameter("@lineID", SqlDbType.Int),
                new SqlParameter("@cStatus", SqlDbType.Int),
                new SqlParameter("@nStatus", SqlDbType.Int),
                new SqlParameter("@operName",SqlDbType.NVarChar, 50)
             };
            array[0].Value = this.Kbid;
            array[1].Value = this.Scanid;
            array[2].Value = 1;
            array[3].Value = type;
            array[4].Value = this.LineID;
            array[5].Value = this.Status;
            array[6].Value = this.Next_status;
            array[7].Value = this.User;
            return DataHelper.ExecuteSqlprocedure("pro_ExecKanbanScanHandle_lansq", DataHelper.CreateSqlConn("shjxmes"));
            
        }

        public string Scan()
        {
            if (this.Tb_Board.IndexOf("-") > -1)
            {
                this.BoardNo = this.Tb_Board;
                string a = this.BoardNo.Substring(0, 1);
                if (a != "S")
                {
                   
                    this.Tb_Board = "";                   
                    Err = "错误提示" + "请扫描生产看板";
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder("select vv.finterid,isnull(vv.fCurScanID,0) as fCurScanID,vv.fbillno,vv.fstatus,vv.fstatuName from lkm_exec_kanban v inner join lkm_exec_kanban_entry vv on v.finterid = vv.fmainid ");
                    stringBuilder.Append(" where v.fstatus = 1 and vv.fstatus != 2 and  vv.fbillno='" + this.BoardNo + "'");
                    DataTable dataTable = (DataTable)DataHelper.ExecuteSql(stringBuilder.ToString(),DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        this.Status = int.Parse(dataTable.Rows[0]["fstatus"].ToString());
                        this.Statusname = dataTable.Rows[0]["fstatuName"].ToString();
                        this.Kbid = int.Parse(dataTable.Rows[0]["finterid"].ToString());
                        this.Scanid = int.Parse(dataTable.Rows[0]["fCurScanID"].ToString());
                        this.Btn_start = (this.Status == 0 || this.Status == 1);
                        this.Btn_submit = (this.Status != 0 && this.Status != 1 && this.Status != -1);
                        this.Btn_retrieve = (this.Status == -1);
                        StringBuilder stringBuilder2 = new StringBuilder("select fitemid,fitemcode,fitemname,ftransfer_batch from vw_ExecKB_Detail_List ");
                        stringBuilder2.Append("  where fbillno='" + this.BoardNo + "'");
                        this.DtItems =(DataTable) DataHelper.ExecuteSql(stringBuilder2.ToString(),DataHelper.CreateSqlConn("shjxmes"),DataHelper.RetrunType.dataTable);
                        if (this.DtItems.Rows.Count > 0)
                        {                          
                            string text = "";
                            for (int i = 0; i <= this.DtItems.Rows.Count - 1; i++)
                            {
                                if (i < this.DtItems.Rows.Count - 1)
                                {
                                    text = text + "'" + this.DtItems.Rows[i]["fitemcode"].ToString() + "',";
                                }
                                else
                                {
                                    text = text + "'" + this.DtItems.Rows[i]["fitemcode"].ToString() + "'";
                                }
                            }
                            try
                            {
                                this.Islast = false;
                                DataTable series = this.GetSeries(text);
                                if (series.Rows.Count < 1)
                                {
                                    
                                    this.DtItems.Clear();
                                    //this.tb_Board.Enabled = true;
                                    //this.tb_Board.Focus();
                                    //this.tb_Board.Text = null;
                                    Err =  "错误提示：[" + text + "]未绑定产线物料";
                                }
                                else
                                {
                                    this.LineID = int.Parse(series.Rows[0]["fconfigid"].ToString());
                                    this.Dtprocess = this.GetProcess(this.LineID);
                                    if (this.Dtprocess.Rows.Count > 0)
                                    {
                                        StringBuilder stringBuilder3 = new StringBuilder("select a.fstatus,a.fstatuName,c.fName,a.finterid,b.fpersons,b.fPsnItemInfo,a.fbillno,b.fProTecID ");
                                        stringBuilder3.Append(" from lkm_exec_kanban_entry a ");
                                        stringBuilder3.Append(" left join LKM_MCCPTEntry b on a.fStatus=b.fInterID ");
                                        stringBuilder3.Append(" left join lkm_CommonBill c on b.fProTecID=c.fInterID ");
                                        stringBuilder3.Append(" where a.fstatus<>2 and a.fbillno='" + this.BoardNo + "'");
                                        DataTable dataTable2 =(DataTable) DataHelper.ExecuteSql(stringBuilder3.ToString(), DataHelper.CreateSqlConn("shjxmes"), DataHelper.RetrunType.dataTable);
                                        if (dataTable2.Rows.Count > 0)
                                        {
                                            this.User = ((dataTable2.Rows[0]["fpersons"].ToString() == "") ? this.Lb_operName : dataTable2.Rows[0]["fpersons"].ToString());
                                            this.Duty = dataTable2.Rows[0]["fPsnItemInfo"].ToString();
                                            this.Scanner = this.Lb_operName;
                                            if (int.Parse(dataTable2.Rows[0]["fstatus"].ToString()) == 0 || int.Parse(dataTable2.Rows[0]["fstatus"].ToString()) == 1)
                                            {
                                                this.Status = int.Parse(this.Dtprocess.Rows[0]["finterid"].ToString());
                                                this.Statusname = this.Dtprocess.Rows[0]["fName"].ToString();
                                            }
                                            else
                                            {
                                                this.Statusname = dataTable2.Rows[0]["fstatuName"].ToString();
                                                this.Status = int.Parse(dataTable2.Rows[0]["fstatus"].ToString());
                                            }
                                            if (this.Status == int.Parse(this.Dtprocess.Rows[this.Dtprocess.Rows.Count - 1]["finterid"].ToString()))
                                            {
                                                this.Islast = true;
                                            }
                                            for (int i = 0; i < this.Dtprocess.Rows.Count; i++)
                                            {
                                                if (this.Status == int.Parse(this.Dtprocess.Rows[i]["finterid"].ToString()))
                                                {
                                                    this.Cur = i;
                                                }
                                            }
                                            if (this.Islast)
                                            {
                                                this.Next_status = -1;
                                                this.Next_statusname = "待回收";
                                            }
                                            else if (this.Status == -1)
                                            {
                                                this.Next_status = 1;
                                                this.Next_statusname = "可用";
                                            }
                                            else
                                            {
                                                this.Next_status = int.Parse(this.Dtprocess.Rows[this.Cur + 1]["finterid"].ToString());
                                                this.Next_statusname = this.Dtprocess.Rows[this.Cur + 1]["fname"].ToString();
                                            }
                                            this.Tb_cPro = this.Statusname;
                                            this.Tb_nPro = this.Next_statusname;
                                            this.Lb_operName = this.Login_user;
                                        }
                                    }
                                   
                                }
                               
                            }
                            catch (Exception ex)
                            {
                                
                                this.Tb_Board = null;
                                //this.tb_Board.Enabled = true;
                                //this.tb_Board.Focus();
                                Err= "错误提示:看板扫描错误!" + ex.Message;
                            }
                        }
                    }
                    else
                    {
                        string text2 = "错误提示:看板[" + this.BoardNo + "]不可用，请重新采集";
                        
                        //this.tb_Board.Enabled = true;
                        //this.tb_Board.Focus();
                        this.Tb_Board = null;
                       
                    }
                   
                }
               
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string StartScan()
        {
            if (this.tb_Board.IndexOf("-") > -1)
            {
                if (this.HandleData(-1) == -1)
                {
                    string text = "【开工】提交失败，请重新采集看板";
                    Err = text;
                }
                //this.tb_Board.Enabled = true;
                //this.tb_Board.Focus();
                this.tb_Board = "";
                this.tb_cPro = "";
                this.tb_nPro = "";
                this._dtItems.Clear();
                this._dtprocess.Clear();
                //this.btn_start.Enabled = false;
                
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public string SubmitScanData()
        {
            if (this.tb_Board.IndexOf("-") > -1)
            {
                for (int i = 0; i <= this._dtItems.Rows.Count - 1; i++)
                {
                    int num = int.Parse(this._dtItems.Rows[i]["ftransfer_batch"].ToString());
                    string sql = string.Concat(new string[]
                    {
                        "select isnull(fcount,0) as fcount from lkm_blank_inventory where fitemid=",
                        this._dtItems.Rows[i]["fitemid"].ToString(),
                        " and flineid = ",
                        this._lineID.ToString(),
                        " and fproid = (select fProTecID from LKM_MCCPTEntry where finterid = ",
                        this._status.ToString(),
                        ")"
                    });
                    int num2 = int.Parse(DataHelper.ExecuteSql(sql, DataHelper.CreateSqlConn("shjxmes"), DataHelper.RetrunType.integer).ToString());
                    if (num2 - num < 0)
                    {
                        string text = "当前物料[" + this._dtItems.Rows[i]["fitemname"].ToString() + "]库存不足，不允许提交";
                        Err = text;
                        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
                    }
                }
                if (this.HandleData(0) == -1)
                {
                    string text = "提交失败，请重新采集看板";
                    Err = text;
                }
                //this.tb_Board.Enabled = true;
                //this.tb_Board.Focus();
                this.tb_Board = null;
                this.tb_cPro = null;
                this.tb_nPro= null;
                this._dtItems.Clear();
                this._dtprocess.Clear();
               
                //this.btn_start.Enabled = false;
                //this.btn_submit.Enabled = false;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
