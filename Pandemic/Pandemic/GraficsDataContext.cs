using OxyPlot;
using OxyPlot.Axes;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandemic
{
    class GraficsDataContext : BindableBase
    {
        private double beginPandemic;
        public double BeginPandemic
        {
            get => beginPandemic;
            set
            {
                if (beginPandemic.Equals(value)) return;
                beginPandemic = value;
                RaisePropertyChanged(nameof(BeginPandemic));
            }
        }
        private double endPandemic;
        public double EndPandemic
        {
            get => endPandemic;
            set
            {
                if (endPandemic.Equals(value)) return;
                endPandemic = value;
                RaisePropertyChanged(nameof(EndPandemic));
            }
        }
        private double totalInfected;
        public double TotalInfected
        {
            get => totalInfected;
            set
            {
                if (totalInfected.Equals(value)) return;
                totalInfected = value;
                RaisePropertyChanged(nameof(TotalInfected));
            }
        }
        private double maxPerDay;
        public double MaxPerDay
        {
            get => maxPerDay;
            set
            {
                if (maxPerDay.Equals(value)) return;
                maxPerDay = value;
                RaisePropertyChanged(nameof(MaxPerDay));
            }
        }
        private ObservableCollection<DataPoint> dinamycPoints;
        public ObservableCollection<DataPoint> DinamycPoints
        {
            get => dinamycPoints;
            set
            {
                if (dinamycPoints.Equals(value)) return;
                dinamycPoints = value;
                RaisePropertyChanged(nameof(DinamycPoints));
            }
        }
        private ObservableCollection<DataPoint> totalInfectedPoints;
        public ObservableCollection<DataPoint> TotalInfectedPoints
        {
            get => totalInfectedPoints;
            set
            {
                if (totalInfectedPoints.Equals(value)) return;
                totalInfectedPoints = value;
                RaisePropertyChanged(nameof(TotalInfectedPoints));
            }
        }
        private ObservableCollection<DataPoint> currentInfectedPoints;
        public ObservableCollection<DataPoint> CurrentInfectedPoints
        {
            get => currentInfectedPoints;
            set
            {
                if (currentInfectedPoints.Equals(value)) return;
                currentInfectedPoints = value;
                RaisePropertyChanged(nameof(CurrentInfectedPoints));
            }
        }


        public GraficsDataContext()
        {
            BeginPandemic = 0;
            EndPandemic = 1;

            TotalInfected = 1;
            MaxPerDay = 1;

            totalInfectedPoints = new ObservableCollection<DataPoint>();
            currentInfectedPoints = new ObservableCollection<DataPoint>();
            dinamycPoints = new ObservableCollection<DataPoint>();
            
        }
        public GraficsDataContext(IList<(DateTime date, int totalInfected, int currentInfected)> statistics)
        {
            BeginPandemic = DateTimeAxis.ToDouble(statistics.First().date);
            EndPandemic = DateTimeAxis.ToDouble(statistics.Last().date + new TimeSpan(2, 0, 0, 0));

            TotalInfected = statistics.Sum(day => day.totalInfected) + 100;
            MaxPerDay = statistics.Max(day => day.totalInfected) + 10;

            dinamycPoints = new ObservableCollection<DataPoint>();
            totalInfectedPoints = new ObservableCollection<DataPoint>();
            currentInfectedPoints = new ObservableCollection<DataPoint>();

            ConvertData(statistics);
        }

        private void ConvertData(IList<(DateTime date, int totalInfected, int currentInfected)> statistics)
        {
            double sumInfected = 0;

            foreach ((DateTime date, int totalInfected, int currentInfected) day in statistics)
            {
                double date = DateTimeAxis.ToDouble(day.date);

                DataPoint dynamicPoint = new DataPoint(date, day.totalInfected);
                DinamycPoints.Add(dynamicPoint);

                sumInfected += day.totalInfected;
                DataPoint totalInfectedPoint = new DataPoint(date, sumInfected);
                TotalInfectedPoints.Add(totalInfectedPoint);
                
                DataPoint currentInfectedPoint = new DataPoint(date, day.currentInfected);
                CurrentInfectedPoints.Add(currentInfectedPoint);
            }
        }
        public override bool Equals(object obj)
        {
            var context = obj as GraficsDataContext;
            return context != null &&
                   beginPandemic == context.beginPandemic &&
                   endPandemic == context.endPandemic &&
                   totalInfected == context.totalInfected &&
                   maxPerDay == context.maxPerDay;
        }
    }
}
