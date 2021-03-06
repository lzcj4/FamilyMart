﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Reflection;

namespace DAL.Common
{
    public class DBHelper : IDisposable
    {
        private SQLiteConnection sqlCon;
        private const string dbPath = "./DB/FMDB.db";
        private const string sqlScriptPath = "./DB/FamilyMateDB.sql";
        private const string connectionStr = "Data Source=./DB/FMDB.db;Version=3;";
        public DBHelper()
        {
            bool isDBExisted = false;
            if (!(isDBExisted = File.Exists(dbPath)))
            {
                using (File.Create(dbPath))
                {
                }
            }
            sqlCon = new SQLiteConnection(connectionStr);
            OpenConnection();

            if (!isDBExisted)
            {
                var s = this.GetType().Assembly.GetManifestResourceStream(sqlScriptPath);
                if (File.Exists(sqlScriptPath))
                {
                    using (FileStream fs = File.OpenRead(sqlScriptPath))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string sql = sr.ReadToEnd();
                            ExecuteSqlNonQuery(sql);
                        }

                    }
                }
            }          
        }

        private void OpenConnection()
        {
            if (sqlCon.State == ConnectionState.Closed || sqlCon.State == ConnectionState.Broken)
            {
                sqlCon.Open();
            }
        }

        private void CheckConnection()
        {
            if (sqlCon.State == ConnectionState.Closed || sqlCon.State == ConnectionState.Broken)
            {
                throw new InvalidOperationException("当前数据库连接非法");
            }
        }

        public SQLiteDataReader Query(string sql)
        {
            OpenConnection();
            using (SQLiteCommand cmd = new SQLiteCommand(sqlCon))
            {
                cmd.CommandText = sql;
                return cmd.ExecuteReader();
            }
        }
        public int Update(string sql)
        {
            return ExecuteSqlNonQuery(sql);
        }
        public int Insert(string sql)
        {
            return ExecuteSqlNonQuery(sql);
        }

        public int Delete(string sql)
        {
            return ExecuteSqlNonQuery(sql);
        }

        /// <summary>
        /// Return the recent insert row id
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int ExecuteSqlNonQuery(string sql)
        {
            OpenConnection();
            using (SQLiteCommand cmd = sqlCon.CreateCommand())
            {
                using (var sqlTrans = sqlCon.BeginTransaction())
                {
                    try
                    {
                        cmd.CommandText = sql;
                        int rowCount = cmd.ExecuteNonQuery();
                        sqlTrans.Commit();
                        rowCount = (int)sqlCon.LastInsertRowId;
                        return rowCount;
                    }
                    catch (System.Exception ex)
                    {
                        sqlTrans.Rollback();
                        throw ex;
                    }
                }
            }
        }


        #region IDisposable Members

        protected void Disposing(bool isDisposed)
        {
            if (sqlCon != null && sqlCon.State != ConnectionState.Closed)
            {
                sqlCon.Close();
                sqlCon.Dispose();
            }
        }

        public void Dispose()
        {
            Disposing(true);
        }

        #endregion
    }
}
