using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataModel;
using DevExpress.Xpf.Core;

namespace DevExpress.DevAV.ViewModels {
    public class MailMergeViewModel<TEntity, TLinks> : IDocumentContent
        where TEntity : class
        where TLinks : class {

        public static MailMergeViewModel<TEntity, TLinks> Create<TUnitOfWork, TPrimaryKey>(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, TPrimaryKey? key, string selectedTemplateName = null, TLinks linksViewModel = null)
            where TUnitOfWork : IUnitOfWork
            where TPrimaryKey : struct {
            var repository = getRepositoryFunc(unitOfWorkFactory.CreateUnitOfWork());
            var entities = repository.ToArray();
            var selectedEntity = key != null ? repository.Find(key.Value) : null;
            return ViewModelSource.Create(() => new MailMergeViewModel<TEntity, TLinks>(entities, selectedEntity, selectedTemplateName, linksViewModel));
        }

        protected MailMergeViewModel(IEnumerable<TEntity> entities, TEntity selectedEntity, string selectedTemplateName, TLinks linksViewModel) {
            Templates = MailMergeTemplatesHelper.GetAllTemplates();
            SelectedTemplate = Templates.FirstOrDefault(t => t.Name == selectedTemplateName);
            IsAdditionParametersVisible = SelectedTemplate != null;
            SelectedTemplate = SelectedTemplate ?? Templates.FirstOrDefault();
            LinksViewModel = linksViewModel;

            Entities = entities;
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
                SelectedEntity = selectedEntity;
            }));
            ActiveRecord = -1;
            Logger.Log(string.Format("OutlookInspiredApp: View Quick Letter: {0}", selectedTemplateName));
        }
        Locker locker = new Locker();
        public virtual TEntity SelectedEntity { get; set; }
        public virtual int ActiveRecord { get; set; }
        public virtual IEnumerable<TEntity> Entities { get; set; }

        public virtual List<TemplateViewModel> Templates { get; set; }
        public virtual TemplateViewModel SelectedTemplate { get; set; }
        public virtual bool IsAdditionParametersVisible { get; set; }
        public virtual TLinks LinksViewModel { get; protected set; }
        public void Close() {
            if(DocumentOwner != null)
                DocumentOwner.Close(this);
        }
        protected IDocumentOwner DocumentOwner { get; private set; }

        public void OnSelectedEntityChanged() {
            this.locker.DoLockedActionIfNotLocked(() => {
                if(Entities != null) {
                    var index = Entities.Cast<object>().Select((x, i) => new { item = x, index = i }).FirstOrDefault(x => x.item == SelectedEntity);
                    ActiveRecord = index != null ? index.index : -1;
                } else
                    ActiveRecord = -1;
            });
        }
        public void OnActiveRecordChanged() {
            this.locker.DoLockedActionIfNotLocked(() => {
                if(Entities != null) {
                    var obj = Entities.Cast<object>().Select((x, i) => new { item = x, index = i }).FirstOrDefault(x => x.index == ActiveRecord);
                    SelectedEntity = obj != null ? obj.item as TEntity : null;
                } else
                    SelectedEntity = null;
            });
        }
        public void OnSelectedTemplateChanged() {
            int activeRecord = ActiveRecord;
            ActiveRecord = -1;
            ActiveRecord = activeRecord;
        }
        #region IDocumentContent
        void IDocumentContent.OnClose(CancelEventArgs e) { }
        void IDocumentContent.OnDestroy() { }
        IDocumentOwner IDocumentContent.DocumentOwner {
            get { return DocumentOwner; }
            set { DocumentOwner = value; }
        }
        object IDocumentContent.Title { get { return "DevAV - Mail Merge"; } }
        #endregion
    }
}