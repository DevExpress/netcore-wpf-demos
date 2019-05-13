using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.RealtorWorld.Xpf.Helpers;
using DevExpress.Mvvm;
using System.Windows.Threading;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public sealed class CalculatorViewModel : ViewModel, IDocumentContent {
        static decimal? savedLoanAmount;
        static decimal? savedInterestRate;
        static int? savedTermOfLoan;
        static DateTime? savedStartMonth;
        decimal payment;
        decimal loanAmount;
        decimal interestRate;
        List<FormatValue> interestRatesList;
        int termOfLoan;
        List<FormatValue> termOfLoanList;
        DateTime startMonth;
        List<FormatValue> startMonthList;
        List<LoanPayment> payments;
        List<LoanPayment> calculatedPayments;
        decimal calculatedMonthlyPayment;
        List<YearPayment> yearPayments;
        List<YearPayment> calculatedYearPayments;
        ICommand calculateCommand;

        public CalculatorViewModel() {
            LoanAmount = savedLoanAmount == null ? 250000M : (decimal)savedLoanAmount;
            InterestRatesList = GetInterestRatesList();
            InterestRate = savedInterestRate == null ? (decimal)InterestRatesList[25].Value : (decimal)savedInterestRate;
            TermOfLoanList = GetTermOfLoanList();
            TermOfLoan = savedTermOfLoan == null ? (int)TermOfLoanList[3].Value : (int)savedTermOfLoan;
            StartMonthList = GetStartMonthList();
            StartMonth = savedStartMonth == null ? (DateTime)StartMonthList[0].Value : (DateTime)savedStartMonth;
            Calculate(false);
        }
        IDocumentOwner IDocumentContent.DocumentOwner { get; set; }
        object IDocumentContent.Title { get { return null; } }
        void IDocumentContent.OnClose(CancelEventArgs e) {
            savedLoanAmount = LoanAmount;
            savedInterestRate = InterestRate;
            savedTermOfLoan = TermOfLoan;
            savedStartMonth = StartMonth;
        }
        void IDocumentContent.OnDestroy() { }

        public decimal Payment {
            get { return payment; }
            private set { SetProperty(ref payment, value, () => Payment); }
        }
        public decimal LoanAmount {
            get { return loanAmount; }
            set { SetProperty(ref loanAmount, value, () => LoanAmount); }
        }
        public decimal InterestRate {
            get { return interestRate; }
            set { SetProperty(ref interestRate, value, () => InterestRate); }
        }
        public List<FormatValue> InterestRatesList {
            get { return interestRatesList; }
            private set { SetProperty(ref interestRatesList, value, () => InterestRatesList); }
        }
        public int TermOfLoan {
            get { return termOfLoan; }
            set { SetProperty(ref termOfLoan, value, () => TermOfLoan); }
        }
        public List<FormatValue> TermOfLoanList {
            get { return termOfLoanList; }
            private set { SetProperty(ref termOfLoanList, value, () => TermOfLoanList); }
        }
        public DateTime StartMonth {
            get { return startMonth; }
            set { SetProperty(ref startMonth, value, () => StartMonth); }
        }
        public List<FormatValue> StartMonthList {
            get { return startMonthList; }
            private set { SetProperty(ref startMonthList, value, () => StartMonthList); }
        }
        public List<LoanPayment> Payments {
            get { return payments; }
            private set { SetProperty(ref payments, value, () => Payments); }
        }
        public List<YearPayment> YearPayments {
            get { return yearPayments; }
            private set { SetProperty(ref yearPayments, value, () => YearPayments); }
        }
        public ICommand CalculateCommand {
            get {
                if(calculateCommand == null)
                    calculateCommand = new DelegateCommand(() => Calculate(true));
                return calculateCommand;
            }
        }
        public void Calculate(bool async) {
            if(async) {
                var splashScreenService = GetService<ISplashScreenService>();
                splashScreenService.ShowSplashScreen();
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
                Task.Factory.StartNew(BeginCalculate).ContinueWith(t => dispatcher.BeginInvoke((Action)(()=> {
                    EndCalculate();
                    splashScreenService.HideSplashScreen();
                })));
            } else {
                BeginCalculate();
                EndCalculate();
            }
        }
        void BeginCalculate() {
            double monthlyPayment;
            this.calculatedPayments = LoanPayment.Calculate((double)LoanAmount, (double)(InterestRate / 12M), (double)(TermOfLoan * 12), StartMonth, out monthlyPayment);
            this.calculatedMonthlyPayment = (decimal)monthlyPayment;
            this.calculatedYearPayments = YearPayment.Calculate(this.calculatedPayments);
        }
        void EndCalculate() {
            Payments = this.calculatedPayments;
            Payment = this.calculatedMonthlyPayment;
            YearPayments = this.calculatedYearPayments;
            this.calculatedPayments = null;
            this.calculatedYearPayments = null;
        }
        List<FormatValue> GetInterestRatesList() {
            List<FormatValue> interestRatesList = new List<FormatValue>();
            for(decimal interestRate = 0.025M; interestRate < 0.15M; interestRate += 0.00125M)
                interestRatesList.Add(new FormatValue() { Value = interestRate, Text = string.Format("{0:p3}", interestRate) });
            return interestRatesList;
        }
        List<FormatValue> GetStartMonthList() {
            List<FormatValue> startMonthList = new List<FormatValue>();
            DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            for(int i = 1; i < 7; i++) {
                DateTime month = start.AddMonths(i);
                startMonthList.Add(new FormatValue() { Value = month, Text = string.Format("{0:MMMM, yyyy}", month) });
            }
            return startMonthList;
        }
        List<FormatValue> GetTermOfLoanList() {
            List<FormatValue> list = new List<FormatValue>();
            foreach(int term in new int[] { 1, 5, 10, 15, 20, 25, 30 })
                list.Add(new FormatValue() { Value = term, Text = term.ToString() + (term == 1 ? " year" : " years") });
            return list;
        }
    }
}