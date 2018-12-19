﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace PortfolioEngineCore
{
    public class dbaRPAdmin
    {
        public static StatusEnum SelectLookupTables(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "SELECT LOOKUP_UID, LOOKUP_NAME FROM EPGP_LOOKUP_TABLES ORDER BY LOOKUP_NAME";
            return dba.SelectData(cmdText, (StatusEnum)99999, out dt);
        }

        public static StatusEnum UpdateRPAdminInfo(DBAccess dba, int deptuid, int roleuid, int caluid, int mode, int opmode, int hoursuid)
        {
            const string cmdText = "UPDATE EPG_ADMIN "
                    + "SET ADM_RPE_DEPT_CODE=@ADM_RPE_DEPT_CODE,ADM_ROLE_CODE=@ADM_ROLE_CODE,ADM_PORT_COMMITMENTS_CB_ID=@ADM_PORT_COMMITMENTS_CB_ID,"
                    + "ADM_PORT_COMMITMENTS_MODE=@ADM_PORT_COMMITMENTS_MODE,ADM_PORT_COMMITMENTS_OPMODE=@ADM_PORT_COMMITMENTS_OPMODE,ADM_PROJ_RES_HOURS_CFID=@ADM_PROJ_RES_HOURS_CFID";

            string s = cmdText;
            s = s.Replace("@ADM_RPE_DEPT_CODE", deptuid.ToString("0"));
            s = s.Replace("@ADM_ROLE_CODE", roleuid.ToString("0"));
            s = s.Replace("@ADM_PORT_COMMITMENTS_CB_ID", caluid.ToString("0"));
            s = s.Replace("@ADM_PORT_COMMITMENTS_MODE", mode.ToString("0"));
            s = s.Replace("@ADM_PORT_COMMITMENTS_OPMODE", opmode.ToString("0"));
            s = s.Replace("@ADM_PROJ_RES_HOURS_CFID", hoursuid.ToString("0"));
            
            int lRowsAffected;
            return dba.ExecuteNonQuery(s, (StatusEnum)99999, out lRowsAffected);
        }

        public static int GetRPCalendar (DBAccess dba)
        {
            SqlCommand oCommand;
            SqlDataReader reader;
            string cmdText;
            int nCal=0;

            cmdText = "SELECT ADM_PORT_COMMITMENTS_CB_ID FROM EPG_ADMIN";
            oCommand = new SqlCommand(cmdText, dba.Connection);
            reader = oCommand.ExecuteReader();
            while (reader.Read())
            {
                nCal = DBAccess.ReadIntValue(reader["ADM_PORT_COMMITMENTS_CB_ID"]);
            }
            reader.Close();
            reader = null;

            return nCal;
        }
    }
    public class dbaCustomFields
    {
        public static StatusEnum SelectCustomFields(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "SELECT FA_FIELD_ID,FA_NAME,FA_FORMAT,FA_TABLE_ID,FA_FIELD_IN_TABLE,FA_DESC,"
                            + " case When (FA_TABLE_ID >100 And FA_TABLE_ID<200) Then 1 When (FA_TABLE_ID >200 And FA_TABLE_ID<300) Then 2 Else 0 End as Entity"
                            + " FROM EPGC_FIELD_ATTRIBS"
                            + " Where FA_TABLE_ID > 100 and FA_TABLE_ID < 300"
                            + " ORDER BY Entity,FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum)99857, out dt);
        }
        public static StatusEnum SelectPortfolioFormulaCustomFields(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "SELECT FA_FIELD_ID,FA_NAME"
                            + " FROM EPGC_FIELD_ATTRIBS"
                            + " WHERE FA_TABLE_ID > 200 AND FA_TABLE_ID < 300 AND (FA_FORMAT = 3 OR FA_FORMAT = 8 OR FA_FORMAT = 9)"
                            + " ORDER BY FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum)99857, out dt);
        }
        public static StatusEnum SelectCustomField(DBAccess dba, int lFieldId, out DataTable dt)
        {
            if (lFieldId > 0)
            {
                const string cmdText = "SELECT FA_FIELD_ID,FA_NAME,FA_FORMAT,FA_TABLE_ID,FA_FIELD_IN_TABLE,FA_DESC,"
                                + " case When (FA_TABLE_ID >100 And FA_TABLE_ID<200) Then 1 When (FA_TABLE_ID >200 And FA_TABLE_ID<300) Then 2 Else 0 End as Entity"
                                + " FROM EPGC_FIELD_ATTRIBS"
                                + " Where FA_TABLE_ID > 100 and FA_TABLE_ID < 300 AND FA_FIELD_ID=@p1"
                                + " ORDER BY Entity,FA_NAME";
                return dba.SelectDataById(cmdText, lFieldId, (StatusEnum)99857, out dt);
            }
            else
            {
                const string cmdText = "SELECT FA_FIELD_ID=0,FA_NAME='New Custom Field',FA_FORMAT=9,FA_TABLE_ID=0,FA_FIELD_IN_TABLE=0,FA_DESC='',Entity=2";
                return dba.SelectData(cmdText, (StatusEnum)99857, out dt);
            }
        }
        public static StatusEnum UpdateCustomFieldInfo(DBAccess dba, int nEntity, int nFieldType, ref int nFieldId, string sFieldName, string sFieldDesc, out string sReply)
        {
            sReply = string.Empty;
            try
            {
                if (!ValidateFieldName(dba, nFieldId, ref sFieldName, ref sReply))
                {
                    return StatusEnum.rsRequestCannotBeCompleted;
                }

                if (nFieldId == 0)
                {
                    var status = AddNewField(dba, nEntity, nFieldType, ref nFieldId, sFieldName, sFieldDesc, ref sReply);
                    if (status.HasValue)
                    {
                        return status.Value;
                    }
                }
                else
                {
                    UpdateFieldAttributes(dba, nFieldId, sFieldName, sFieldDesc);
                }
                return StatusEnum.rsSuccess;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception Supressed {0}", ex);
                sReply = SqlDb.FormatAdminError("exception", "Customfields.UpdateCustomFieldInfo", ex.Message);
                return StatusEnum.rsRequestCannotBeCompleted;
            }
        }

        private static StatusEnum? AddNewField(
            DBAccess dba,
            int nEntity,
            int nFieldType,
            ref int nFieldId,
            string sFieldName,
            string sFieldDesc,
            ref string sReply)
        {
            var tableId = EPKClass01.GetTableID(nEntity, nFieldType);
            string table;
            string field;
            StatusEnum status;

            if (EPKClass01.GetTableAndField(tableId, 1, out table, out field))
            {
                int maxField;
                SelectFirstTable(dba, table, field, out status, out maxField);

                var usedFields = SelectFieldAttributes(dba, tableId);

                int nUseField;
                if (!ValidateUSedFields(maxField, usedFields, ref sReply, out nUseField))
                {
                    return status;
                }

                InsertFieldAttributes(dba, nFieldType, sFieldName, sFieldDesc, tableId, nUseField);
                dba.GetLastIdentityValue(out nFieldId);
            }

            return null;
        }

        private static bool ValidateFieldName(DBAccess dbAccess, int fieldId, ref string fieldName, ref string reply)
        {
            fieldName = fieldName.Trim();
            if (fieldName.Length == 0)
            {
                reply = SqlDb.FormatAdminError("error", "Customfields.UpdateCustomFieldInfo", "Please enter a Field Name");
                {
                    return false;
                }
            }
            const string Command = "SELECT FA_FIELD_ID From EPGC_FIELD_ATTRIBS WHERE FA_NAME = @p1";
            DataTable dataTable;
            dbAccess.SelectDataByName(Command, fieldName, (StatusEnum)99917, out dataTable);
            if (dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                var existingId = SqlDb.ReadIntValue(row["FA_FIELD_ID"]);
                if (existingId != fieldId)
                {
                    reply = SqlDb.FormatAdminError(
                        "error",
                        "Customfields.UpdateCustomFieldInfo",
                        string.Format("Can't save field.\nA field with name '{0}' already exists", fieldName));
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void SelectFirstTable(DBAccess dba, string table, string field, out StatusEnum status, out int maxField)
        {
            var command = string.Format("SELECT TOP 1 * FROM {0}", table);
            DataTable dataTable;
            status = dba.SelectData(command, (StatusEnum)99885, out dataTable);

            // find the field with the highest number suffix - eg PC_050 - user can add to these tables
            maxField = 0;
            var prefix = field.Substring(0, 3);
            var columns = dataTable.Columns;
            foreach (DataColumn column in columns)
            {
                var columnName = column.ColumnName;
                var thisPrefix = columnName.Substring(0, 3);
                if (thisPrefix == prefix)
                {
                    var fieldNumber = int.Parse(columnName.Substring(3));
                    if (fieldNumber > maxField)
                    {
                        maxField = fieldNumber;
                    }
                }
            }
        }

        private static List<int> SelectFieldAttributes(DBAccess dba, int tableId)
        {
            var usedFields = new List<int>();
            var command = string.Format("SELECT FA_FIELD_IN_TABLE FROM EPGC_FIELD_ATTRIBS Where FA_TABLE_ID={0}", tableId);
            using (var sqlCommand = new SqlCommand(command, dba.Connection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nFieldNumber = SqlDb.ReadIntValue(reader["FA_FIELD_IN_TABLE"]);
                        usedFields.Add(nFieldNumber);
                    }
                    reader.Close();
                }
            }
            return usedFields;
        }

        private static bool ValidateUSedFields(int maxField, List<int> usedFields, ref string reply, out int nUseField)
        {
            for (nUseField = 1; nUseField < maxField + 2; nUseField++)
            {
                if (!usedFields.Contains(nUseField))
                {
                    break;
                }
            }
            if (nUseField > maxField)
            {
                reply = SqlDb.FormatAdminError("error", "Customfields.UpdateCustomFieldInfo", "Can't save Field, all Fields of this type are already used");
                return false;
            }
            return true;
        }

        private static void InsertFieldAttributes(DBAccess dbAccess, int fieldType, string fieldName, string fieldDesc, int tableId, int useField)
        {
            const string Command = "INSERT Into EPGC_FIELD_ATTRIBS "
                + " (FA_NAME,FA_DESC,FA_FORMAT,FA_TABLE_ID,FA_FIELD_IN_TABLE)"
                + " Values(@pNAME,@pDESC,@pFORMAT,@pTABLE,@pFIELD)";
            using (var sqlCommand = new SqlCommand(Command, dbAccess.Connection))
            {
                sqlCommand.Parameters.AddWithValue("@pNAME", fieldName);
                sqlCommand.Parameters.AddWithValue("@pDESC", fieldDesc);
                sqlCommand.Parameters.AddWithValue("@pFORMAT", fieldType);
                sqlCommand.Parameters.AddWithValue("@pTABLE", tableId);
                sqlCommand.Parameters.AddWithValue("@pFIELD", useField);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void UpdateFieldAttributes(DBAccess dbAccess, int fieldId, string fieldName, string fieldDesc)
        {
            const string Command = "UPDATE EPGC_FIELD_ATTRIBS " + " SET FA_NAME=@pNAME,FA_DESC=@pDESC" + " WHERE FA_FIELD_ID = @pFIELDID";
            using (var sqlCommand = new SqlCommand(Command, dbAccess.Connection, dbAccess.Transaction))
            {
                sqlCommand.Parameters.AddWithValue("@pNAME", fieldName);
                sqlCommand.Parameters.AddWithValue("@pDESC", fieldDesc);
                sqlCommand.Parameters.AddWithValue("@pFIELDID", fieldId);
                var rowsAffected = sqlCommand.ExecuteNonQuery();
            }
        }

        public static StatusEnum DeleteCustomField(DBAccess dba, int nFieldId, out string sReply)
        {
            string cmdText;
            SqlCommand oCommand;
            sReply = "";
            try
            {
                // check if field can be deleted - not the best place for it but should be fine
                string sdeletemessage;
                if (CanDeleteCustomField(dba, nFieldId, out sdeletemessage) != StatusEnum.rsSuccess) goto Exit_Function;
                if (sdeletemessage.Length > 0)
                {
                    sReply = DBAccess.FormatAdminError("error", "Customfields.DeleteCustomField", "This Custom Field cannot be deleted, it is currently used as follows:" + "\n" + "\n" + sdeletemessage);
                    return StatusEnum.rsRequestCannotBeCompleted;
                }

                // get info for field to be deleted
                int nTable;
                int nField;
                {
                    cmdText = "SELECT * FROM EPGC_FIELD_ATTRIBS Where FA_FIELD_ID = @p1";
                    DataTable dt;
                    dba.SelectDataById(cmdText, nFieldId, (StatusEnum)99999, out dt);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        nTable = DBAccess.ReadIntValue(row["FA_TABLE_ID"]);
                        nField = DBAccess.ReadIntValue(row["FA_FIELD_IN_TABLE"]);
                    }
                    else
                    {
                        sReply = DBAccess.FormatAdminError("error", "Customfields.DeleteCustomField", "Can't delete field, field not found");
                        return StatusEnum.rsRequestCannotBeCompleted;
                    }
                }

                //   to be tidy need to clear field data

                string sTable;
                string sField;
                if (EPKClass01.GetTableAndField(nTable, nField, out sTable, out sField))
                {
                    if (nTable >= (int)CustomFieldTable.PortfolioINT && nTable <= (int)CustomFieldTable.PortfolioDATE)
                    {
                        cmdText = "Update " + sTable + " Set " + sField + "=NULL";
                        oCommand = new SqlCommand(cmdText, dba.Connection);
                        int lRowsAffected = oCommand.ExecuteNonQuery();
                    }
                    else if (nTable >= (int)CustomFieldTable.ResourceINT && nTable <= (int)CustomFieldTable.ResourceINT)
                    {
                        cmdText = "Update " + sTable + " Set " + sField + "=NULL";
                        oCommand = new SqlCommand(cmdText, dba.Connection);
                        int lRowsAffected = oCommand.ExecuteNonQuery();
                    }
                    else if (nTable == (int)CustomFieldTable.ResourceMV)
                    {
                        cmdText = "Delete From EPGC_RESOURCE_MV_VALUES Where MVR_FIELD_ID=@pField";
                        oCommand = new SqlCommand(cmdText, dba.Connection);
                        oCommand.Parameters.AddWithValue("@pField", nField);
                        int lRowsAffected = oCommand.ExecuteNonQuery();
                    }
                    // Delete any CALCs or CALC components
                    cmdText = "DELETE FROM EPGP_CALCS Where CL_RESULT=@pField1 Or CL_COMPONENT=@pField2";
                    oCommand = new SqlCommand(cmdText, dba.Connection);
                    oCommand.Parameters.AddWithValue("@pField1", nFieldId);
                    oCommand.Parameters.AddWithValue("@pField2", nFieldId);
                    oCommand.ExecuteNonQuery();

                    // Delete the CF itself
                    cmdText = "DELETE FROM EPGC_FIELD_ATTRIBS Where FA_FIELD_ID=@pField";
                    oCommand = new SqlCommand(cmdText, dba.Connection);
                    oCommand.Parameters.AddWithValue("@pField", nFieldId);
                    oCommand.ExecuteNonQuery();
                }

        Exit_Function:
                return dba.Status;
            }
            catch (Exception ex)
            {
                sReply = DBAccess.FormatAdminError("exception", "Customfields.DeleteCustomField", ex.Message);
                return StatusEnum.rsRequestCannotBeCompleted;
            }
        }

        public static StatusEnum CanDeleteCustomField(DBAccess dba, int nFieldId, out string sReply)
        {
            SqlCommand oCommand;
            SqlDataReader reader;
            sReply = "";
            try
            {
                // check if field can be deleted - not the best place for it but should be fine
                oCommand = new SqlCommand("EPG_SP_ReadUsedCF", dba.Connection);
                oCommand.CommandType = System.Data.CommandType.StoredProcedure;
                oCommand.Parameters.AddWithValue("@FieldID", nFieldId);
                reader = oCommand.ExecuteReader();

                while (reader.Read())
                {
                    sReply += DBAccess.ReadStringValue(reader["UsedMessage"]) + ": ";
                    sReply += DBAccess.ReadStringValue(reader["UsedData"]) + "\n";
                }
                reader.Close();
                return StatusEnum.rsSuccess;
            }
            catch (Exception ex)
            {
                sReply = DBAccess.FormatAdminError("exception", "Customfields.CanDeleteCustomField", ex.Message);
                return StatusEnum.rsRequestCannotBeCompleted;
            }
        }
        public static StatusEnum GetCustomFieldNameFromID(DBAccess dba, int lFieldID, out string sTableName, out string sFieldName)
        {
            StatusEnum eStatus = StatusEnum.rsSuccess;
            sTableName = "";
            sFieldName = "";
            try
            {
                // Bugfix 27JUN08 V5.3 - Don't allow showthrough of table or field if fieldid not found
                SqlCommand oCommand = new SqlCommand("EPG_SP_ReadFieldInfo", dba.Connection, dba.Transaction);
                oCommand.CommandType = System.Data.CommandType.StoredProcedure;
                oCommand.Parameters.Add("FieldID", SqlDbType.Int).Value = lFieldID;
                SqlDataReader reader = oCommand.ExecuteReader();

                if (reader.Read())
                {
                    int lTableID = DBAccess.ReadIntValue(reader["FA_TABLE_ID"]);
                    int lFieldInTableID = DBAccess.ReadIntValue(reader["FA_FIELD_IN_TABLE"]);
                    GetCFFieldName(lTableID, lFieldInTableID, out sTableName, out sFieldName);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                eStatus = dba.HandleException("GetCustomFieldNameFromID", (StatusEnum)9194, ex);
            }
            return eStatus;
        }
        public static StatusEnum SelectRPTotalHoursCustomFieldCandidates(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "SELECT FA_FIELD_ID,FA_NAME,FA_DESC"
                            + " FROM EPGC_FIELD_ATTRIBS"
                            + " Where FA_TABLE_ID = 203 And FA_FORMAT=3"
                            + " ORDER BY FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum)99857, out dt);
        }
        public static StatusEnum SelectRoles(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "SELECT LV_UID, LV_VALUE "
                            + " FROM EPGP_LOOKUP_VALUES"
                            + " WHERE LOOKUP_UID = (SELECT ADM_ROLE_CODE FROM EPG_ADMIN)"
                            + " ORDER BY LV_ID";
            return dba.SelectData(cmdText, (StatusEnum)99857, out dt);
        }
        private static void GetCFFieldName(int lTableID, int lFieldID, out string sTable, out string sField)
        {
            switch ((CustomFieldDBTable)lTableID)
            {
                case CustomFieldDBTable.ResourceINT:
                    sTable = "EPGC_RESOURCE_INT_VALUES";
                    sField = "RI_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ResourceTEXT:
                    sTable = "EPGC_RESOURCE_TEXT_VALUES";
                    sField = "RT_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ResourceDEC:
                    sTable = "EPGC_RESOURCE_DEC_VALUES";
                    sField = "RC_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ResourceNTEXT:
                    sTable = "EPGC_RESOURCE_NTEXT_VALUES";
                    sField = "RN_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ResourceDATE:
                    sTable = "EPGC_RESOURCE_DATE_VALUES";
                    sField = "RD_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ResourceMV:
                    sTable = "EPGC_RESOURCE_MV_VALUES";
                    sField = "MVR_UID";
                    break;
                case CustomFieldDBTable.PortfolioINT:
                    sTable = "EPGP_PROJECT_INT_VALUES";
                    sField = "PI_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.PortfolioTEXT:
                    sTable = "EPGP_PROJECT_TEXT_VALUES";
                    sField = "PT_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.PortfolioDEC:
                    sTable = "EPGP_PROJECT_DEC_VALUES";
                    sField = "PC_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.PortfolioNTEXT:
                    sTable = "EPGP_PROJECT_NTEXT_VALUES";
                    sField = "PN_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.PortfolioDATE:
                    sTable = "EPGP_PROJECT_DATE_VALUES";
                    sField = "PD_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.Program:
                    sTable = "EPGP_PI_PROGS";
                    sField = "PROG_UID";
                    break;
                case CustomFieldDBTable.ProjectINT:
                    sTable = "EPGX_PROJ_INT_VALUES";
                    sField = "XI_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ProjectTEXT:
                    sTable = "EPGX_PROJ_TEXT_VALUES";
                    sField = "XT_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ProjectDEC:
                    sTable = "EPGX_PROJ_DEC_VALUES";
                    sField = "XC_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ProjectNTEXT:
                    sTable = "EPGX_PROJ_NTEXT_VALUES";
                    sField = "XN_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ProjectDATE:
                    sTable = "EPGX_PROJ_DATE_VALUES";
                    sField = "XD_" + lFieldID.ToString("000");
                    break;
                case CustomFieldDBTable.ProgramText:
                    sTable = "EPGP_PI_PROGS";
                    sField = "PROG_PI_TEXT" + lFieldID.ToString("0");
                    break;
                case CustomFieldDBTable.TaskWIINT:
                    sTable = "EPGP_PI_WORKITEMS";
                    sField = "WORKITEM_FLAG" + lFieldID.ToString("0");
                    break;
                case CustomFieldDBTable.TaskWITEXT:
                    sTable = "EPGP_PI_WORKITEMS";
                    sField = "WORKITEM_CTEXT" + lFieldID.ToString("0");
                    break;
                case CustomFieldDBTable.TaskWIDEC:
                    sTable = "EPGP_PI_WORKITEMS";
                    sField = "WORKITEM_NUMBER" + lFieldID.ToString("0");
                    break;
                default:
                    sTable = "Unknown Table";
                    sField = "";
                    break;
            }
        }
        public static StatusEnum SelectCustomFieldFormula(DBAccess dba, int lFieldId, out DataTable dt)
        {
            const string cmdText = "SELECT CL_UID,CL_SEQ,CL_RESULT,CL_COMPONENT,CL_RATIO,CL_OP,CL_PRI,f1.FA_NAME as ResultName,f2.FA_NAME as ComponentName"
                + " FROM EPGP_CALCS c "
                + " Join EPGC_FIELD_ATTRIBS f1 On c.CL_RESULT=f1.FA_FIELD_ID"
                + " Left Join EPGC_FIELD_ATTRIBS f2 On c.CL_COMPONENT=f2.FA_FIELD_ID"
                + " Where CL_OBJECT=1 AND CL_RESULT=@p1"
                + " Order By CL_UID,CL_SEQ";
            return dba.SelectDataById(cmdText, lFieldId, (StatusEnum)99857, out dt);
        }
        public static StatusEnum DeleteCustomFieldFormula(DBAccess dba, int lFieldId, out int lRowsAffected)
        {
            const string cmdText = "DELETE EPGP_CALCS WHERE CL_OBJECT=1 AND CL_RESULT=@p1";
            return dba.DeleteDataById(cmdText, lFieldId, (StatusEnum)99857, out lRowsAffected);
        }

        private class ItemRow
        {
            public bool hasOp = false;
            public bool hasField = false;
            public bool hasConstant = false;
            public decimal ratio = (decimal)1;
            public int fieldId = 0;
            public int opId = 0;
            public string value = "";
        }

        public static string ValidateAndSaveCustomFieldFormula(DBAccess dba, int nFieldId, ref string sFormula, bool bSave = false)
        {
            var error = string.Empty;
            sFormula = sFormula.Trim();
            if (sFormula == string.Empty)
            {
                if (bSave)
                {
                    int rowsAffected;
                    DeleteCustomFieldFormula(dba, nFieldId, out rowsAffected);
                }
                return error;
            }

            DataTable dataTable;
            SelectPortfolioFormulaCustomFields(dba, out dataTable);

            var formulas = Regex.Split(sFormula, @"([-+/*])");

            var itemRows = new List<ItemRow>();
            if (!ValidateAndTrimFormula(nFieldId, formulas, itemRows, dataTable, ref error))
            {
                return error;
            }
            ProcessPreValidatedElements(ref itemRows);
            SaveValidFormula(dba, nFieldId, bSave, itemRows);
            return error;
        }

        private static bool ValidateAndTrimFormula(
            int nFieldId,
            string[] formulas,
            List<ItemRow> itemRows,
            DataTable dataTable,
            ref string error)
        {
            var opCount = 0;
            var fieldCount = 0;
            var decCount = 0;
            for (var i = 0; i < formulas.Length; i++)
            {
                formulas[i] = formulas[i].Trim();
                if (formulas[i] != string.Empty)
                {
                    var itemRow = new ItemRow
                    {
                        value = formulas[i]
                    };
                    switch (formulas[i])
                    {
                        case "*":
                        case "/":
                        case "+":
                        case "-":
                            if (!ProcessMathOpCase(formulas, itemRows, itemRow, i, decCount, fieldCount, ref opCount, ref error))
                            {
                                return false;
                            }
                            break;
                        default:
                            if (!ProcessFieldsAndConstants(nFieldId, formulas, itemRows, dataTable, i, itemRow, opCount, ref fieldCount, ref error, ref decCount))
                            {
                                return false;
                            }
                            break;
                    }
                }
            }

            if (opCount >= decCount + fieldCount)
            {
                error = SqlDb.FormatAdminError(
                    "error",
                    "dbaCustomFields.ValidateAndSaveCustomFieldFormula",
                    "Formula cannot start or finish with an operator");
                return false;
            }
            return true;
        }

        private static bool ProcessFieldsAndConstants(
            int nFieldId,
            string[] formulas,
            List<ItemRow> itemRows,
            DataTable dataTable,
            int index,
            ItemRow itemRow,
            int opCount,
            ref int fieldCount,
            ref string error,
            ref int decCount)
        {
            if (Regex.IsMatch(formulas[index], @"\A[0-9]*(\.[0-9]+)\z|\A[0-9]+\z") != true)
            {
                if (!ProcessField(nFieldId, formulas, itemRows, dataTable, index, itemRow, ref fieldCount, ref error))
                {
                    return false;
                }
            }
            else
            {
                if (!ProcessConstant(formulas, itemRows, index, itemRow, ref error, ref decCount))
                {
                    return false;
                }
            }
            if (opCount + 1 < decCount + fieldCount)
            {
                error = SqlDb.FormatAdminError("error", "dbaCustomFields.ValidateAndSaveCustomFieldFormula", "Too few operators");
                return false;
            }
            return true;
        }

        private static bool ProcessConstant(string[] formulas, List<ItemRow> itemRows, int i, ItemRow itemRow, ref string error, ref int decCount)
        {
            decimal dec;
            if (decimal.TryParse(formulas[i], out dec))
            {
                itemRow.hasConstant = true;
                itemRow.ratio = dec;
                itemRows.Add(itemRow);
                decCount++;
            }
            else
            {
                error = SqlDb.FormatAdminError("error", "dbaCustomFields.ValidateAndSaveCustomFieldFormula", "invalid value: " + itemRow.value);
                return false;
            }
            return true;
        }

        private static bool ProcessField(
            int nFieldId,
            string[] formulas,
            List<ItemRow> itemRows,
            DataTable dataTable,
            int i,
            ItemRow itemRow,
            ref int fieldCount,
            ref string error)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                if (SqlDb.ReadStringValue(row["FA_NAME"]) == formulas[i])
                {
                    var nFoundFieldId = SqlDb.ReadIntValue(row["FA_FIELD_ID"]);
                    if (nFoundFieldId == nFieldId)
                    {
                        error = SqlDb.FormatAdminError(
                            "error",
                            "dbaCustomFields.ValidateAndSaveCustomFieldFormula",
                            "Cannot use custom field in its own formula");
                        return false;
                    }
                    itemRow.hasField = true;
                    itemRow.fieldId = nFoundFieldId;
                    itemRows.Add(itemRow);
                    fieldCount++;
                    break;
                }
            }
            if (itemRow.hasField == false)
            {
                error = SqlDb.FormatAdminError("error", "dbaCustomFields.ValidateAndSaveCustomFieldFormula", "Unknown custom field name: " + itemRow.value);
                return false;
            }
            return true;
        }

        private static bool ProcessMathOpCase(
            string[] formulas,
            List<ItemRow> itemRows,
            ItemRow itemRow,
            int i,
            int decCount,
            int fieldCount,
            ref int opCount,
            ref string error)
        {
            itemRow.hasOp = true;
            switch (formulas[i])
            {
                case "+":
                    itemRow.opId = 0;
                    break;
                case "-":
                    itemRow.opId = 1;
                    break;
                case "*":
                    itemRow.opId = 2;
                    break;
                case "/":
                    itemRow.opId = 3;
                    break;
                default:
                    break;
            }
            itemRows.Add(itemRow);
            opCount++;
            if (opCount > decCount + fieldCount)
            {
                error = SqlDb.FormatAdminError("error", "dbaCustomFields.ValidateAndSaveCustomFieldFormula", "Too many operators");
                return false;
            }
            return true;
        }

        private static void ProcessPreValidatedElements(ref List<ItemRow> itemRows)
        {
            var arrItemRows = itemRows.ToArray();
            itemRows = new List<ItemRow>();
            
            var i = 0;
            var itemRow = new ItemRow();
            while (i < arrItemRows.Length)
            {
                var inc = 1;
                if (arrItemRows[i].hasOp)
                {
                    itemRow.opId = arrItemRows[i].opId;
                }
                else
                {
                    var rowComplete = false;
                    if (i + 2 < arrItemRows.Length)
                    {
                        if (arrItemRows[i].hasField && arrItemRows[i + 1].value == "*" && arrItemRows[i + 2].hasConstant)
                        {
                            itemRow.fieldId = arrItemRows[i].fieldId;
                            itemRow.ratio = arrItemRows[i + 2].ratio;
                            inc = 3;
                            rowComplete = true;
                        }
                        else if (arrItemRows[i].hasConstant && arrItemRows[i + 1].value == "*" && arrItemRows[i + 2].hasField)
                        {
                            itemRow.fieldId = arrItemRows[i + 2].fieldId;
                            itemRow.ratio = arrItemRows[i].ratio;
                            inc = 3;
                            rowComplete = true;
                        }
                    }
                    if (rowComplete == false)
                    {
                        if (arrItemRows[i].hasField)
                        {
                            itemRow.fieldId = arrItemRows[i].fieldId;
                            itemRow.ratio = 1;
                            rowComplete = true;
                        }
                        else if (arrItemRows[i].hasConstant)
                        {
                            itemRow.fieldId = 0;
                            itemRow.ratio = arrItemRows[i].ratio;
                            rowComplete = true;
                        }
                    }
                    if (rowComplete)
                    {
                        itemRows.Add(itemRow);
                        itemRow = new ItemRow();
                    }
                }
                i += inc;
            }
        }

        private static void SaveValidFormula(DBAccess dba, int nFieldId, bool bSave, List<ItemRow> itemRows)
        {
            if (bSave)
            {
                int rowsAffected;
                DeleteCustomFieldFormula(dba, nFieldId, out rowsAffected);
                var newClUId = 0;
                const string cmdText = "SELECT MAX(CL_UID) As maxUID FROM EPGP_CALCS";
                DataTable dataTable;
                dba.SelectData(cmdText, (StatusEnum)99999, out dataTable);

                if (dataTable.Rows.Count == 1)
                {
                    var row = dataTable.Rows[0];
                    newClUId = SqlDb.ReadIntValue(row["maxUID"]) + 1;
                }

                const string Command = "INSERT INTO  EPGP_CALCS (CL_OBJECT, CL_PRI, CL_OP, CL_UID, CL_SEQ, CL_RESULT, CL_COMPONENT, CL_RATIO) "
                    + "VALUES(1, 0, @CL_OP, @CL_UID, @CL_SEQ, @CL_RESULT, @CL_COMPONENT, @CL_RATIO)";

                using (var sqlCommand = new SqlCommand(Command, dba.Connection))
                {
                    var clOp = sqlCommand.Parameters.Add("@CL_OP", SqlDbType.Int);
                    var clUId = sqlCommand.Parameters.Add("@CL_UID", SqlDbType.Int);
                    var clSeq = sqlCommand.Parameters.Add("@CL_SEQ", SqlDbType.Int);
                    var clResult = sqlCommand.Parameters.Add("@CL_RESULT", SqlDbType.Int);
                    var clComponent = sqlCommand.Parameters.Add("@CL_COMPONENT", SqlDbType.Int);
                    var clRatio = sqlCommand.Parameters.Add("@CL_RATIO", SqlDbType.Decimal);

                    clRatio.Precision = 25;
                    clRatio.Scale = 6;

                    var seq = 0;
                    foreach (var itemRow in itemRows)
                    {
                        clOp.Value = itemRow.opId;
                        clUId.Value = newClUId;
                        clSeq.Value = ++seq;
                        clResult.Value = nFieldId;
                        clComponent.Value = itemRow.fieldId;
                        clRatio.Value = itemRow.ratio;

                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    public class dbaPrioritz
    {
        public static StatusEnum SelectFields(DBAccess dba, out DataTable dt)
        {
            const string cmdText =
                "SELECT * FROM EPGC_FIELD_ATTRIBS Where ((FA_TABLE_ID=203 and FA_FORMAT=3) or (FA_TABLE_ID=202 and FA_FORMAT=9)) ORDER BY FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum) 99858, out dt);
        }

        public static StatusEnum SelectComponents(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "Select * From EPGP_PI_PRI_COMPONENTS Order BY CC_SEQ";
            return dba.SelectData(cmdText, (StatusEnum) 99857, out dt);
        }

        public static StatusEnum SelectFormulas(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "Select Distinct c.CW_RESULT,f.FA_NAME From EPGP_PI_PRI_WEIGHTS c" +
                             " Inner Join EPGC_FIELD_ATTRIBS f On f.FA_FIELD_ID=c.CW_RESULT Order BY FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum) 99856, out dt);
        }

        public static StatusEnum SelectWeights(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "Select * From EPGP_PI_PRI_WEIGHTS";
            return dba.SelectData(cmdText, (StatusEnum) 99855, out dt);
        }

        public static StatusEnum SelectComponentFields(DBAccess dba, out DataTable dt)
        {
            const string cmdText =
                "SELECT * FROM EPGC_FIELD_ATTRIBS Where ((FA_TABLE_ID=203 and FA_FORMAT=3) or (FA_TABLE_ID=202 and FA_FORMAT=9))" +
                " And FA_FIELD_ID Not In (Select Distinct CL_RESULT From EPGP_CALCS Where CL_PRI=1)" +
                " ORDER BY FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum) 99854, out dt);
        }

        public static StatusEnum SelectFormulaFields(DBAccess dba, out DataTable dt)
        {
            const string cmdText = "SELECT * FROM EPGC_FIELD_ATTRIBS Where (FA_TABLE_ID=203 and FA_FORMAT=3)" +
                             " And FA_FIELD_ID Not In (Select CC_COMPONENT From EPGP_PI_PRI_COMPONENTS)" +
                             " ORDER BY FA_NAME";
            return dba.SelectData(cmdText, (StatusEnum) 99853, out dt);
        }

        public static StatusEnum UpdateComponents(DBAccess dba, DataTable dtComponents, out int lRowsAffected)
        {
            StatusEnum eStatus = StatusEnum.rsSuccess;
            lRowsAffected = 0;

            if (eStatus == StatusEnum.rsSuccess)
            {
                lRowsAffected = 0;
                {
                    dba.BeginTransaction();
                    try
                    {
                        SqlCommand oCommand;
                        string cmdText;
                        cmdText = "DELETE FROM EPGP_PI_PRI_COMPONENTS";
                        oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                        lRowsAffected = oCommand.ExecuteNonQuery();

                        if (dtComponents.Rows.Count > 0)
                        {
                            cmdText = "INSERT INTO EPGP_PI_PRI_COMPONENTS (CC_COMPONENT,CC_SEQ) VALUES(@CC_COMPONENT,@CC_SEQ)";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            SqlParameter pComponent = oCommand.Parameters.Add("@CC_COMPONENT", SqlDbType.Int);
                            SqlParameter pSEQ = oCommand.Parameters.Add("@CC_SEQ", SqlDbType.Int);

                            int lCTRowsAffected = 0;
                            int lSEQ = 0;
                            foreach (DataRow row in dtComponents.Rows)
                            {
                                pSEQ.Value = ++lSEQ;
                                int nValue = DBAccess.ReadIntValue(row["ComponentValue"]);
                                pComponent.Value = nValue;
                                lCTRowsAffected += oCommand.ExecuteNonQuery();
                            }

                            // clean out orphans that may have been caused in WEIGHTS
                            cmdText = "DELETE FROM EPGP_PI_PRI_WEIGHTS Where CW_COMPONENT Not In (Select CC_COMPONENT From EPGP_PI_PRI_COMPONENTS)";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            lRowsAffected = oCommand.ExecuteNonQuery();

                            eStatus = UpdateCalcs(dba);
                        }
                        dba.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        eStatus = dba.HandleStatusError(SeverityEnum.Exception, "UpdateComponents", (StatusEnum) 99852, ex.Message);
                        dba.RollbackTransaction();
                    }
                }
            }
            return eStatus;
        }

        public static StatusEnum UpdateFormulas(DBAccess dba, DataTable dtFormulas, out int lRowsAffected)
        {
            StatusEnum eStatus = StatusEnum.rsSuccess;
            lRowsAffected = 0;

            SqlCommand oCommand;
            string cmdText;

            // need to read existing formulas and components
            DataTable dt;
            List<int> Components = new List<int>();
            if (SelectComponents(dba, out dt) != StatusEnum.rsSuccess) goto Status_Error;
            foreach (DataRow row in dt.Rows)
            {
                int lFieldID = DBAccess.ReadIntValue(row["CC_COMPONENT"]);
                Components.Add(lFieldID);
            }

            List<int> OLDFormulas = new List<int>();
            if (SelectFormulas(dba, out dt) != StatusEnum.rsSuccess) goto Status_Error;
            foreach (DataRow row in dt.Rows)
            {
                int lFieldID = DBAccess.ReadIntValue(row["CW_RESULT"]);
                OLDFormulas.Add(lFieldID);
            }


            if (eStatus == StatusEnum.rsSuccess)
            {
                lRowsAffected = 0;
                {
                    dba.BeginTransaction();
                    try
                    {
                        if (dtFormulas.Rows.Count > 0)
                        {
                            cmdText =
                                "INSERT INTO EPGP_PI_PRI_WEIGHTS (CW_RESULT,CW_COMPONENT,CW_RATIO) VALUES(@CW_RESULT,@CW_COMPONENT,@CW_RATIO)";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            SqlParameter pRESULT = oCommand.Parameters.Add("@CW_RESULT", SqlDbType.Int);
                            SqlParameter pCOMPONENT = oCommand.Parameters.Add("@CW_COMPONENT", SqlDbType.Int);
                            oCommand.Parameters.AddWithValue("@CW_RATIO", 0);

                            foreach (DataRow row in dtFormulas.Rows)
                            {
                                int nFormula = DBAccess.ReadIntValue(row["Formula"]);
                                if (OLDFormulas.Contains(nFormula))
                                {
                                    OLDFormulas.Remove(nFormula);
                                }
                                else
                                {
                                    // add an entry into WEIGHT table for each component
                                    foreach (int nComponent in Components)
                                    {
                                        pRESULT.Value = nFormula;
                                        pCOMPONENT.Value = nComponent;
                                        int lCCRowsAffected = oCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        // delete formulas no longer needed
                        foreach (int nFormula in OLDFormulas)
                        {
                            cmdText = "DELETE FROM EPGP_PI_PRI_WEIGHTS Where CW_RESULT=" + nFormula.ToString("0");
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            int lCCRowsAffected = oCommand.ExecuteNonQuery();
                        }

                        eStatus = UpdateCalcs(dba);

                        dba.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        eStatus = dba.HandleStatusError(SeverityEnum.Exception, "UpdateFormulas", (StatusEnum) 99851, ex.Message);
                        dba.RollbackTransaction();
                    }
                }
            }
            Status_Error:

            return eStatus;
        }

        public static StatusEnum UpdateWeights(DBAccess dba, List<EPKPriFormula> Formulas, out int lRowsAffected)
        {
            StatusEnum eStatus = StatusEnum.rsSuccess;
            lRowsAffected = 0;

            SqlCommand oCommand;
            string cmdText;

            if (eStatus == StatusEnum.rsSuccess)
            {
                lRowsAffected = 0;
                {
                    dba.BeginTransaction();
                    try
                    {
                        cmdText = "DELETE FROM EPGP_PI_PRI_WEIGHTS";
                        oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                        lRowsAffected = oCommand.ExecuteNonQuery();

                        if (Formulas.Count > 0)
                        {
                            cmdText =
                                "INSERT INTO EPGP_PI_PRI_WEIGHTS (CW_RESULT,CW_COMPONENT,CW_RATIO) VALUES(@CW_RESULT,@CW_COMPONENT,@CW_RATIO)";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            SqlParameter pResult = oCommand.Parameters.Add("@CW_RESULT", SqlDbType.Int);
                            SqlParameter pComponent = oCommand.Parameters.Add("@CW_COMPONENT", SqlDbType.Int);
                            SqlParameter pRatio = oCommand.Parameters.Add("@CW_RATIO", SqlDbType.Float);
                            pRatio.Precision = 25;
                            pRatio.Scale = 6;

                            int lCWRowsAffected = 0;
                            foreach (EPKPriFormula Formula in Formulas)
                            {
                                foreach (ComponentWeight component in Formula.components)
                                {
                                    pResult.Value = Formula.uid;
                                    pComponent.Value = component.ComponentId;
                                    pRatio.Value = component.Weight;
                                    lCWRowsAffected += oCommand.ExecuteNonQuery();
                                }
                            }
                            eStatus = UpdateCalcs(dba);
                        }
                        dba.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        eStatus = dba.HandleStatusError(SeverityEnum.Exception, "UpdateComponents", (StatusEnum) 99850, ex.Message);
                        dba.RollbackTransaction();
                    }
                }
            }
            if (dba.Status == StatusEnum.rsSuccess) eStatus = PerformCustomFieldsCalculate(dba);

            return eStatus;
        }

        private static StatusEnum UpdateCalcs(DBAccess dba)
        {
            StatusEnum eStatus = StatusEnum.rsSuccess;

            SqlCommand oCommand;
            string cmdText;

            // read formulas and components
            DataTable dt;
            List<int> Components = new List<int>();
            if (SelectComponents(dba, out dt) != StatusEnum.rsSuccess) goto Status_Error;
            foreach (DataRow row in dt.Rows)
            {
                int lFieldID = DBAccess.ReadIntValue(row["CC_COMPONENT"]);
                Components.Add(lFieldID);
            }

            List<int> Formulas = new List<int>();
            if (SelectFormulas(dba, out dt) != StatusEnum.rsSuccess) goto Status_Error;
            foreach (DataRow row in dt.Rows)
            {
                int lFieldID = DBAccess.ReadIntValue(row["CW_RESULT"]);
                Formulas.Add(lFieldID);
            }

            if (eStatus == StatusEnum.rsSuccess)
            {
                int lRowsAffected = 0;
                {
                    try
                    {
                        cmdText = "DELETE FROM EPGP_CALCS Where CL_PRI=1";
                        oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                        lRowsAffected = oCommand.ExecuteNonQuery();

                        if (Formulas.Count > 0 && Components.Count > 0)
                        {
                            // get UID for next formula
                            int nNewCLUID = 0;
                            cmdText = "SELECT MAX(CL_UID) As maxUID FROM EPGP_CALCS";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            SqlDataReader reader = oCommand.ExecuteReader();

                            if (reader.Read())
                            {
                                nNewCLUID = DBAccess.ReadIntValue(reader["maxUID"]);
                            }
                            reader.Close();

                            cmdText =
                                "INSERT INTO EPGP_CALCS (CL_OBJECT,CL_UID,CL_SEQ,CL_RESULT,CL_COMPONENT,CL_RATIO,CL_OP,CL_PRI) VALUES(@CL_OBJECT,@CL_UID,@CL_SEQ,@CL_RESULT,@CL_COMPONENT,@CL_RATIO,@CL_OP,@CL_PRI)";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            oCommand.Parameters.AddWithValue("@CL_OBJECT", 1);
                            SqlParameter pUID = oCommand.Parameters.Add("@CL_UID", SqlDbType.Int);
                            SqlParameter pSEQ = oCommand.Parameters.Add("@CL_SEQ", SqlDbType.Int);
                            SqlParameter pRESULT = oCommand.Parameters.Add("@CL_RESULT", SqlDbType.Int);
                            SqlParameter pCOMPONENT = oCommand.Parameters.Add("@CL_COMPONENT", SqlDbType.Int);
                            oCommand.Parameters.AddWithValue("@CL_RATIO", 0);
                            oCommand.Parameters.AddWithValue("@CL_OP", 0);
                            oCommand.Parameters.AddWithValue("@CL_PRI", 1);

                            foreach (int nFormula in Formulas)
                            {
                                // add an entry into CLAC table for each component for each formula
                                int lSEQ = 0;
                                nNewCLUID = nNewCLUID + 1;
                                foreach (int nComponent in Components)
                                {
                                    lSEQ = lSEQ + 1;
                                    pUID.Value = nNewCLUID;
                                    pSEQ.Value = lSEQ;
                                    pRESULT.Value = nFormula;
                                    pCOMPONENT.Value = nComponent;
                                    int lCCRowsAffected = oCommand.ExecuteNonQuery();
                                }
                            }

                            // update the Ratios in the CALC table from the Weights table
                            cmdText = "Update EPGP_CALCS Set CL_RATIO=CW_RATIO"
                                      + " From EPGP_CALCS JOIN EPGP_PI_PRI_WEIGHTS"
                                      +
                                      " On EPGP_CALCS.CL_RESULT=EPGP_PI_PRI_WEIGHTS.CW_RESULT And EPGP_CALCS.CL_COMPONENT=EPGP_PI_PRI_WEIGHTS.CW_COMPONENT"
                                      + " And EPGP_CALCS.CL_PRI=1";
                            oCommand = new SqlCommand(cmdText, dba.Connection, dba.Transaction);
                            lRowsAffected = oCommand.ExecuteNonQuery();

                        }
                    }
                    catch (Exception ex)
                    {
                        eStatus = dba.HandleStatusError(SeverityEnum.Exception, "UpdateComponents", (StatusEnum) 99849,
                                                        ex.Message.ToString());
                    }
                }
            }

            Status_Error:

            return eStatus;
        }

        private static StatusEnum PerformCustomFieldsCalculate(DBAccess dba)
        {
            var sqlStatements = new List<string>();
            var seqStmt = string.Empty;
            var seqStmtStringBuilder = new StringBuilder(seqStmt);
            var lastId = -1;

            //CC-78062 - leaving as concatenation because since it's all static strings it will be inlined by the compiler, and keeps the formatting nicer than using interpolation or string format.
            const string Command = "SELECT     EPGP_CALCS.CL_UID, EPGP_CALCS.CL_SEQ, EPGP_CALCS.CL_RESULT, EPGC_FIELD_ATTRIBS.FA_FIELD_IN_TABLE, "
                + "EPGP_CALCS.CL_COMPONENT, EPGC_FIELD_ATTRIBS_1.FA_FIELD_IN_TABLE AS Expr1, EPGP_CALCS.CL_RATIO, EPGP_CALCS.CL_OP, "
                + "EPGC_FIELD_ATTRIBS_1.FA_TABLE_ID AS EXFAT "
                + "FROM         EPGP_CALCS INNER JOIN "
                + "EPGC_FIELD_ATTRIBS ON EPGP_CALCS.CL_RESULT = EPGC_FIELD_ATTRIBS.FA_FIELD_ID LEFT JOIN "
                + "EPGC_FIELD_ATTRIBS AS EPGC_FIELD_ATTRIBS_1 ON EPGP_CALCS.CL_COMPONENT = EPGC_FIELD_ATTRIBS_1.FA_FIELD_ID "
                + "Where (EPGP_CALCS.CL_OBJECT = 1) "
                + " AND (EPGP_CALCS.CL_PRI = 1) "
                + " ORDER BY EPGP_CALCS.CL_UID, EPGP_CALCS.CL_SEQ";

            var whereClause = string.Empty;

            using (var sqlCommand = new SqlCommand(Command, dba.Connection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    var errSql = string.Empty;

                    while (reader.Read())
                    {
                        var id = SqlDb.ReadIntValue(reader["CL_UID"]);
                        var fieldInTable = 0;

                        HandleLastFieldCase(
                            ref lastId,
                            id,
                            ref seqStmt,
                            sqlStatements,
                            reader,
                            ref seqStmtStringBuilder,
                            ref whereClause,
                            ref errSql);

                        var seq = SqlDb.ReadIntValue(reader["CL_SEQ"]);
                        var operationId = SqlDb.ReadIntValue(reader["CL_OP"]);

                        HandleMiddleFieldCase(seq, operationId, seqStmtStringBuilder, ref seqStmt);

                        fieldInTable = SqlDb.ReadIntValue(reader["Expr1"]);
                        var fat = SqlDb.ReadIntValue(reader["EXFAT"]);
                        var ratio = SqlDb.ReadDoubleValue(reader["CL_RATIO"]);

                        HandleFirstFieldCase(fieldInTable, seqStmtStringBuilder, ratio, fat, seq, operationId, out seqStmt, ref whereClause);
                    }

                    AddCommands(seqStmt, seqStmtStringBuilder, sqlStatements, whereClause, errSql);
                }
            }

            ExecuteSqlCommands(dba, sqlStatements);
            ReadServerUrls(dba);

            return dba.Status;
        }

        public static void HandleLastFieldCase(
            ref int lastId,
            int id,
            ref string seqStmt,
            List<string> sqlStatements,
            SqlDataReader reader,
            ref StringBuilder seqStmtStringBuilder,
            ref string whereClause,
            ref string errSql)
        {
            int fieldInTable;
            if (lastId != id)
            {
                if (!string.IsNullOrWhiteSpace(seqStmt))
                {
                    seqStmtStringBuilder.Append(
                        "  FROM EPGP_PROJECT_DEC_VALUES  INNER JOIN  EPGP_PROJECT_TEXT_VALUES ON EPGP_PROJECT_DEC_VALUES.PROJECT_ID = EPGP_PROJECT_TEXT_VALUES.PROJECT_ID ");
                    seqStmt = seqStmtStringBuilder.ToString();

                    sqlStatements.Add(string.Format("{0}{1}", seqStmt, whereClause));

                    if (!string.IsNullOrWhiteSpace(whereClause))
                    {
                        sqlStatements.Add(string.Format("{0}{1}", errSql, whereClause));
                    }
                }

                whereClause = string.Empty;

                lastId = id;

                fieldInTable = SqlDb.ReadIntValue(reader["FA_FIELD_IN_TABLE"]);

                seqStmt = string.Format("UPDATE EPGP_PROJECT_DEC_VALUES SET PC_{0:000} = ", fieldInTable);
                seqStmtStringBuilder = new StringBuilder(seqStmt);

                errSql = string.Format("UPDATE EPGP_PROJECT_DEC_VALUES SET PC_{0:000} = 999999 ", fieldInTable);
            }
        }

        private static void HandleMiddleFieldCase(int seq, int operationId, StringBuilder seqStmtStringBuilder, ref string seqStmt)
        {
            if (seq != 1)
            {
                string operation;
                switch (operationId)
                {
                    case 1:
                        operation = " - ";
                        break;
                    case 2:
                        operation = " * ";
                        break;
                    case 3:
                        operation = " / ";
                        break;
                    default:
                        operation = " + ";
                        break;
                }

                seqStmtStringBuilder.Append(operation);
                seqStmt = seqStmtStringBuilder.ToString();
            }
        }

        private static void HandleFirstFieldCase(
            int fieldInTable,
            StringBuilder seqStmtStringBuilder,
            double ratio,
            int fat,
            int seq,
            int operationId,
            out string seqStmt,
            ref string whereClause)
        {
            if (fieldInTable == 0)
            {
                seqStmtStringBuilder.Append(ratio.ToString());
                seqStmt = seqStmtStringBuilder.ToString();
            }
            else
            {
                var sval = GetCustFieldVal(fieldInTable, fat);

                seqStmtStringBuilder.Append(string.Format("({0} * {1})", sval, ratio));
                seqStmt = seqStmtStringBuilder.ToString();

                if (seq != 1)
                {
                    const int operationIdToCheck = 3;
                    if (operationId == operationIdToCheck)
                    {
                        if (whereClause == string.Empty)
                        {
                            whereClause = string.Format(" WHERE ({0}<> 0) ", sval);
                        }
                        else
                        {
                            var whereStringBuilder = new StringBuilder(whereClause);
                            whereStringBuilder.Append(string.Format(" AND ({0}<> 0) ", sval));
                            whereClause = whereStringBuilder.ToString();
                        }
                    }
                }
            }
        }

        private static void AddCommands(string seqStmt, StringBuilder stringBuilder, List<string> sqlStatements, string whereClause, string errSql)
        {
            if (!string.IsNullOrWhiteSpace(seqStmt))
            {
                stringBuilder.Append(
                    "  FROM EPGP_PROJECT_DEC_VALUES  INNER JOIN  EPGP_PROJECT_TEXT_VALUES ON EPGP_PROJECT_DEC_VALUES.PROJECT_ID = EPGP_PROJECT_TEXT_VALUES.PROJECT_ID ");
                seqStmt = stringBuilder.ToString();

                sqlStatements.Add(string.Format("{0}{1}", seqStmt, whereClause));
                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    sqlStatements.Add(string.Format("{0}{1}", errSql, whereClause));
                }
            }
        }

        private static void ExecuteSqlCommands(DBAccess dbAccess, List<string> sqlStatements)
        {
            foreach (var sql in sqlStatements)
            {
                using (var sqlCommand = new SqlCommand(sql, dbAccess.Connection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private static void ReadServerUrls(DBAccess dbAccess)
        {
            var weServerUrl = string.Empty;

            const string Command = "SELECT ADM_WE_SERVERURL FROM EPG_ADMIN";

            using (var sqlCommand = new SqlCommand(Command, dbAccess.Connection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        weServerUrl = SqlDb.ReadStringValue(reader["ADM_WE_SERVERURL"]);
                    }
                }
            }
        }

        public static string GetCustFieldVal(int lfit, int lfat)
        {
            string sfn = "0";
            if (lfat == 203)
                sfn = "PC_" + lfit.ToString("000");
            else if (lfat == 202)
            {
                //sfn = "CAST(PT_" + lfit.ToString("000") + " AS int)";

                //  want: IsNull(cast(Left(PT_003, PatIndex('%[^0-9]%', PT_003+'x' ) - 1 ) as int),0)
                sfn = "IsNull(cast(Left(PT_" + lfit.ToString("000") + ", PatIndex('%[^0-9]%', PT_" +
                      lfit.ToString("000") + "+'x' ) - 1 ) as int),0)";
            }

            return sfn;
        }

    }
}
