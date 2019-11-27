﻿using Fate.Infrastructure.Configuration.Management.Dashboard.Interface;
using Fate.Infrastructure.Configuration.Management.DB;
using Fate.Infrastructure.Configuration.Management.Object;
using Fate.Infrastructure.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Fate.Infrastructure.Redis.IRedisManage;

namespace Fate.Infrastructure.Configuration.Management.Dashboard.Services
{
    /// <summary>
    /// z张海波
    /// 2019-11-14
    /// 配置操作的默认服务提供
    /// </summary>
    public class DefaultConfigurationServices : IConfigurationServices
    {
        private readonly IUnitOfWork<ConfigurationDbContent> unitOfWork;

        private readonly IConfigurationPublish publish;
        public DefaultConfigurationServices(IUnitOfWork<ConfigurationDbContent> _unitOfWork, IConfigurationPublish _publish)
        {
            unitOfWork = _unitOfWork;
            publish = _publish;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<bool> AddConfiguration(ConfigurationEndPoint info)
        {
            if (info == null)
                return default;
            await unitOfWork.Command<ConfigurationEndPoint>().AddAsync(info);
            var res = await SaveChangeAsync();
            if (res)
            {
                await publish.PublishAsync();
            }
            return res;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteConfiguration(string[] ids)
        {
            if (ids == null || ids.Count() <= 0)
                return default;
            await unitOfWork.Command<ConfigurationEndPoint>().DeleteAsync(a => ids.Contains(a.Id));
            return await SaveChangeAsync();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<DashboardResult> QueryConfiguration(QueryConfigurationDTO info)
        {
            if (info == null)
                return default;
            //查询条件
            var configurationEndpoint = unitOfWork.Query<ConfigurationEndPoint>().AsQueryable()
                .WhereIf(!string.IsNullOrWhiteSpace(info.Group), a => a.Group.Contains(info.Group))
                .WhereIf(info.EnvironmentType >= 0, a => a.EnvironmentType == info.EnvironmentType).AsNoTracking();
            var result = new DashboardResult();
            result.msg = "查询成功";
            result.count = await configurationEndpoint.CountAsync();
            result.data = await configurationEndpoint.PageBy(info.Page, info.Limit).ToListAsync();
            return result;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<bool> UpdateConfiguration(ConfigurationEndPoint info)
        {
            if (info == null)
                return default;
            //修改
            await unitOfWork.Command<ConfigurationEndPoint>().UpdateAsync(a => a.Id == info.Id, item =>
             {
                 item.Value = info.Value;
                 item.Remark = info.Remark;
                 item.Key = info.Key;
                 item.Group = info.Group;
                 item.EnvironmentType = info.EnvironmentType;
                 return item;
             });

            var res = await SaveChangeAsync();
            if (res)
            {
                await publish.PublishAsync();
            }
            return res;
        }

        /// <summary>
        /// 查询单挑配置信息
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigurationEndPoint> QueryFirstConfiguration(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;
            return await unitOfWork.Query<ConfigurationEndPoint>().Where(a => a.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveChangeAsync()
        {
            return (await unitOfWork.SaveChangeAsync()) > 0;
        }
    }
}
