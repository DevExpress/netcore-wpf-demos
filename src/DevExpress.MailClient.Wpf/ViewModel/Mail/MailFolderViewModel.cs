using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Grid;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DevExpress.MailClient.ViewModel {
    public interface IMailFolderDescription {
        int UnreadCount { get; set; }
        MessageFolderName Folder { get; }
        MessageType Type { get; }
        IEnumerable<IMailFolderDescription> GetSubFolders();
    }
    public class MailFolderViewModel : IMailFolderDescription {
        public static MailFolderViewModel Create(string name, Uri icon, MessageFolderName folder, MessageType type) {
            return ViewModelSource.Create(() => new MailFolderViewModel() {
                Name = name,
                Icon = icon,
                Folder = folder,
                Type = type
            });
        }

        public string Name { get; set; }
        public List<MailFolderViewModel> SubFolders { get; set; }
        public Uri Icon { get; set; }
        public MessageFolderName Folder { get; set; }
        public MessageType Type { get; set; }
        public virtual int UnreadCount { get; set; }

        protected MailFolderViewModel() {
            SubFolders = new List<MailFolderViewModel>();
            Type = MessageType.Inbox;
        }

        IEnumerable<IMailFolderDescription> IMailFolderDescription.GetSubFolders() {
            return SubFolders;
        }
    }
    public class MailFoldersChildSelector : IChildNodesSelector {
        IEnumerable IChildNodesSelector.SelectChildren(object item) {
            if(item is IMailFolderDescription)
                return (item as IMailFolderDescription).GetSubFolders();

            return null;
        }
    }
}
