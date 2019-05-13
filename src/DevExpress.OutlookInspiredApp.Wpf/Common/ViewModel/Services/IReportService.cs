using System;
using DevExpress.XtraReports;

namespace DevExpress.DevAV.Common.ViewModel {
    public interface IReportInfo {
        object ParametersViewModel { get; }
        IReport CreateReport();
    }
    public class ParameterlessReportInfo : IReportInfo {
        IReport report;

        public ParameterlessReportInfo(IReport report) {
            this.report = report;
        }

        object IReportInfo.ParametersViewModel { get { return null; } }
        IReport IReportInfo.CreateReport() { return report; }
    }
    public class ReportInfo<TParametersViewModel> : IReportInfo {
        TParametersViewModel parametersViewModel;
        Func<TParametersViewModel, IReport> reportFactory;

        public ReportInfo(TParametersViewModel parametersViewModel, Func<TParametersViewModel, IReport> reportFactory) {
            this.parametersViewModel = parametersViewModel;
            this.reportFactory = reportFactory;
        }

        object IReportInfo.ParametersViewModel { get { return parametersViewModel; } }
        IReport IReportInfo.CreateReport() { return reportFactory(parametersViewModel); }
    }
    public interface IReportService {
        void SetDefaultReport(IReportInfo reportInfo);
        void ShowReport(IReportInfo reportInfo);
    }
}
