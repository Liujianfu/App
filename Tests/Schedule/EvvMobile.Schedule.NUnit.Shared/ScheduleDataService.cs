using System;
using System.Collections.Generic;
using System.Text;
using Evv.Message.Portable.Schedulers.Dtos;
using NUnit.Framework;
using EvvMobile.DataService.Services;
namespace EvvMobile.Schedule.NUnit.Shared
{
    [TestFixture]
    public class ScheduleDataService
    {
        [SetUp]
        public void Setup()
        {
            _schedulerService = new SchedulerService(Statics.AppConifgurations.ScheduleServiceBaseUrl);
        }


        [TearDown]
        public void Tear() { }

        [Test]
        public void DownloadMyFurtureSchedules_Pass()
        {
            var searchCriteria = new ServiceVisitSearchCriteriaDto
            {
                PageNumber = 1,
                PeriodStart = DateTimeOffset.Now.AddDays(-30),
                PeriodEnd = DateTimeOffset.Now.AddDays(1),
                //   VisitStaffId = "123456789"
            };
            var serviceVisitResult =  _schedulerService.DownloadMyFurtureSchedules(searchCriteria,true).Result;
            Assert.IsNotNull(serviceVisitResult.ModelObject);

        }


        private SchedulerService _schedulerService;
    }
}
