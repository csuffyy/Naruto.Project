﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fate.Domain.Model;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
namespace Fate.Common.Repository.Mysql.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MysqlDbContent dbContext;
        public UnitOfWork(MysqlDbContent _dbContext)
        {
            dbContext = _dbContext;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTransaction()
        {
            dbContext.Database.BeginTransaction();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            dbContext.Database.CommitTransaction();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            dbContext?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBackTransaction()
        {
            dbContext.Database.RollbackTransaction();
        }
        /// <summary>
        /// 异步提交
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangeAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public IRepository<T> Respositiy<T>() where T : class, IEntity
        {
            //获取仓储服务（需先注入仓储集合，否则将报错）
            IRepository<T> repository = dbContext.GetService<IRepository<T>>();
            repository.ChangeDbContext(dbContext);
            return repository;
        }

        /// <summary>
        /// 更改为只读连接字符串
        /// </summary>
        /// <returns></returns>
        public async Task ChangeReadOnlyConnection()
        {
            //获取配置文件
            var config = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            //获取只读的连接字符串
            var connection = config.GetConnectionString("ReadOnlyMySqlConnection");
            if (connection == null)
                throw new ApplicationException("数据库只读连接字符串不能为空");
            //获取连接字符串的数组 多个用|分割开
            var connections = connection.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (connections == null || connections.Count() <= 0)
                throw new ApplicationException("数据库只读连接字符串不能为空");
            //随机数
            var random = new Random();
            dbContext.Database.GetDbConnection().ConnectionString = connections[random.Next(0, connections.Count() - 1)];
            await Task.FromResult(0).ConfigureAwait(false);
        }
    }
}
