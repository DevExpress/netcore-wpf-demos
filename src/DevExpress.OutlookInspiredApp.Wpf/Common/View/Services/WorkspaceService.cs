using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.DevAV.Common.ViewModel;

namespace DevExpress.DevAV.Common.View {
    public class WorkspaceService : Decorator, IWorkspaceService {
        class WorkspaceServiceInternal : ServiceBase, IWorkspaceService {
            IWorkspaceService workspaceService;

            public WorkspaceServiceInternal(IWorkspaceService workspace) {
                this.workspaceService = workspace;
            }
            public Workspace SaveWorkspace() {
                return workspaceService.SaveWorkspace();
            }
            public void RestoreWorkspace(Workspace workspace) {
                workspaceService.RestoreWorkspace(workspace);
            }
        }
        public static readonly RoutedEvent WorkspaceRegionRegisterEvent =
            EventManager.RegisterRoutedEvent("WorkspaceRegionRegister", RoutingStrategy.Bubble, typeof(WorkspaceRegionEventHandler), typeof(WorkspaceService));
        public static readonly RoutedEvent WorkspaceRegionUnregisterEvent =
            EventManager.RegisterRoutedEvent("WorkspaceRegionUnregister", RoutingStrategy.Bubble, typeof(WorkspaceRegionEventHandler), typeof(WorkspaceService));

        Dictionary<string, IWorkspaceRegion> regions = new Dictionary<string, IWorkspaceRegion>();
        Workspace workspace = new Workspace();
        bool workspaceChanging = false;

        public WorkspaceService() {
            AddHandler(WorkspaceRegionRegisterEvent, new WorkspaceRegionEventHandler(OnWorkspaceRegionRegister));
            AddHandler(WorkspaceRegionUnregisterEvent, new WorkspaceRegionEventHandler(OnWorkspaceRegionUnregister));
            Interaction.GetBehaviors(this).Add(new WorkspaceServiceInternal(this));
        }
        void OnWorkspaceRegionRegister(object sender, WorkspaceRegionEventArgs e) {
            e.Handled = true;
            if(!regions.ContainsKey(e.Region.Id))
                regions.Add(e.Region.Id, e.Region);
            if(!workspaceChanging)
                SyncWorkspaceRegionLayout(e.Region);
        }
        void OnWorkspaceRegionUnregister(object sender, WorkspaceRegionEventArgs e) {
            e.Handled = true;
            regions.Remove(e.Region.Id);
        }
        public Workspace SaveWorkspace() {
            if(workspaceChanging)
                throw new InvalidOperationException();
            workspaceChanging = true;
            workspace = new Workspace();
            foreach(IWorkspaceRegion region in regions.Values)
                workspace.AddRegion(region.Id, region.SaveLayout());
            return workspace;
        }
        public void RestoreWorkspace(Workspace workspace) {
            if(!workspaceChanging)
                throw new InvalidOperationException();
            this.workspace = workspace;
            foreach(IWorkspaceRegion region in regions.Values)
                SyncWorkspaceRegionLayout(region);
            workspaceChanging = false;
        }
        void SyncWorkspaceRegionLayout(IWorkspaceRegion region) {
            string regionLayout = workspace.FindRegionLayout(region.Id);
            if(regionLayout != null)
                region.RestoreLayout(regionLayout);
            else
                workspace.AddRegion(region.Id, region.SaveLayout());
        }
    }
    public interface IWorkspaceRegion {
        string Id { get; }
        string SaveLayout();
        void RestoreLayout(string layout);
    }
    public class WorkspaceRegionEventArgs : RoutedEventArgs {
        public WorkspaceRegionEventArgs(IWorkspaceRegion region) {
            Region = region;
        }
        public IWorkspaceRegion Region { get; private set; }
    }
    public delegate void WorkspaceRegionEventHandler(object sender, WorkspaceRegionEventArgs e);
    public class WorkspaceRegionBehavior : Behavior<FrameworkElement>, IWorkspaceRegion {
        #region Dependency Properties
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(WorkspaceRegionBehavior), new PropertyMetadata(null));
        #endregion
        MethodInfo restoreLayoutFromStreamMethod;
        MethodInfo saveLayoutToStreamMethod;

        public string Id {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            if(AssociatedObject.IsLoaded)
                OnAssociatedObjectLoaded(AssociatedObject, null);
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
            if(AssociatedObject.IsLoaded)
                OnAssociatedObjectUnloaded(AssociatedObject, null);
        }
        void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e) {
            AssociatedObject.RaiseEvent(new WorkspaceRegionEventArgs(this) { RoutedEvent = WorkspaceService.WorkspaceRegionUnregisterEvent });
        }
        void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) {
            AssociatedObject.RaiseEvent(new WorkspaceRegionEventArgs(this) { RoutedEvent = WorkspaceService.WorkspaceRegionRegisterEvent });
        }
        MethodInfo GetMethod(Type type, string methodName) {
            for(; type != null; type = type.BaseType) {
                MethodInfo method = type.GetMethod(methodName);
                if(method != null) return method;
            }
            throw new InvalidOperationException();
        }
        MethodInfo RestoreLayoutFromStreamMethod {
            get {
                if(restoreLayoutFromStreamMethod == null)
                    restoreLayoutFromStreamMethod = GetMethod(AssociatedObject.GetType(), "RestoreLayoutFromStream");
                return restoreLayoutFromStreamMethod;
            }
        }
        MethodInfo SaveLayoutToStreamMethod {
            get {
                if(saveLayoutToStreamMethod == null)
                    saveLayoutToStreamMethod = GetMethod(AssociatedObject.GetType(), "SaveLayoutToStream");
                return saveLayoutToStreamMethod;
            }
        }
        void SaveLayoutToStream(Stream stream) {
            SaveLayoutToStreamMethod.Invoke(AssociatedObject, new object[] { stream });
        }
        void RestoreLayoutFromStream(Stream stream) {
            RestoreLayoutFromStreamMethod.Invoke(AssociatedObject, new object[] { stream });
        }
        string IWorkspaceRegion.Id { get { return Id; } }
        string IWorkspaceRegion.SaveLayout() {
            using (MemoryStream stream = new MemoryStream()) {
                SaveLayoutToStream(stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        void IWorkspaceRegion.RestoreLayout(string layout) {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(layout))) {
                RestoreLayoutFromStream(stream);
            }
        }
    }
}