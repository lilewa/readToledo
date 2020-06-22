using System;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;

namespace wmsDH.DataAccess
{

    public class ExecuteFromXml
    {
        public static Task<Tuple<int,string>> ExecuteReturnInt(string spaceAndForm, string funcName, Dictionary<string, object> dic=null)
        {
            return Task.Factory.StartNew(() =>
            {
                int returnVal = 0;
                string errMsg = string.Empty;


                int place = spaceAndForm.LastIndexOf('.');
                //location=/root/JGWL.WindowsForm.WLGoosArrival/Form_Fhmx_Excel/btnReceive
                string location = "/root/" + spaceAndForm.Substring(0, place) + "/" + spaceAndForm.Substring(place + 1) + "/" + funcName;
                XmlNode node = OracleHelper.dataAccessXml.SelectSingleNode(location);
                if (node == null)
                {
                    errMsg = "xml中未找到节点";
                    return Tuple.Create(returnVal, errMsg);
                }
                int paraCount = node.SelectNodes("parameter").Count;//xml中参数个数
                OracleParameter[] oracleParam = { };
                if (paraCount != 0)
                {
                    oracleParam = new OracleParameter[paraCount];
                }

                CommandType cmdType = new CommandType();
                string sql = "";//过程名或sql语句
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    //<parameter name="bjlsh" type="Char" direction="in"></parameter> 
                    //<commandType>Text</commandType>
                    //<sql>update t_dfbj set wcbj='0' where bjlsh=:bjlsh</sql>
                    if (node.ChildNodes[i].Name == "parameter")
                    {
                        oracleParam[i] = new OracleParameter();
                        OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), node.ChildNodes[i].Attributes["type"].Value, true);
                        oracleParam[i].OracleDbType = paramType;
                        oracleParam[i].ParameterName = node.ChildNodes[i].Attributes["name"].Value;
                        if (node.ChildNodes[i].Attributes["direction"].Value == "out")
                        {
                            oracleParam[i].Direction = ParameterDirection.Output;
                            oracleParam[i].Size = 500;
                        }
                        else
                        {
                            if (dic.ContainsKey(oracleParam[i].ParameterName))
                            {
                                oracleParam[i].Value = dic[oracleParam[i].ParameterName];
                            }
                            else
                            {
                                return Tuple.Create(0, "未给出参数" + oracleParam[i].ParameterName + "的值");
                                // errMsg = "未给出参数" + oracleParam[i].ParameterName + "的值";
                                // return 0;
                            }
                        }
                    }
                    else if (node.ChildNodes[i].Name == "commandType")
                    {
                        cmdType = (CommandType)Enum.Parse(typeof(CommandType), node.ChildNodes[i].InnerText, true);

                    }
                    else
                    {
                        sql = node.ChildNodes[i].InnerText;
                    }
                }
                try
                {

                    if (cmdType == CommandType.StoredProcedure)
                    {
                        OracleHelper.ExecuteNonQuery(cmdType, sql, oracleParam);
                        string rtn = oracleParam[paraCount - 1].Value.ToString().Trim().ToLower();
                        if (rtn != "ok" && rtn != "null")
                        {
                            errMsg = rtn;
                        }
                    }
                    else
                    {
                        returnVal = OracleHelper.ExecuteNonQuery(cmdType, sql, oracleParam);

                    }

                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }

                return Tuple.Create(returnVal, errMsg);
            });
        }

        public static Task<Tuple<DataTable, string, string>> ExecuteReturnTb(string spaceAndForm, string funcName, Dictionary<string, object> dic = null)
        { 
            return  Task.Factory.StartNew(() => {

                
                DataTable dt = null;
                string errMsg = string.Empty;
                string selectSql = string.Empty;

                int outParaStartIndex = 0; //oracleParam中out参数的其实序号
                int place = spaceAndForm.LastIndexOf('.');
                //location=/root/JGWL.WindowsForm.WLGoosArrival/Form_Fhmx_Excel/btnReceive
                string location = "/root/" + spaceAndForm.Substring(0, place) + "/" + spaceAndForm.Substring(place + 1) + "/" + funcName;
                XmlNode node = OracleHelper.dataAccessXml.SelectSingleNode(location);
                if (node == null)
                {
                    errMsg = "xml中未找到节点";
                    return Tuple.Create(dt,errMsg ,selectSql);
                    //  throw new Exception("xml中未找到节点");
                }

                int paraCount = node.SelectNodes("parameter").Count;//xml中参数个数

                OracleParameter[] oracleParam = { };
                if (paraCount != 0)
                {
                    oracleParam = new OracleParameter[paraCount];
                }

                CommandType cmdType = new CommandType();
                string sql = "";//过程名或sql语句
                int k = 0;
                bool outParaStartIndexFind = false;
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    //<parameter name="bjlsh" type="Char" direction="in"></parameter> 
                    //<commandType>Text</commandType>
                    //<sql>update t_dfbj set wcbj='0' where bjlsh=:bjlsh</sql>
                    if (node.ChildNodes[i].Name == "parameter")
                    {

                        oracleParam[k] = new OracleParameter();
                        OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), node.ChildNodes[i].Attributes["type"].Value, true);
                        oracleParam[k].OracleDbType = paramType;
                        oracleParam[k].ParameterName = node.ChildNodes[i].Attributes["name"].Value;
                        if (node.ChildNodes[i].Attributes["direction"].Value == "out")
                        {
                            //存储过程多个返回值放到datatable中返回。
                            dt.Columns.Add(node.ChildNodes[i].Attributes["name"].Value);
                            oracleParam[k].Direction = ParameterDirection.Output;
                            oracleParam[k].Size = 500;
                            if (!outParaStartIndexFind)//oracleParam中out参数的真实序号
                            {
                                outParaStartIndexFind = true;
                                outParaStartIndex = k;
                            }
                        }
                        else
                        {
                            if (dic.ContainsKey(oracleParam[k].ParameterName))
                            {
                                oracleParam[k].Value = dic[oracleParam[k].ParameterName];
                            }
                            else
                            {
                                return Tuple.Create(dt, "未给出参数" + oracleParam[k].ParameterName + "的值", selectSql);

                            }

                            //oracleParam[k].Value = dic[oracleParam[k].ParameterName];
                        }
                        k++;
                    }
                    else if (node.ChildNodes[i].Name == "commandType")
                    {
                        cmdType = (CommandType)Enum.Parse(typeof(CommandType), node.ChildNodes[i].InnerText, true);

                    }
                    else
                    {
                        selectSql = sql = node.ChildNodes[i].InnerText;
                    }
                }
                try
                {
                    if (cmdType == CommandType.StoredProcedure)
                    {
                        int returnVal = OracleHelper.ExecuteNonQuery(cmdType, sql, oracleParam);

                        DataRow dr = dt.NewRow();
                        for (int i = outParaStartIndex; i < oracleParam.Length; i++)
                        {
                            dr[i - outParaStartIndex] = oracleParam[i].Value;

                        }
                        dt.Rows.Add(dr);
                        dt.TableName = "1";

                        string rtn = oracleParam[paraCount - 1].Value.ToString().Trim().ToLower();
                        if (rtn != "ok" && rtn != "null")
                        {
                            errMsg = rtn;

                        }
                    }
                    else
                    {
                        dt = OracleHelper.ExecuteDataset(cmdType, sql, oracleParam).Tables[0];

                    }

                }
                catch (Exception ex)
                {

                    errMsg = ex.Message;
                }

                return Tuple.Create(dt, errMsg, selectSql);

            });
            
        }
         
        public static string ExecuteTrans(string spaceAndForm, string funcName, Dictionary<string, object>[] param)
        {
            string errMsg = string.Empty;
         
            //获取xml文件中的指定节点
            int place = spaceAndForm.LastIndexOf('.');
            string location = "/root/" + spaceAndForm.Substring(0, place) + "/" + spaceAndForm.Substring(place + 1) + "/" + funcName;
            XmlNode node = OracleHelper.dataAccessXml.SelectSingleNode(location);

            // 根据xml准备command
            OracleCommand command = new OracleCommand();


            int paraCount = node.SelectNodes("parameter").Count;//xml中参数个数 
            OracleParameter[] oracleParam = { };
            if (paraCount != 0)
            {
                oracleParam = new OracleParameter[paraCount];
            }

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                //<parameter name="bjlsh" type="Char" direction="in"></parameter> 
                //<commandType>Text</commandType>
                //<sql>update t_dfbj set wcbj='0' where bjlsh=:bjlsh<l>
                if (node.ChildNodes[i].Name == "parameter")
                {
                    oracleParam[i] = new OracleParameter();
                    OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), node.ChildNodes[i].Attributes["type"].Value, true);
                    oracleParam[i].OracleDbType = paramType;
                    oracleParam[i].ParameterName = node.ChildNodes[i].Attributes["name"].Value;
                    if (node.ChildNodes[i].Attributes["direction"].Value == "out")
                    {
                        oracleParam[i].Direction = ParameterDirection.Output;
                        oracleParam[i].Size = 500;
                    }
                    command.Parameters.Add(oracleParam[i]);

                }
                else if (node.ChildNodes[i].Name == "commandType")
                {
                    command.CommandType = (CommandType)Enum.Parse(typeof(CommandType), node.ChildNodes[i].InnerText, true);

                }
                else
                {
                    command.CommandText = node.ChildNodes[i].InnerText;
                }
            }
            using (OracleConnection connection = new OracleConnection(OracleHelper.LocalConnectionString))
            {
                connection.Open();

                OracleTransaction transaction;

                // Start a local transaction
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {

                    foreach (Dictionary<string, object> dic in param)
                    {
                        // Dictionary<string, object> dic = Param2Dic(jsonParam);

                        foreach (OracleParameter p in command.Parameters)
                        {
                            if (p.Direction != ParameterDirection.Output)
                            {
                                p.Value = dic[p.ParameterName];
                            }
                        }
                        command.ExecuteNonQuery();
                        if (command.CommandType == CommandType.StoredProcedure)
                        {

                            if (oracleParam[paraCount - 1].Value.ToString().Trim().ToLower() != "ok")
                            {
                                errMsg = oracleParam[paraCount - 1].Value.ToString();
                                transaction.Rollback();
                                return errMsg;
                            }
                        }
                    }
                    transaction.Commit();
                    return errMsg;

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return errMsg = e.Message;


                }
            }

        }
         

        public static Task<Tuple<DataTable, string>> ExecuteDynamicSql(string spaceAndForm, string funcName, Dictionary<string, object> dic=null)
        {
            return Task.Factory.StartNew(() =>
            {
                string errMsg = string.Empty;

                DataTable dt = null;

                XmlNode node = SearchXml(spaceAndForm, funcName);
                if (node == null)
                {
                    errMsg = "xml中未找到节点";
                    return Tuple.Create(dt, errMsg);
                    //  throw new Exception("xml中未找到节点");
                }
                int paramLocation = 0;
                int funcLocation = node.Attributes["location"] == null ? 1 : int.Parse(node.Attributes["location"].Value);
                string[] condition = new string[funcLocation];

                XmlNode nodesql = node.SelectSingleNode("sql");
                string sql = nodesql.InnerText;

                int paraCount = dic.Count;//xml中参数个数
                OracleParameter[] oracleParam = new OracleParameter[paraCount];
                int k = 0;
                try
                {
                    foreach (string key in dic.Keys)
                    {
                        XmlNode nodeParam = node.SelectSingleNode("parameter[@name='" + key + "']");
                        if (nodeParam == null)
                        {
                            errMsg = "xml中未提供" + key;
                            return Tuple.Create(dt, errMsg);
                            //throw new Exception("xml中未提供" + key);
                        }
                        if (funcLocation > 1)
                        {
                            paramLocation = int.Parse(nodeParam.Attributes["location"].Value);
                        }

                        condition[paramLocation] += nodeParam.Attributes["condition"].Value;

                        oracleParam[k] = new OracleParameter();
                        OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), nodeParam.Attributes["type"].Value, true);
                        oracleParam[k].OracleDbType = paramType;
                        oracleParam[k].ParameterName = key;
                        oracleParam[k].Value = dic[key];
                        k++;
                    }
                    if (sql.Contains("{0}"))
                    {
                        sql = string.Format(sql, condition);
                    }
                    else
                    {
                        sql += condition[0];
                    }
                    XmlNode lastsql = node.SelectSingleNode("lastsql");
                    if (lastsql != null)
                    {
                        sql = sql + " " + lastsql.InnerText;
                    }
                    dt = OracleHelper.ExecuteDataset(CommandType.Text, sql, oracleParam).Tables[0];
                    dt.TableName = "1";

                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;

                }

                return Tuple.Create(dt, errMsg);
            });

        }

        //public static DataTable ExecuteCheckBoxTable(string spaceAndForm, string funcName, string[] dicParams, out string errMsg)
        //{


        //    errMsg = string.Empty; 
        //    Dictionary<string, object> dic = Param2Dic(dicParams) ;

        //    XmlNode node= SearchXml(spaceAndForm, funcName);
        //    if (node == null)
        //    {
        //        errMsg = "xml中未找到节点";
        //        return null;
        //    }


        //    int paraCount = node.SelectNodes("parameter").Count;
        //    OracleParameter[] oracleParam= new OracleParameter[paraCount];



        //    string sql = "";//过程名或sql语句
        //    for (int i = 0; i < node.ChildNodes.Count; i++)
        //    {
        //        //<parameter name="bjlsh" type="Char" direction="in"></parameter> 
        //        //<commandType>Text</commandType>
        //        //<sql>update t_dfbj set wcbj='0' where bjlsh=:bjlsh</sql>
        //        if (node.ChildNodes[i].Name == "parameter")
        //        {
        //            oracleParam[i] = new OracleParameter();
        //            OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), node.ChildNodes[i].Attributes["type"].Value, true);
        //            oracleParam[i].OracleDbType = paramType;
        //            oracleParam[i].ParameterName = node.ChildNodes[i].Attributes["name"].Value;

        //            if (dic.ContainsKey(oracleParam[i].ParameterName))
        //            {
        //                oracleParam[i].Value = dic[oracleParam[i].ParameterName];
        //            }
        //            else
        //            {
        //                errMsg = "未给出参数" + oracleParam[i].ParameterName + "的值";
        //                return null;
        //            }

        //        } 
        //        else if(node.ChildNodes[i].Name == "sql")
        //        {
        //             sql = node.ChildNodes[i].InnerText;
        //        }
        //    }
        //    try
        //    {
        //        DataTable dt = OracleHelper.CheckBoxTable(sql, oracleParam);
        //        dt.TableName = "1";
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //        return null;
        //    }

        //}

        //public static DataTable ExecuteCheckBoxTable(string spaceAndForm, string funcName, Dictionary<string, object> dic, out string errMsg)
        //{

        //    //errMsg = CheckToken(dic);
        //    //if (!string.IsNullOrEmpty(errMsg))
        //    //{
        //    //    return null;
        //    //}
        //    //  Dictionary<string, object> dic = Param2Dic(dicParams);

        //    XmlNode node = SearchXml(spaceAndForm, funcName);
        //    if (node == null)
        //    {
        //        errMsg = "xml中未找到节点";
        //        return null;
        //    }

        //    XmlNode nodesql = node.SelectSingleNode("sql");
        //    string sql = nodesql.InnerText;
        //    int paraCount = dic.Count;//xml中参数个数
        //    OracleParameter[] oracleParam = new OracleParameter[paraCount];
        //    int k = 0;
        //    foreach (string key in dic.Keys)
        //    {
        //        XmlNode nodeParam = node.SelectSingleNode("parameter[@name='" + key + "']");
        //        if (nodeParam == null)
        //        {
        //            errMsg = "xml中未提供" + key;
        //            return null;
        //        }
        //        sql += nodeParam.Attributes["condition"].Value;

        //        oracleParam[k] = new OracleParameter();
        //        OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), nodeParam.Attributes["type"].Value, true);
        //        oracleParam[k].OracleDbType = paramType;
        //        oracleParam[k].ParameterName = key;
        //        oracleParam[k].Value = dic[key];
        //        k++;
        //    }
        //    XmlNode lastsql = node.SelectSingleNode("lastsql");
        //    if (lastsql != null)
        //    {
        //        sql = sql + " " + lastsql.InnerText;
        //    }
        //    try
        //    {
        //        DataTable dt = OracleHelper.CheckBoxTable(sql, oracleParam);
        //        dt.TableName = "1";
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //        return null;
        //    }

        //}

        private static XmlNode SearchXml(string spaceAndForm, string funcName)
        {

            int place = spaceAndForm.LastIndexOf('.');
            //location=/root/JGWL.WindowsForm.WLGoosArrival/Form_Fhmx_Excel/btnReceive
            string location = "/root/" + spaceAndForm.Substring(0, place) + "/" + spaceAndForm.Substring(place + 1) + "/" + funcName;
            XmlNode node = OracleHelper.dataAccessXml.SelectSingleNode(location);
            return node;
        }
 
        

        public static DataTable ExecuteProcTb(string spaceAndForm, string funcName, Dictionary<string, object> dic, out string errMsg)
        {
            errMsg = string.Empty;
              
            DataTable dt = new DataTable();
          
            int outParaStartIndex = 0; //oracleParam中out参数的其实序号
            int place = spaceAndForm.LastIndexOf('.');
            //location=/root/JGWL.WindowsForm.WLGoosArrival/Form_Fhmx_Excel/btnReceive
            string location = "/root/" + spaceAndForm.Substring(0, place) + "/" + spaceAndForm.Substring(place + 1) + "/" + funcName;
            XmlNode node = OracleHelper.dataAccessXml.SelectSingleNode(location);

            int paraCount = node.SelectNodes("parameter").Count;//xml中参数个数

            OracleParameter[] oracleParam = { };
            if (paraCount != 0)
            {
                oracleParam = new OracleParameter[paraCount];
            }

            CommandType cmdType = new CommandType();
            string sql = "";//过程名或sql语句
            int k = 0;
            bool outParaStartIndexFind = false;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                //<parameter name="bjlsh" type="Char" direction="in"></parameter> 
                //<commandType>Text</commandType>
                //<sql>update t_dfbj set wcbj='0' where bjlsh=:bjlsh</sql>
                if (node.ChildNodes[i].Name == "parameter")
                {

                    oracleParam[k] = new OracleParameter();
                    OracleDbType paramType = (OracleDbType)Enum.Parse(typeof(OracleDbType), node.ChildNodes[i].Attributes["type"].Value, true);
                    oracleParam[k].OracleDbType = paramType;
                    oracleParam[k].ParameterName = node.ChildNodes[i].Attributes["name"].Value;
                    if (node.ChildNodes[i].Attributes["direction"].Value == "out")
                    {
                        //存储过程多个返回值放到datatable中返回。
                        dt.Columns.Add(node.ChildNodes[i].Attributes["name"].Value);
                        oracleParam[k].Direction = ParameterDirection.Output;
                        oracleParam[k].Size = 500;
                        if (!outParaStartIndexFind)//oracleParam中out参数的真实序号
                        {
                            outParaStartIndexFind = true;
                            outParaStartIndex = k;
                        }
                    }
                    else
                    {
                        oracleParam[k].Value = dic[oracleParam[k].ParameterName];
                    }
                    k++;
                }
                else if (node.ChildNodes[i].Name == "commandType")
                {
                    cmdType = (CommandType)Enum.Parse(typeof(CommandType), node.ChildNodes[i].InnerText, true);

                }
                else
                {
                    sql = node.ChildNodes[i].InnerText;
                }
            }
            try
            {
                dt = OracleHelper.ExecuteDataset(cmdType, sql, oracleParam).Tables[0];
                  

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                     
            }
           
            return dt;

        }
    }
}
