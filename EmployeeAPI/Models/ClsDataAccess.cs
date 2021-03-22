using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace DataAccess
{
    public class ClsDataAccess
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        public DataTable GetTable(string Qry)   //  Return DataTable for specified Query
        {
            DataTable dDataTable = new DataTable("FillTable");
            SqlDataAdapter dAdp = new SqlDataAdapter("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ;" + Qry + ";  SET TRANSACTION ISOLATION LEVEL REPEATABLE READ ;", ConnectionString);
            try { dAdp.Fill(dDataTable); }
            catch { }
            finally { dAdp.Dispose(); }
            return dDataTable;
        }



        public DataView GetView(string Qry)     //  Return DataView for specified Query
        {
            DataTable dDataTable = new DataTable();
            SqlDataAdapter dAdp = new SqlDataAdapter(Qry, ConnectionString);
            try { dAdp.Fill(dDataTable); }
            catch { }
            finally { dAdp.Dispose(); }
            return dDataTable.DefaultView;
        }

        public String GetSingleValue(string Qry)        // Return single value for the query specified
        {
            object objVal = null;
            SqlConnection tmpCn = new SqlConnection(ConnectionString);
            try
            {
                tmpCn.Open();
                SqlCommand cmd = new SqlCommand(Qry, tmpCn);
                objVal = cmd.ExecuteScalar();
                cmd.Dispose();
            }
            catch { }
            finally { tmpCn.Close(); tmpCn.Dispose(); }
            return (objVal == null) ? "" : objVal.ToString();

        }

        public String GetSingleValue(string Qry, SqlConnection Connection, SqlTransaction Transaction)     // Return single value for the query specified with transaction (If we need any value at the time of any transaction, we should read the value from table by specify the same transaction)
        {
            object objVal = null;
            SqlCommand cmd = new SqlCommand(Qry, Connection, Transaction);
            try
            {
                objVal = cmd.ExecuteScalar();
            }
            catch { }
            finally { cmd.Dispose(); }
            return objVal.ToString();
        }


        public DataRow GetRow(string Qry)
        {
            SqlDataAdapter dAdp = new SqlDataAdapter(Qry, ConnectionString);
            DataTable dTbl = new DataTable();
            try { dAdp.Fill(dTbl); }
            catch { return null; }
            finally { dAdp.Dispose(); }
            if (dTbl.Rows.Count > 0) return dTbl.Rows[0];
            else return null;
        }

        public Boolean SaveData(DataTable DataTbl)      // Save the data to database. Here DataTable's name should be same as the BackEnd Table name
        {
            //if (!HasChanges(DataTbl)) return true;
            SqlDataAdapter dAdp = new SqlDataAdapter("Select * From " + DataTbl.TableName + " Where 1 = 2 ", ConnectionString);
            SqlCommandBuilder cBld = new SqlCommandBuilder(dAdp);
            int iSave = 0;
            try { iSave = dAdp.Update(DataTbl); DataTbl.AcceptChanges(); }
            catch { }
            finally { dAdp.Dispose(); cBld.Dispose(); }
            return (iSave != 0);
        }

        public Boolean SaveData(DataTable DataTbl, string TableName)        // Save the data to database from datatable to specified Table name. 
        {
            //if (!HasChanges(DataTbl)) return true;
            SqlDataAdapter dAdp = new SqlDataAdapter("Select * From " + (string)TableName + " Where 1 = 2 ", ConnectionString);
            SqlCommandBuilder cBld = new SqlCommandBuilder(dAdp);
            int iSave = 0;
            try { iSave = dAdp.Update(DataTbl); DataTbl.AcceptChanges(); }
            catch (Exception ex) { }
            finally { dAdp.Dispose(); cBld.Dispose(); }
            return (iSave != 0);
        }

        public Boolean SaveData(DataTable DataTbl, string TableName, SqlConnection Cn, SqlTransaction Tran)     // Save the data to database with Transaction. 
        {
            //if (!HasChanges(DataTbl)) return true;
            SqlDataAdapter dAdp = new SqlDataAdapter("Select * From " + (string)TableName + " Where 1 = 2", Cn);
            SqlCommandBuilder cBld = new SqlCommandBuilder(dAdp);
            dAdp.SelectCommand.Transaction = Tran;
            int iSave = 0;
            try { iSave = dAdp.Update(DataTbl); DataTbl.AcceptChanges(); }
            catch (Exception ex) { }
            finally { dAdp.Dispose(); cBld.Dispose(); }
            return (iSave != 0);
        }

        public Boolean ExecuteQry(string Qry)
        {
            int iSave = 0;
            SqlConnection tmpCn = new SqlConnection(ConnectionString);
            try
            {
                tmpCn.Open();
                SqlCommand cmd = new SqlCommand(Qry, tmpCn);
                iSave = cmd.ExecuteNonQuery();
                cmd.Dispose();

            }
            catch (Exception ex) { return false; }
            finally { tmpCn.Close(); tmpCn.Dispose(); }
            return (iSave != 0);
        }
        public string DataTableToJSON(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
      

    }
}
