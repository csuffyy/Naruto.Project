﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fate.Common.Repository.Interface
{
    public interface ISqlCommand : ISqlCommon
    {
        /// <summary>
        /// 执行增删改的异步操作 返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string sql, params object[] _params);
        /// <summary>
        /// 执行增删改的操作 返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, params object[] _params);
    }
    /// <summary>
    /// 张海波
    /// 2019-10-25
    /// 执行SQL语句的增删改操作
    /// </summary>
    public interface ISqlCommand<TDbContext> : ISqlCommand, IRepositoryDependency where TDbContext : DbContext
    {

    }
}
