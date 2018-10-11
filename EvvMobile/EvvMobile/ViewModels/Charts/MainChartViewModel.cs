using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Evv.Message.Portable.Schedulers.Dtos;
using EvvMobile.Charts;
using EvvMobile.Charts.Layouts;
using EvvMobile.ViewModels.Base;
using EvvMobile.ViewModels.Systems;
using SkiaSharp;
using Xamarin.Forms;
using Entry = EvvMobile.Charts.Entry;

namespace EvvMobile.ViewModels.Charts
{
    public class MainChartViewModel : BaseViewModel
    {
        public MainChartViewModel()
        {
            _charts = new ObservableCollection<Chart>();

            PieChartEndDateTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(1);
            PieChartStartDateTime = PieChartEndDateTime.AddMonths(-12);
            PieChartEndDateTime=PieChartEndDateTime.AddHours(-1);
        }

        #region properties
        private ObservableCollection<Chart> _charts;
        private const string ChartsPropertyName = "Charts";
        public ObservableCollection<Chart> Charts
        {
            get { return _charts; }
            set
            {
                SetProperty(ref _charts, value, ChartsPropertyName);
            }
        }
        public Chart TwelveMonthLineChart { get; set; }
        public Chart PieChart { get; set; }
        private int _pieChartTotal = 0;

        public int PieChartTotal
        {
            get { return _pieChartTotal; }
            set
            {
                SetProperty(ref _pieChartTotal, value, "PieChartTotal");
            }
        }
        public DateTime PieChartStartDateTime { get; set; }
        public DateTime PieChartEndDateTime { get; set; }
        #endregion


        #region commands

        Command _loadStatusPieChartDataCommand;

        public Command LoadStatusPieChartDataCommand
        {
            get { return _loadStatusPieChartDataCommand ?? (
                             _loadStatusPieChartDataCommand = new Command<ChartDataSearchCriteria>(async (ChartDataSearchCriteria criteria) =>
                    await ExecuteStatusPieChartDataCommand(criteria), 
                    (criteria) => !IsBusy&& SystemViewModel.Instance.HasNetworkConnection));
            }
        }

        public async Task ExecuteStatusPieChartDataCommand(ChartDataSearchCriteria criteria)
        {
            if (IsBusy || /*criteria==null||*/  !SystemViewModel.Instance.HasNetworkConnection)
                return;

            IsBusy = true;
            IsInitialized = true;
            _charts.Clear();
            try
            {
                await GetStatusPieCharts(criteria);
                await GetMonthlyCharts();
            }
            catch (Exception e)
            {

            }
            IsBusy = false;
        }
        #endregion
        public override void RaiseCommandsChangeCanExecuteEvent()
        {
            LoadStatusPieChartDataCommand.ChangeCanExecute();

        }
        #region Private methods
        public async Task GetMonthlyCharts()
        {
            //TODO: get real data instead of using fake data
            var curDate = DateTime.Now;
            var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
            //TODO:turn on staff id 
            var monthlyCriteria = new ScheduleStatisticsCriteriaDto
            {
                // StaffId = SystemViewModel.Instance.CurrentStaffId,
                PeriodEnd = DateTimeOffset.Now,
                PeriodStart = DateTimeOffset.Now.AddMonths(-11),
            };
            var entries = new List<Entry>();
            var monthlyStatistics = await schedulerDataService.GetMonthlyScheduleStatisticsForStaff(monthlyCriteria);
            if (monthlyStatistics.ModelObject != null)
            {
                foreach (var month in monthlyStatistics.ModelObject.MonthResults)
                {
                    SKColor color = SKColor.Parse("#0000ff");

                    entries.Add(new Entry(month.TotalSchedules)
                    {
                        Label = month.Month.ToString("MMM"),
                        ValueLabel = month.TotalSchedules.ToString(),
                        Color = color
                    });
                }
            }
            else
            {
                //error
            }
            TwelveMonthLineChart = new LineChart()
            {
                Entries = entries,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                PointSize = 18,
                LabelTextSize = 25
            };
        }
        public async Task GetStatusPieCharts(ChartDataSearchCriteria criteria)
        {
            //TODO: get real data instead of using fake data
            //testing data , should get data from restful service when it is online
            //waiting indicator test
            try
            {
                var schedulerDataService = SystemViewModel.Instance.SchedulerDataService;
                var shiftStatusEntries = new List<Entry>();
                var pieChartTotal = 0;
                var searchCriteria = new ScheduleStatisticsCriteriaDto
                {
                    // StaffId = SystemViewModel.Instance.CurrentStaffId,
                    PeriodEnd = PieChartEndDateTime,
                    PeriodStart = PieChartStartDateTime,
                };

                var serviceVisitStatistics =
                    await schedulerDataService.GetScheduleStatusStatisticsForStaff(searchCriteria);
                LoadStatusPieChartDataCommand.ChangeCanExecute();
                if (serviceVisitStatistics.ModelObject != null)
                {
                    var statistisDto = serviceVisitStatistics.ModelObject;
                    var totalVisites = statistisDto.TotalSchedules<=0?1: statistisDto.TotalSchedules;
                    pieChartTotal = statistisDto.TotalSchedules;
                    shiftStatusEntries = new List<Entry>
                    {
                        new Entry(statistisDto.CompletedSchedules)
                        {
                            Label = "Completed",
                            ValueLabel = ((double)statistisDto.CompletedSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#00ff00")
                        },
                        new Entry(statistisDto.CompletedWihoutClockInSchedules)
                        {
                            Label = "Missing Clock In",
                            ValueLabel =  ((double)statistisDto.CompletedWihoutClockInSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#00ffFF")
                        },
                        new Entry(statistisDto.DeniedByMmisSchedules)
                        {
                            Label = "MMIS Denied",
                            ValueLabel = ((double)statistisDto.DeniedByMmisSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#FF0000")
                        },
                        new Entry(statistisDto.DiscardedSchedules)
                        {
                            Label = "Discarded",
                            ValueLabel = ((double)statistisDto.DiscardedSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#C3C3C3")
                        },
                        new Entry(statistisDto.InitiatedSchedules)
                        {
                            Label = "Initiated",
                            ValueLabel = ((double)statistisDto.InitiatedSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#F5817a")
                        },
                        new Entry(statistisDto.InprogressSchedules)
                        {
                            Label = "In Progress",
                            ValueLabel = ((double)statistisDto.InprogressSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#6ba3f5")
                        },
                        new Entry(statistisDto.MissedSchedules)
                        {
                            Label = "Missed",
                            ValueLabel = ((double)statistisDto.MissedSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#ff80ff")
                        },
                        new Entry(statistisDto.RejectedByClientSchedules)
                        {
                            Label = "Client Rejected",
                            ValueLabel = ((double)statistisDto.RejectedByClientSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#004080")
                        },
                        new Entry(statistisDto.RejectedByStaffSchedules)
                        {
                            Label = "Staff Rejected",
                            ValueLabel = ((double)statistisDto.RejectedByStaffSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#808040")
                        },
                        new Entry(statistisDto.ScheduledSchedules)
                        {
                            Label = "Scheduled",
                            ValueLabel = ((double)statistisDto.ScheduledSchedules/totalVisites).ToString("P", CultureInfo.InvariantCulture),
                            Color = SKColor.Parse("#008040")
                        },
                    };

                }
                var entries = new[]
                {
                new Entry(212)
                {
                    Label = "UWP",
                    ValueLabel = "212",
                    Color = SKColor.Parse("#2c3e50"),
                    SubEntries = new Collection<Entry>
                    {
                        new Entry(70)
                        {
                            Label = "Android",
                            ValueLabel = "70",
                            Color = SKColor.Parse("#FF0000")
                        },
                        new Entry(100)
                        {
                            Label = "Android",
                            ValueLabel = "70",
                            Color = SKColor.Parse("#2c3e50")
                        },
                        new Entry(42)
                        {
                            Label = "Android",
                            ValueLabel = "70",
                            Color = SKColor.Parse("#b455b6")
                        },
                    }
                },
                new Entry(248)
                {
                    Label = "Android",
                    ValueLabel = "248",
                    Color = SKColor.Parse("#77d065")
                },
                new Entry(128)
                {
                    Label = "iOS",
                    ValueLabel = "128",
                    Color = SKColor.Parse("#b455b6")
                },
                new Entry(514)
                {
                    Label = "Shared",
                    ValueLabel = "514",
                    Color = SKColor.Parse("#3498db")
                }
            };
           Device.BeginInvokeOnMainThread(() =>
           {
                PieChartTotal = pieChartTotal;//ui related
               
           });
                PieChart = new DonutChart
                {
                    Entries = shiftStatusEntries,
                    LabelTextSize = 35,
                    LabelHeightMargin = 60,
                    LabelTextMaxSpace = 320
                };
                Charts = new ObservableCollection<Chart>
            {
                new DonutChart() { Entries = shiftStatusEntries,
                    LabelTextSize = 30,
                    LabelHeightMargin = 60,
                    LabelTextMaxSpace =320
                },
                new LineChart()
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    LineSize = 8,
                    PointMode = PointMode.Square,
                    PointSize = 18,
                    LabelTextSize =40
                },
                new BarChart() { Entries = entries,LabelTextSize =40 ,MarginLeft = 100},
                new PointChart() { Entries = entries },

                new RadialGaugeChart() { Entries = entries },
                new RadarChart() { Entries = entries },
            };


            }
            catch (Exception)
            {
                
            }
            LoadStatusPieChartDataCommand.ChangeCanExecute();

        }
        #endregion

        #region Fields

        #endregion
    }
}
