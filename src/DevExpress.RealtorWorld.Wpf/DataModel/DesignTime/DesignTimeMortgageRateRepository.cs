using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class DesignTimeMortgageRateRepository : IMortgageRateRepository {
        static List<MortgageRate> mortgageRates;

        IQueryable<MortgageRate> IRepository<MortgageRate, DateTime>.Get() { return GetSampleData(); }
        ObservableCollection<MortgageRate> IRepository<MortgageRate, DateTime>.Local { get { return new ObservableCollection<MortgageRate>(GetSampleData()); } }
        MortgageRate IRepository<MortgageRate, DateTime>.Find(DateTime date) {
            return mortgageRates.Where(h => h.Date == date).FirstOrDefault();
        }
        IUnitOfWork IRepository<MortgageRate, DateTime>.UnitOfWork { get { return null; } }

        static IQueryable<MortgageRate> GetSampleData() {
            if(mortgageRates != null) return mortgageRates.AsQueryable();
            mortgageRates = new List<MortgageRate>();
            mortgageRates.Add(new MortgageRate() { Date = new DateTime(2008, 10, 3), FRM30 = 6.1, FRM15 = 5.78, ARM1 = 5.12 });
            mortgageRates.Add(new MortgageRate() { Date = new DateTime(2008, 10, 10), FRM30 = 5.94, FRM15 = 5.63, ARM1 = 5.15 });
            mortgageRates.Add(new MortgageRate() { Date = new DateTime(2008, 10, 17), FRM30 = 6.46, FRM15 = 6.14, ARM1 = 5.16 });
            return mortgageRates.AsQueryable();
        }
    }
}
