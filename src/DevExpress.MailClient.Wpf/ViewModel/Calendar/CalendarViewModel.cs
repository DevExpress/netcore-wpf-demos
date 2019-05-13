using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Scheduling;

namespace DevExpress.MailClient.ViewModel {
    public class CalendarViewModel : ContentViewModelBase<AppointmentViewModel> {
        public static CalendarViewModel Create() {
            return ViewModelSource.Create(() => new CalendarViewModel());
        }

        public virtual SchedulerControl SchedulerControl { get; protected set; }

        protected CalendarViewModel() : base(ModuleType.Calendar) { }

        public void Initialize(object parameter) {
            SchedulerControl = parameter as SchedulerControl;
        }
    }
}