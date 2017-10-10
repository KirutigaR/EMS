using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EMS.Utility
{
    public class GetSheet
    {
        public static String[] GetSheetName(OleDbConnection objConn)
        {
            DataTable dt = null;

            try
            {
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    return null;
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                return excelSheets;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException());
                return null;
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                }
            }
        }
    }
}