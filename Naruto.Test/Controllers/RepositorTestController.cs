﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Naruto.Domain.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Naruto.Repository.UnitOfWork;
using System.Data;
using Naruto.Domain.Model;
using System.Threading;

namespace Naruto.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RepositorTestController : ControllerBase
    {
        IUnitOfWork<MysqlDbContent> unit;
        public RepositorTestController(IUnitOfWork<MysqlDbContent> ofWork)
        {
            unit = ofWork;
        }
        public async Task<IActionResult> test()
        {
            List<setting> settings = new List<setting>();
            for (int i = 0; i < 10; i++)
            {
                settings.Add(new setting() { Contact = "1", Description = "1", DuringTime = "1", Integral = 1, Rule = "1" });
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            await unit.Command<setting>().BulkAddAsync(settings);
            Thread.Sleep(1000);
            await unit.SaveChangeAsync();
            stopwatch.Stop();
            return new JsonResult(new { stopwatch.ElapsedMilliseconds });
        }

        public async Task<IActionResult> testbulk()
        {
            DataTable dt = new DataTable
            {
                TableName = "setting"
            };
            dt.Columns.Add("Contact");
            dt.Columns.Add("Description");
            dt.Columns.Add("DuringTime");
            dt.Columns.Add("Integral");
            dt.Columns.Add("Rule");
            for (int i = 0; i < 10000; i++)
            {
                DataRow dr = dt.NewRow();
                dr["Contact"] = "1";
                dr["Description"] = "1";
                dr["DuringTime"] = "1";
                dr["Integral"] = "1";
                dr["Rule"] = "1";
                dt.Rows.Add(dr);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
           // await dt.BulkLoadAsync("");
            stopwatch.Stop();
            return new JsonResult(new { stopwatch.ElapsedMilliseconds });
        }
    }
}