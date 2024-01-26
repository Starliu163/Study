using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReadAndSaveFile
{
    public class ExcelHelper
    {
        public DataTable ReadExcelToDataTablet(string path)
        {
            //连接字符串
            /* 备注：
            	添加 IMEX=1 表示将所有列当做字符串读取，实际应该不是这样，
            	系统默认会查看前8行如果有字符串，则该列会识别为字符串列。
            	如果前8行都是数字，则还是会识别为数字列，日期也一样；
            	如果你觉得8行不够或者太多了，则只能修改注册表HKEY_LOCAL_MACHINE/Software/Microsoft/Jet/4.0/Engines/Excel/TypeGuessRows，
            	如果此值为0，则会根据所有行来判断使用什么类型，通常不建议这麽做，除非你的数据量确实比较少
            */
            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;IMEX=1';";

            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                //开启连接
                conn.Open();
                System.Data.DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });//存放所有的sheet
                DataSet set = new DataSet();


                for (int i = 0; i < sheetsName.Rows.Count; i++)
                {
                    string sheetName = sheetsName.Rows[i][2].ToString();
                    string sql = string.Format("SELECT * FROM [{0}]", sheetName);
                    OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);

                    ada.Fill(set);
                    set.Tables[i].TableName = sheetName;
                }

                //返回第一张表
                return set.Tables[0];

            }
        }
    }
}
