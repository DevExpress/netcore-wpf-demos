using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Properties;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using System;

namespace DevExpress.DevAV.ViewModels {
    internal static class FiltersSettings {
        public static FilterTreeViewModel<Employee, long> GetEmployeesFilterTree(object parentViewModel) {
            return FilterTreeViewModel<Employee, long>.Create(
                new FilterTreeModelPageSpecificSettings<Settings>(Settings.Default, "Status", x => x.EmployeesStaticFilters, x => x.EmployeesCustomFilters,
                    new[] {
                        BindableBase.GetPropertyName(() => new Employee().FullNameBindable),
                        BindableBase.GetPropertyName(() => new Employee().Id),
                        BindableBase.GetPropertyName(() => new Employee().Picture),
                        BindableBase.GetPropertyName(() => new Employee().PictureId),
                    },
                    new[] {
                        BindableBase.GetPropertyName(() => new Employee().Address) + "." + BindableBase.GetPropertyName(() => new Address().City),
                        BindableBase.GetPropertyName(() => new Employee().Address) + "." + BindableBase.GetPropertyName(() => new Address().State),
                        BindableBase.GetPropertyName(() => new Employee().Address) + "." + BindableBase.GetPropertyName(() => new Address().ZipCode),
                    }),
                CreateUnitOfWork().Employees, new Action<object, Action>(RegisterEntityChangedMessageHandler<Employee, long>)
            ).SetParentViewModel(parentViewModel);
        }
        public static FilterTreeViewModel<EmployeeTask, long> GetTasksFilterTree(object parentViewModel)
        {
            return FilterTreeViewModel<EmployeeTask, long>.Create(
                new FilterTreeModelPageSpecificSettings<Settings>(Settings.Default, "Tasks", x => x.TasksStaticFilters, x => x.TasksCustomFilters, null, null, "By Employee"),
                CreateUnitOfWork().Tasks, new Action<object, Action>(RegisterEntityChangedMessageHandler<EmployeeTask, long>)
            ).SetParentViewModel(parentViewModel);
        }
        public static FilterTreeViewModel<Customer, long> GetCustomersFilterTree(object parentViewModel) {
            return FilterTreeViewModel<Customer, long>.Create(
                new FilterTreeModelPageSpecificSettings<Settings>(Settings.Default, "Favorites", x => x.CustomersStaticFilters, x => x.CustomersCustomFilters,
                    new[] {
                        BindableBase.GetPropertyName(() => new Customer().Id),
                    },
                    new[] {
                        BindableBase.GetPropertyName(() => new Customer().BillingAddress) + "." + BindableBase.GetPropertyName(() => new Address().City),
                        BindableBase.GetPropertyName(() => new Customer().BillingAddress) + "." + BindableBase.GetPropertyName(() => new Address().State),
                        BindableBase.GetPropertyName(() => new Customer().BillingAddress) + "." + BindableBase.GetPropertyName(() => new Address().ZipCode),
                    }),
                CreateUnitOfWork().Customers, new Action<object, Action>(RegisterEntityChangedMessageHandler<Customer, long>)
            ).SetParentViewModel(parentViewModel);
        }
        public static FilterTreeViewModel<Product, long> GetProductsFilterTree(object parentViewModel) {
            return FilterTreeViewModel<Product, long>.Create(
                new FilterTreeModelPageSpecificSettings<Settings>(Settings.Default, "Category", x => x.ProductsStaticFilters, x => x.ProductsCustomFilters, null,
                    new[] {
                        BindableBase.GetPropertyName(() => new Product().Id),
                        BindableBase.GetPropertyName(() => new Product().EngineerId),
                        BindableBase.GetPropertyName(() => new Product().PrimaryImageId),
                        BindableBase.GetPropertyName(() => new Product().SupportId),
                        BindableBase.GetPropertyName(() => new Product().Support),
                    }),
                CreateUnitOfWork().Products, new Action<object, Action>(RegisterEntityChangedMessageHandler<Product, long>)
            ).SetParentViewModel(parentViewModel);
        }
        public static FilterTreeViewModel<Order, long> GetSalesFilterTree(object parentViewModel) {
            return FilterTreeViewModel<Order, long>.Create(
                new FilterTreeModelPageSpecificSettings<Settings>(Settings.Default, "Category", x => x.OrdersStaticFilters, x => x.OrdersCustomFilters,
                    new[] {
                        BindableBase.GetPropertyName(() => new Order().Id),
                        BindableBase.GetPropertyName(() => new Order().CustomerId),
                        BindableBase.GetPropertyName(() => new Order().EmployeeId),
                        BindableBase.GetPropertyName(() => new Order().StoreId),
                    },
                    new[] {
                        BindableBase.GetPropertyName(() => new Order().Customer) + "." + BindableBase.GetPropertyName(() => new Customer().Name),
                    }),
                CreateUnitOfWork().Orders.ActualOrders(), new Action<object, Action>(RegisterEntityChangedMessageHandler<Order, long>)
            ).SetParentViewModel(parentViewModel);
        }
        public static FilterTreeViewModel<Quote, long> GetOpportunitiesFilterTree(object parentViewModel) {
            return FilterTreeViewModel<Quote, long>.Create(
                new FilterTreeModelPageSpecificSettings<Settings>(Settings.Default, "Category", x => x.QuotesStaticFilters, null, null),
                CreateUnitOfWork().Quotes.ActualQuotes(), new Action<object, Action>(RegisterEntityChangedMessageHandler<Quote, long>)
            ).SetParentViewModel(parentViewModel);
        }

        static IDevAVDbUnitOfWork CreateUnitOfWork() {
            return UnitOfWorkSource.GetUnitOfWorkFactory().CreateUnitOfWork();
        }

        static void RegisterEntityChangedMessageHandler<TEntity, TPrimaryKey>(object recipient, Action handler) {
            Messenger.Default.Register<EntityMessage<TEntity, TPrimaryKey>>(recipient, message => handler());
        }
    }
}